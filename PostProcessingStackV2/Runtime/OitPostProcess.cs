using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace OrderIndependentTransparency.PostProcessingStackV2
{
    [Serializable]
    public sealed class ShaderParameter : ParameterOverride<Shader>
    {
    }
    [Serializable]
    public sealed class ComputeShaderParameter : ParameterOverride<ComputeShader>
    {
    }

    [Serializable]
    [PostProcess(typeof(OitPostProcessRenderer), PostProcessEvent.BeforeTransparent, "OrderIndependentTransparency")]
    public sealed class OitPostProcess : PostProcessEffectSettings
    {
        // can we provide a default value here?
        public ShaderParameter fullscreenShader = new();
        public ComputeShaderParameter resetShader = new();
    }

    public sealed class OitPostProcessRenderer : PostProcessEffectRenderer<OitPostProcess>
    {
        private IOrderIndependentTransparency orderIndependentTransparency;
        private CommandBuffer cmdPreRender;

        public override void Init()
        {
            base.Init();
            orderIndependentTransparency ??= new OitLinkedList(settings.fullscreenShader.value,
                settings.resetShader.value);

            cmdPreRender ??= new CommandBuffer();
            Camera.onPreRender += PreRender;
        }

        private void PreRender(Camera cam)
        {
            if (cmdPreRender == null) return;
            cmdPreRender.Clear();
            orderIndependentTransparency?.PreRender(cmdPreRender);
            Graphics.ExecuteCommandBuffer(cmdPreRender);
        }

        public override void Render(PostProcessRenderContext context)
        {
            orderIndependentTransparency?.Render(context.command, context.source, context.destination);
        }

        public override void Release()
        {
            base.Release();
            orderIndependentTransparency?.Release();
            Camera.onPreRender -= PreRender;
        }
    }
}