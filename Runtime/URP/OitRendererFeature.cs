using System;
using UnityEngine.Rendering.Universal;

namespace OrderIndependentTransparency.URP
{
    [Serializable]
    public class OitRendererFeature : ScriptableRendererFeature
    {
        private OitPass oitPass;

        public override void Create()
        {
            oitPass?.Cleanup();
            oitPass = new OitPass();
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