using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OitPassURP : ScriptableRenderPass
{
    private readonly IOrderIndependentTransparency orderIndependentTransparency;

    public OitPassURP()
    {
        orderIndependentTransparency = new OitLinkedList();
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        orderIndependentTransparency.PreRender();
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        // orderIndependentTransparency.Render(context, context.source, context.destination);
    }

    public override void OnFinishCameraStackRendering(CommandBuffer cmd)
    {
        orderIndependentTransparency.Release();
    }
}