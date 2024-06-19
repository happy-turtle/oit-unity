using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace OrderIndependentTransparency.PostProcessingStackV2
{
    [Serializable]
    [PostProcess(typeof(OitPostProcessRenderer), PostProcessEvent.BeforeTransparent, "OrderIndependentTransparency")]
    internal class OitPostProcess : PostProcessEffectSettings
    {
    }

    internal class OitPostProcessRenderer : PostProcessEffectRenderer<OitPostProcess>
    {
        private IOrderIndependentTransparency orderIndependentTransparency;
        private CommandBuffer cmdPreRender;

        public override void Init()
        {
            base.Init();
            orderIndependentTransparency ??= new OitLinkedList("OitRender");
            Camera.onPreRender += PreRender;
        }

        private void PreRender(Camera cam)
        {
            cmdPreRender ??= new CommandBuffer();
            cmdPreRender.Clear();
            orderIndependentTransparency?.PreRender(cmdPreRender);
            Graphics.ExecuteCommandBuffer(cmdPreRender);
        }

        public override void Render(PostProcessRenderContext context)
        {
            var mat = orderIndependentTransparency?.Render(context.command, context.source, context.destination);
            context.command.Blit(context.source, context.destination, mat);
        }

        public override void Release()
        {
            base.Release();
            orderIndependentTransparency?.Release();
            Camera.onPreRender -= PreRender;
        }
    }
}