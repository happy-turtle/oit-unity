using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class OitCameraComponent : MonoBehaviour
{
    private IOrderIndependentTransparency orderIndependentTransparency;
    private Camera cam;
    private CommandBuffer command;

    private void OnEnable()
    {
        orderIndependentTransparency = new OitLinkedList();
        cam = GetComponent<Camera>();
        command = new CommandBuffer();
        command.name = "Order-Independent Transparency";
        orderIndependentTransparency.Render(command, BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CurrentActive);
        cam.AddCommandBuffer(CameraEvent.BeforeImageEffects, command);
    }

    private void OnPreRender()
    {
        orderIndependentTransparency.PreRender();
    }

    private void OnDisable()
    {
        orderIndependentTransparency.Release();
        cam.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, command);
        command.Dispose();
    }
}
