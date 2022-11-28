using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace OrderIndependentTransparency.HDRP
{
    [Serializable]
    public sealed class OitResourcesParameter : VolumeParameter<OitResources>
    {
    }


    [Serializable]
    [VolumeComponentMenu("Order-Independent Transparency/OIT Post Process Volume")]
    public sealed class OitPostProcessVolume : CustomPostProcessVolumeComponent, IPostProcessComponent
    {

        public OitResourcesParameter shaderResources = new();

        private OitLinkedList orderIndependentTransparency;

        // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > Graphics > HDRP Settings).
        public override CustomPostProcessInjectionPoint injectionPoint =>
            CustomPostProcessInjectionPoint.AfterOpaqueAndSky;

        public bool IsActive()
        {
            return orderIndependentTransparency != null;
        }

        public override void Setup()
        {
            orderIndependentTransparency =
                new OitLinkedList(shaderResources.value.oitFullscreenRender, shaderResources.value.oitComputeUtils);
            RenderPipelineManager.beginContextRendering += PreRender;
        }

        private void PreRender(ScriptableRenderContext context, List<Camera> cameras)
        {
            var cmd = CommandBufferPool.Get("Order Independent Transparency Pre Render");
            cmd.Clear();
            orderIndependentTransparency.PreRender(cmd);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            orderIndependentTransparency.Render(cmd, source, destination);
        }

        public override void Cleanup()
        {
            orderIndependentTransparency.Release();
            RenderPipelineManager.beginContextRendering -= PreRender;
        }
    }
}