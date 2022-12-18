using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace OrderIndependentTransparency.HDRP
{
    class OitRenderPass : CustomPass
    {
        public LayerMask transparentLayer = 0;
        private OitLinkedList orderIndependentTransparency;
        private Material material;

        protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
        {
            orderIndependentTransparency ??= new OitLinkedList();
            material = new Material(Shader.Find("Hidden/OitFullscreenRender"));
        }

        protected override void Execute(CustomPassContext ctx)
        {
            var cmd = CommandBufferPool.Get("Order Independent Transparency Pre Render");
            cmd.Clear();
            orderIndependentTransparency.PreRender(cmd);
            ctx.renderContext.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
            CustomPassUtils.DrawRenderers(ctx, transparentLayer);
            orderIndependentTransparency.Render(ctx.cmd, ctx.cameraColorBuffer, ctx.cameraColorBuffer);
        }

        protected override void Cleanup()
        {
            orderIndependentTransparency.Release();
        }
    }
}