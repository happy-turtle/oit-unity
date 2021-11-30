using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class OrderIndependentTransparency : MonoBehaviour
{
    public Shader customRenderingShader = null;
    [Tooltip("This can be increased if objects disappear or block artifacts appear. A lower value keeps the used video memory at a minimum.")]
    [Range(1f, 5f)]
    public int listSizeMultiplier = 1;

    private static ComputeBuffer fragmentLinkBuffer;
    private static ComputeBuffer startOffsetBuffer;
    private int bufferSize;
    private int bufferStride;
    private Material linkedListMaterial;
    private uint[] resetTable;


    private void OnEnable()
    {
        linkedListMaterial = new Material(customRenderingShader != null ? customRenderingShader : Shader.Find("Hidden/LinkedListRendering"));
        int bufferWidth = Screen.width > 0 ? Screen.width : 1024;
        int bufferHeight = Screen.height > 0 ? Screen.height : 1024;
        int msaaFactor = QualitySettings.antiAliasing > 0 ? QualitySettings.antiAliasing : 1;

        int bufferSize = bufferWidth * bufferHeight * msaaFactor * listSizeMultiplier;
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

    private void OnDisable()
    {
        fragmentLinkBuffer?.Dispose();
        startOffsetBuffer?.Dispose();
    }
}
