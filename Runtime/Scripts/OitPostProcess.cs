using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
public sealed class OitModeParameter : ParameterOverride<OitMode>
{
}

[Serializable]
[PostProcess(typeof(OitPostProcessRenderer), PostProcessEvent.BeforeTransparent, "OrderIndependentTransparency")]
public sealed class OitPostProcess : PostProcessEffectSettings
{
}

public sealed class OitPostProcessRenderer : PostProcessEffectRenderer<OitPostProcess>
{
    private IOrderIndependentTransparency orderIndependentTransparency;

    public override void Init()
    {
        base.Init();
        orderIndependentTransparency = new OitLinkedList();

        Camera.onPreRender += PreRender;
    }

    private void PreRender(Camera cam)
    {
        orderIndependentTransparency.PreRender();
    }

    public override void Render(PostProcessRenderContext context)
    {
        orderIndependentTransparency.Render(context.command, context.source, context.destination);
    }

    public override void Release()
    {
        base.Release();
        orderIndependentTransparency.Release();
        Camera.onPreRender -= PreRender;
    }
}