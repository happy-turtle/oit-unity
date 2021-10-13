using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(OrderIndependentTransparencyPPRenderer), PostProcessEvent.BeforeStack, "OrderIndependentTransparency")]
public sealed class OrderIndependentTransparencyPP : PostProcessEffectSettings
{
}
public sealed class OrderIndependentTransparencyPPRenderer : PostProcessEffectRenderer<OrderIndependentTransparencyPP>
{
    private Material linkedListMaterial;

    public override void Init()
    {
        base.Init();
        linkedListMaterial = new Material(Shader.Find("Hidden/LinkedListRendering"));
    }

    public override void Render(PostProcessRenderContext context)
    {
        if (linkedListMaterial != null && OrderIndependentTransparency.buffersAreReady())
        {
            linkedListMaterial.SetBuffer("FLBuffer", OrderIndependentTransparency.GetPerPixelLinkedListBuffer());
            linkedListMaterial.SetBuffer("StartOffsetBuffer", OrderIndependentTransparency.GetPerPixelLinkedListHeadBuffer());
            context.command.Blit(context.source, context.destination, linkedListMaterial);
        }
    }
}