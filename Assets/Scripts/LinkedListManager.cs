using UnityEngine;

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
    private uint[] resetTable;

    private void Start()
    {
        cam = GetComponent<Camera>();
        linkedListMaterial = new Material(listRenderingShader);

        int bufferSize = Screen.width * Screen.height * listDepth;
        int bufferStride = sizeof(float) * 5 + sizeof(uint);
        //the structured buffer contains all information about the transparent fragments
        //this is the per pixel linked list on the gpu
        fragmentLinkBuffer = new ComputeBuffer(bufferSize, bufferStride, ComputeBufferType.Counter);

        int bufferSizeHead = Screen.width * Screen.height;
        int bufferStrideHead = sizeof(uint);
        //create buffer for addresses, this is the head of the linked list
        startOffsetBuffer = new ComputeBuffer(bufferSizeHead, bufferStrideHead, ComputeBufferType.Raw);

        resetTable = new uint[bufferSizeHead];

        transparencyCam.depthTextureMode = DepthTextureMode.Depth;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //reset StartOffsetBuffer to zeros and reset counter of the StructuredBuffer
        startOffsetBuffer.SetData(resetTable);
        fragmentLinkBuffer.SetCounterValue(0);

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

    private void OnDestroy()
    {
        if (fragmentLinkBuffer != null)
            fragmentLinkBuffer.Dispose();
        if (startOffsetBuffer != null)
            startOffsetBuffer.Dispose();
    }
}
