using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class OrderIndependentTransparency : MonoBehaviour
{
    public Shader listRenderingShader = null;

    private ComputeBuffer fragmentLinkBuffer;
    private ComputeBuffer startOffsetBuffer;
    private int bufferSize;
    private int bufferStride;
    private Material linkedListMaterial;
    private uint[] resetTable;

    private const int LIST_SIZE_MULTPLIER = 1;

    private void OnEnable()
    {
        linkedListMaterial = new Material(listRenderingShader);

        int bufferSize = Screen.width * Screen.height * LIST_SIZE_MULTPLIER;
        int bufferStride = sizeof(float) * 5 + sizeof(uint);
        //the structured buffer contains all information about the transparent fragments
        //this is the per pixel linked list on the gpu
        fragmentLinkBuffer = new ComputeBuffer(bufferSize, bufferStride, ComputeBufferType.Counter);

        int bufferSizeHead = Screen.width * Screen.height;
        int bufferStrideHead = sizeof(uint);
        //create buffer for addresses, this is the head of the linked list
        startOffsetBuffer = new ComputeBuffer(bufferSizeHead, bufferStrideHead, ComputeBufferType.Raw);

        resetTable = new uint[bufferSizeHead];
    }

    private void OnPreRender()
    {
        //reset StartOffsetBuffer to zeros
        startOffsetBuffer.SetData(resetTable);

        Graphics.SetRandomWriteTarget(1, fragmentLinkBuffer);
        Graphics.SetRandomWriteTarget(2, startOffsetBuffer);

    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.ClearRandomWriteTargets();
        // blend linked list
        linkedListMaterial.SetBuffer("FLBuffer", fragmentLinkBuffer);
        linkedListMaterial.SetBuffer("StartOffsetBuffer", startOffsetBuffer);
        Graphics.Blit(source, destination, linkedListMaterial);
    }

    private void OnDisable()
    {
        if (fragmentLinkBuffer != null)
            fragmentLinkBuffer.Dispose();
        if (startOffsetBuffer != null)
            startOffsetBuffer.Dispose();
    }
}
