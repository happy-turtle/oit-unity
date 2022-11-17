using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OitPassURP : ScriptableRenderPass
{
    private readonly IOrderIndependentTransparency orderIndependentTransparency;

    public OitPassURP()
    {
        orderIndependentTransparency = new OitLinkedList();
    }

    public override void Frame(CommandBuffer cmd, ref RenderingData renderingData)
    {
        orderIndependentTransparency.PreRender(cmd);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get("Order Independent Transparency");
        cmd.Clear();
        orderIndependentTransparency.Render(cmd, renderingData.cameraData.renderer.cameraColorTarget, renderingData.cameraData.renderer.cameraColorTarget);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnFinishCameraStackRendering(CommandBuffer cmd)
    {
        orderIndependentTransparency.Release();
    }
}