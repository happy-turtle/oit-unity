using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace OrderIndependentTransparency.URP
{
    // Do not forget to add this Renderer Feature in the Universal Render Pipeline Settings.
    [Serializable]
    public class OrderIndependentTransparencyRenderer : ScriptableRendererFeature
    {
        private OitPass oitPass;

        public override void Create()
        {
            oitPass?.Cleanup();
            oitPass = new OitPass();
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            // Unity does not provide an example how to perform a fullscreen Blit that works in scene view
            // Hence only Blit in Game view for now
            if (renderingData.cameraData.cameraType != CameraType.Game)
                return;
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