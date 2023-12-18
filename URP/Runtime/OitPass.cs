using System.Collections.Generic;
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
            orderIndependentTransparency = new OitLinkedList();
            RenderPipelineManager.beginContextRendering += PreRender;
        }

        private void PreRender(ScriptableRenderContext context, List<Camera> cameras)
        {
            CommandBuffer cmd = CommandBufferPool.Get("Order Independent Transparency Pre Render");
            cmd.Clear();
            orderIndependentTransparency.PreRender(cmd);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("Order Independent Transparency");
            cmd.Clear();
            var mat = orderIndependentTransparency.Render(cmd, renderingData.cameraData.renderer.cameraColorTargetHandle,
                renderingData.cameraData.renderer.cameraColorTargetHandle);
            Blitter.BlitCameraTexture(cmd, renderingData.cameraData.renderer.cameraColorTargetHandle,
                renderingData.cameraData.renderer.cameraColorTargetHandle, mat, 0);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Cleanup()
        {
            orderIndependentTransparency.Release();
            RenderPipelineManager.beginContextRendering -= PreRender;
        }
    }
}