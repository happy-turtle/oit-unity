using UnityEngine;

public enum OitMode
{
    MLAB,
    LinkedList,
}

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class OrderIndependentTransparency : MonoBehaviour
{
    [Tooltip("This can be increased if objects disappear or block artifacts appear. A lower value keeps the used video memory at a minimum.")]
    [Range(1f, 24f)]
    public int listSizeMultiplier = 5;

    [Tooltip("Use Multi-Layer Alpha Blending if your graphics target supports shader model 5.1 and the Rasterizer Order Views (ROV) feature." +
                "For legacy shader model 5.0 support use the linked list mode.")]
    public OitMode oitMode = OitMode.MLAB;

    private GraphicsBuffer fragmentLinkBuffer;
    private int fragmentLinkBufferId;
    private GraphicsBuffer startOffsetBuffer;
    private int startOffsetBufferId;
    private int bufferSize;
    private int bufferStride;
    private Material linkedListMaterial;
    private uint[] resetTable;


    private void OnEnable()
    {
        linkedListMaterial = new Material(Shader.Find("Hidden/LinkedListRendering"));
        int bufferWidth = Screen.width > 0 ? Screen.width : 1024;
        int bufferHeight = Screen.height > 0 ? Screen.height : 1024;

        int bufferSize = bufferWidth * bufferHeight * listSizeMultiplier;
        int bufferStride = sizeof(uint) * 3;
        //the structured buffer contains all information about the transparent fragments
        //this is the per pixel linked list on the gpu
        fragmentLinkBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Counter, bufferSize, bufferStride);
        fragmentLinkBufferId = Shader.PropertyToID("FLBuffer");

        int bufferSizeHead = bufferWidth * bufferHeight;
        int bufferStrideHead = sizeof(uint);
        //create buffer for addresses, this is the head of the linked list
        startOffsetBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, bufferSizeHead, bufferStrideHead);
        startOffsetBufferId = Shader.PropertyToID("StartOffsetBuffer");

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
        linkedListMaterial.SetBuffer(fragmentLinkBufferId, fragmentLinkBuffer);
        linkedListMaterial.SetBuffer(startOffsetBufferId, startOffsetBuffer);
        Graphics.Blit(source, destination, linkedListMaterial);
    }

    private void OnDisable()
    {
        fragmentLinkBuffer?.Dispose();
        startOffsetBuffer?.Dispose();
    }
}
