using System;
using UnityEngine.Rendering.Universal;

namespace OrderIndependentTransparency.URP
{
    // Do not forget to add this Renderer Feature in the Universal Render Pipeline Settings.
    [Serializable]
    public class OrderIndependentTransparencyRenderer : ScriptableRendererFeature
    {
        public OitResources shaderResources;
        private OitPass oitPass;

        public override void Create()
        {
            oitPass?.Cleanup();
            oitPass = new OitPass(shaderResources);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(oitPass);
        }

        protected override void Dispose(bool disposing)
        {
            oitPass.Cleanup();
        }
    }
}