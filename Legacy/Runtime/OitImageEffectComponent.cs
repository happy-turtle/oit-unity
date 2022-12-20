using UnityEngine;
using UnityEngine.Rendering;

namespace OrderIndependentTransparency
{
    [ImageEffectAllowedInSceneView]
    [ExecuteAlways]
    internal class OitImageEffectComponent : MonoBehaviour
    {
        private CommandBuffer cmdPreRender;
        private CommandBuffer cmdRender;

        private IOrderIndependentTransparency orderIndependentTransparency;

        private void Start()
        {
            orderIndependentTransparency ??= new OitLinkedList();
        }

        private void OnDisable()
        {
            orderIndependentTransparency?.Release();
        }

        private void OnDestroy()
        {
            orderIndependentTransparency?.Release();
        }

        private void OnPreRender()
        {
            cmdPreRender ??= new CommandBuffer();
            cmdPreRender.Clear();
            orderIndependentTransparency?.PreRender(cmdPreRender);
            Graphics.ExecuteCommandBuffer(cmdPreRender);
        }

        [ImageEffectUsesCommandBuffer]
        [ImageEffectOpaque]
        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            cmdRender ??= new CommandBuffer();
            cmdRender.Clear();
            orderIndependentTransparency?.Render(cmdRender, src, src);
            Graphics.ExecuteCommandBuffer(cmdRender);
            // Additional Blit to ensure we write to destination
            Graphics.Blit(src, dest);
        }
    }
}