using UnityEngine;
using UnityEngine.Rendering;

namespace OrderIndependentTransparency
{
    [ImageEffectAllowedInSceneView]
    public class OitImageEffectComponent : MonoBehaviour
    {
        public OitResources shaderResources;
        private CommandBuffer cmdPreRender;
        private CommandBuffer cmdRender;

        private IOrderIndependentTransparency orderIndependentTransparency;

        private void OnEnable()
        {
            orderIndependentTransparency =
                new OitLinkedList(shaderResources.oitFullscreenRender, shaderResources.oitComputeUtils);
            cmdRender = new CommandBuffer();
            cmdPreRender = new CommandBuffer();
        }

        private void OnPreRender()
        {
            cmdPreRender.Clear();
            orderIndependentTransparency.PreRender(cmdPreRender);
            Graphics.ExecuteCommandBuffer(cmdPreRender);
        }

        [ImageEffectUsesCommandBuffer]
        [ImageEffectOpaque]
        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            cmdRender.Clear();
            orderIndependentTransparency.Render(cmdRender, src,
                dest);
            Graphics.ExecuteCommandBuffer(cmdRender);
        }

        private void OnDisable()
        {
            orderIndependentTransparency.Release();
        }
    }
}