using UnityEngine.Rendering.Universal;

namespace OrderIndependentTransparency.URP
{
    // Do not forget to add this Renderer Feature in the Universal Render Pipeline Settings.
    internal class OrderIndependentTransparencyRenderer : ScriptableRendererFeature
    {
        private OitPass oitPass;

        public override void Create()
        {
            oitPass?.Cleanup();
            oitPass = new OitPass();
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.isPreviewCamera)
            {
                return;
            }
            //Calling ConfigureInput with the ScriptableRenderPassInput.Color argument ensures that the opaque texture is available to the Render Pass
            oitPass.ConfigureInput(ScriptableRenderPassInput.Color);
            renderer.EnqueuePass(oitPass);
        }

        protected override void Dispose(bool disposing)
        {
            oitPass.Cleanup();
        }
    }
}