using UnityEngine;
using UnityEngine.Rendering;

namespace OrderIndependentTransparency
{
    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    public class OitCameraComponent : MonoBehaviour
    {
        private IOrderIndependentTransparency orderIndependentTransparency;
        private Camera cam;
        private CommandBuffer cmdRender;
        private CommandBuffer cmdPreRender;

        private void OnEnable()
        {
            orderIndependentTransparency = new OitLinkedList("Hidden/OitFullscreenRender");
            cam = GetComponent<Camera>();
            cmdRender = new CommandBuffer();
            cmdPreRender = new CommandBuffer();
            cmdRender.name = "Order-Independent Transparency";
            cmdPreRender.name = "PreRender Order-Independent Transparency";
            orderIndependentTransparency.Render(cmdRender, BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget);
            cam.AddCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, cmdRender);
        }

        private void OnPreRender()
        {
            cmdPreRender.Clear();
            orderIndependentTransparency.PreRender(cmdPreRender);
            Graphics.ExecuteCommandBuffer(cmdPreRender);
        }

        private void OnDisable()
        {
            orderIndependentTransparency.Release();
            cam.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, cmdRender);
        }
    }
}