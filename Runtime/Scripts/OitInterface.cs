using UnityEngine.Rendering;

namespace OrderIndependentTransparency
{
    public enum OitMode
    {
        LinkedList,
    }

    public interface IOrderIndependentTransparency
    {
        void PreRender(CommandBuffer command);
        void Render(CommandBuffer command, RenderTargetIdentifier src, RenderTargetIdentifier dest);
        void Release();
    }
}