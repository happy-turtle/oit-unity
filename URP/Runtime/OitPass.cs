using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OrderIndependentTransparency.URP
{
    internal class OitPass : ScriptableRenderPass
    {
        private readonly IOrderIndependentTransparency orderIndependentTransparency;

        public OitPass()
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
            orderIndependentTransparency = new OitLinkedList("OitRender");
            RenderPipelineManager.beginCameraRendering += PreRender;
        }

        private void PreRender(ScriptableRenderContext context, Camera camera)
        {
            CommandBuffer cmd = CommandBufferPool.Get("Order Independent Transparency Pre Render");
            orderIndependentTransparency.PreRender(cmd, camera);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("Order Independent Transparency");
            var mat = orderIndependentTransparency.Render(cmd, renderingData.cameraData.renderer.cameraColorTargetHandle,
                renderingData.cameraData.renderer.cameraColorTargetHandle);
            if (mat != null)
            {
                Blitter.BlitCameraTexture(cmd, renderingData.cameraData.renderer.cameraColorTargetHandle,
                    renderingData.cameraData.renderer.cameraColorTargetHandle, mat, 0);
            }
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        public void Cleanup()
        {
            orderIndependentTransparency.Release();
            RenderPipelineManager.beginCameraRendering -= PreRender;
        }
    }
}