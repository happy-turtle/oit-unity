using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class OitCameraComponent : MonoBehaviour
{
    [Tooltip("This can be increased if objects disappear or block artifacts appear. A lower value keeps the used video memory at a minimum.")]
    [Range(1f, 24f)]
    public int listSizeMultiplier = 5;

    [Tooltip("Use Multi-Layer Alpha Blending if your graphics target supports shader model 5.1 and the Rasterizer Order Views (ROV) feature." +
                "For legacy shader model 5.0 support use the linked list mode.")]
    public OitMode oitMode = OitMode.MLAB;

    private IOrderIndependentTransparency orderIndependentTransparency;

    private void OnEnable()
    {
        if (oitMode == OitMode.MLAB)
        {
            orderIndependentTransparency = new OitMultiLayerAlphaBlending(listSizeMultiplier);
        }
        else
        {
            orderIndependentTransparency = new OitLinkedList(listSizeMultiplier);
        }
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
