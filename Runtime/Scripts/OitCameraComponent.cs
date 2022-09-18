using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class OitCameraComponent : MonoBehaviour
{
    private IOrderIndependentTransparency orderIndependentTransparency;

    private void OnEnable()
    {
        orderIndependentTransparency = new OitLinkedList();
    }

    private void OnPreRender()
    {
        orderIndependentTransparency.PreRender();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        orderIndependentTransparency.Render(source, destination);
    }

    private void OnDisable()
    {
        orderIndependentTransparency.Release();
    }
}
