using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace OrderIndependentTransparency.HDRP
{
    class OitRenderPass : CustomPass
    {
        [SerializeField]
        LayerMask objectLayerMask = 0;
        OitLinkedList orderIndependentTransparency;

        protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
        {
            orderIndependentTransparency ??= new OitLinkedList();
        }

        protected override void Execute(CustomPassContext ctx)
        {
            var cmd = CommandBufferPool.Get("Order Independent Transparency Pre Render");
            cmd.Clear();
            orderIndependentTransparency.PreRender(cmd);
            ctx.renderContext.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
            CustomPassUtils.DrawRenderers(ctx, objectLayerMask);
            orderIndependentTransparency.Render(ctx.cmd, ctx.cameraColorBuffer, ctx.cameraColorBuffer);
        }

        protected override void Cleanup()
        {
            orderIndependentTransparency.Release();
        }
    }
}