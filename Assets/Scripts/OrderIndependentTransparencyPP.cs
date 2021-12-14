#if UNITY_POST_PROCESSING_STACK_V2
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(OrderIndependentTransparencyPPRenderer), PostProcessEvent.BeforeStack, "OrderIndependentTransparency")]
public sealed class OrderIndependentTransparencyPP : PostProcessEffectSettings
{
    [Tooltip("This can be increased if objects disappear or block artifacts appear. A lower value keeps the used video memory at a minimum.")]
    [Range(1f, 24f)]
    public IntParameter listSizeMultiplier = new IntParameter { value = 1 };
}
public sealed class OrderIndependentTransparencyPPRenderer : PostProcessEffectRenderer<OrderIndependentTransparencyPP>
{
    private GraphicsBuffer fragmentLinkBuffer;
    private int fragmentLinkBufferId;
    private GraphicsBuffer startOffsetBuffer;
    private int startOffsetBufferId;
    private int bufferSize;
    private int bufferStride;
    private Material linkedListMaterial;
    private uint[] resetTable;

    public override void Init()
    {
        base.Init();
        linkedListMaterial = new Material(Shader.Find("Hidden/LinkedListRendering"));
        linkedListMaterial.EnableKeyword("POST_PROCESSING");
        int bufferWidth = Screen.width > 0 ? Screen.width : 1024;
        int bufferHeight = Screen.height > 0 ? Screen.height : 1024;

        int bufferSize = bufferWidth * bufferHeight * settings.listSizeMultiplier;
        int bufferStride = sizeof(float) * 5 + sizeof(uint);
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

        Camera.onPreRender += PreRender;
    }

    private void PreRender(Camera cam)
    {
        if (fragmentLinkBuffer == null || startOffsetBuffer == null || startOffsetBuffer.count != resetTable.Length)
            return;

        //reset StartOffsetBuffer to zeros
        startOffsetBuffer.SetData(resetTable);

        // set buffers for rendering
        Graphics.SetRandomWriteTarget(1, fragmentLinkBuffer);
        Graphics.SetRandomWriteTarget(2, startOffsetBuffer);
    }

    public override void Render(PostProcessRenderContext context)
    {
        if (fragmentLinkBuffer == null || startOffsetBuffer == null || linkedListMaterial == null)
            return;

        context.command.ClearRandomWriteTargets();
        // blend linked list
        linkedListMaterial.SetBuffer(fragmentLinkBufferId, fragmentLinkBuffer);
        linkedListMaterial.SetBuffer(startOffsetBufferId, startOffsetBuffer);
        context.command.Blit(context.source, context.destination, linkedListMaterial);
    }

    public override void Release()
    {
        base.Release();
        fragmentLinkBuffer?.Dispose();
        startOffsetBuffer?.Dispose();
        Camera.onPreRender -= PreRender;
    }
}
#endif