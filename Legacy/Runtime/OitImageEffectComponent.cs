using UnityEngine;
using UnityEngine.Rendering;

namespace OrderIndependentTransparency
{
    [ImageEffectAllowedInSceneView]
    [ExecuteAlways]
    public class OitImageEffectComponent : MonoBehaviour
    {
        public Shader fullscreenShader;
        public ComputeShader resetShader;
        private CommandBuffer cmdPreRender;
        private CommandBuffer cmdRender;

        private IOrderIndependentTransparency orderIndependentTransparency;

        private void Start()
        {
            orderIndependentTransparency ??= new OitLinkedList(fullscreenShader, resetShader);
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
            // I don't see a possibility here to not write to dest. Unity still warns me this wouldn't do so.
            if (orderIndependentTransparency != null)
            {
                orderIndependentTransparency.Render(cmdRender, src,
                    dest);
            }
            else
            {
                cmdRender.ClearRandomWriteTargets();
                cmdRender.Blit(src, dest);
            }

            Graphics.ExecuteCommandBuffer(cmdRender);
        }
    }
}