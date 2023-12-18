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
            // draw objects with UAV targets set
            var preRenderCmd = CommandBufferPool.Get("Order Independent Transparency Pre Render");
            preRenderCmd.Clear();
            orderIndependentTransparency.PreRender(preRenderCmd);
            ctx.renderContext.ExecuteCommandBuffer(preRenderCmd);
            CommandBufferPool.Release(preRenderCmd);
            CustomPassUtils.DrawRenderers(ctx, objectLayerMask);

            // fullscreen blend of transparent pixel buffer
            var mat = orderIndependentTransparency.Render(ctx.cmd, ctx.cameraColorBuffer, ctx.cameraColorBuffer);
            Blitter.BlitCameraTexture(ctx.cmd, ctx.cameraColorBuffer, ctx.cameraColorBuffer, mat, 0);
        }

        protected override void Cleanup()
        {
            orderIndependentTransparency.Release();
        }
    }
}