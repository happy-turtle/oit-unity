using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class OrderIndependentTransparency : MonoBehaviour
{
    public Shader listRenderingShader = null;

    private static ComputeBuffer fragmentLinkBuffer;
    private static ComputeBuffer startOffsetBuffer;
    private int bufferSize;
    private int bufferStride;
    private Material linkedListMaterial;
    private uint[] resetTable;

    private const int LIST_SIZE_MULTPLIER = 1;

    private void OnEnable()
    {
        linkedListMaterial = new Material(listRenderingShader);
        int bufferWidth = Screen.width > 0 ? Screen.width : 1024;
        int bufferHeight = Screen.height > 0 ? Screen.height : 1024;

        int bufferSize = bufferWidth * bufferHeight * LIST_SIZE_MULTPLIER;
        int bufferStride = sizeof(float) * 5 + sizeof(uint);
        //the structured buffer contains all information about the transparent fragments
        //this is the per pixel linked list on the gpu
        fragmentLinkBuffer = new ComputeBuffer(bufferSize, bufferStride, ComputeBufferType.Counter);

        int bufferSizeHead = bufferWidth * bufferHeight;
        int bufferStrideHead = sizeof(uint);
        //create buffer for addresses, this is the head of the linked list
        startOffsetBuffer = new ComputeBuffer(bufferSizeHead, bufferStrideHead, ComputeBufferType.Raw);

        resetTable = new uint[bufferSizeHead];
    }

    private void OnPreRender()
    {
        if (fragmentLinkBuffer == null || startOffsetBuffer == null)
            return;

        //reset StartOffsetBuffer to zeros
        startOffsetBuffer.SetData(resetTable);

        // set buffers for rendering
        Graphics.SetRandomWriteTarget(1, fragmentLinkBuffer);
        Graphics.SetRandomWriteTarget(2, startOffsetBuffer);

    }

#if UNITY_POST_PROCESSING_STACK_V2
    private void OnPostRender()
    {
        Graphics.ClearRandomWriteTargets();
    }
#else
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (fragmentLinkBuffer == null || startOffsetBuffer == null || linkedListMaterial == null)
            return;

        Graphics.ClearRandomWriteTargets();
        // blend linked list
        linkedListMaterial.SetBuffer("FLBuffer", fragmentLinkBuffer);
        linkedListMaterial.SetBuffer("StartOffsetBuffer", startOffsetBuffer);
        Graphics.Blit(source, destination, linkedListMaterial);
    }
#endif

    private void OnDisable()
    {
        fragmentLinkBuffer?.Dispose();
        startOffsetBuffer?.Dispose();
    }


    public static ComputeBuffer GetPerPixelLinkedListBuffer()
    {
        return fragmentLinkBuffer;
    }

    public static ComputeBuffer GetPerPixelLinkedListHeadBuffer()
    {
        return startOffsetBuffer;
    }

    public static bool buffersAreReady()
    {
        if (fragmentLinkBuffer != null && startOffsetBuffer != null)
        {
            return true;
        }
        return false;
    }
}
