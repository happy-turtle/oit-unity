using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class LinkedListManager : MonoBehaviour
{

    public Shader listCreationShader = null;
    public Shader listRenderingShader = null;
    public int listDepth = 3;

    private Camera cam;
    public Camera transparencyCam;

    private ComputeBuffer fragmentLinkBuffer;
    private ComputeBuffer startOffsetBuffer;
    private int bufferSize;
    private int bufferStride;
    private Material linkedListMaterial;
    private uint[] m_listHeadBufferResetTable;

    private void OnEnable()
    {
        cam = GetComponent<Camera>();
        transparencyCam.CopyFrom(cam);
        linkedListMaterial = new Material(listRenderingShader);

        transparencyCam.depthTextureMode = DepthTextureMode.Depth;
        transparencyCam.cullingMask = 1 << LayerMask.NameToLayer("Transparent");

        int bufferSize = Screen.width * Screen.height * listDepth;
        int bufferStride = sizeof(float) * 5 + sizeof(uint);
        //the structured buffer contains all information about the transparent fragments
        //this is the per pixel linked list on the gpu
        fragmentLinkBuffer = new ComputeBuffer(bufferSize, bufferStride, ComputeBufferType.Counter);

        int bufferSizeHead = Screen.width * Screen.height;
        int bufferStrideHead = sizeof(uint);
        //create buffer for addresses, this is the head of the linked list
        startOffsetBuffer = new ComputeBuffer(bufferSizeHead, bufferStrideHead, ComputeBufferType.Raw);

        m_listHeadBufferResetTable = new uint[bufferSizeHead];
        foreach (int i in m_listHeadBufferResetTable)
        {
            m_listHeadBufferResetTable[i] = 0;
        }
    }

    private void OnDisable()
    {
        if (fragmentLinkBuffer != null)
            fragmentLinkBuffer.Dispose();
        if (startOffsetBuffer != null)
            startOffsetBuffer.Dispose();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //reset StartOffsetBuffer to zeros and reset counter of the StructuredBuffer
        startOffsetBuffer.SetData(m_listHeadBufferResetTable);
        fragmentLinkBuffer.SetCounterValue(1);

        // render per pixel linked list for transparent objects
        Graphics.SetRandomWriteTarget(1, fragmentLinkBuffer, true);
        Graphics.SetRandomWriteTarget(2, startOffsetBuffer);
        transparencyCam.targetTexture = source;
        transparencyCam.RenderWithShader(listCreationShader, null);
        Graphics.ClearRandomWriteTargets();

        // blend linked list
        linkedListMaterial.SetBuffer("FLBuffer", fragmentLinkBuffer);
        linkedListMaterial.SetBuffer("StartOffsetBuffer", startOffsetBuffer);
        Graphics.Blit(source, destination, linkedListMaterial);
    }
}
