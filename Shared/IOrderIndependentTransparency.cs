using UnityEngine;
using UnityEngine.Rendering;

namespace OrderIndependentTransparency
{
    public interface IOrderIndependentTransparency
    {
        void PreRender(CommandBuffer command, Camera camera);
        Material Render(CommandBuffer command, RenderTargetIdentifier src, RenderTargetIdentifier dest);
        void Release();
    }
}