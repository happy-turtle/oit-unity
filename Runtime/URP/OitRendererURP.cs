using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[Serializable]
public class OitRendererURP : ScriptableRendererFeature
{
    private OitPassURP oitPass;

    public override void Create()
    {
        Debug.Log("construcor");
        oitPass = new OitPassURP();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(oitPass);
    }

    protected override void Dispose(bool disposing)
    {
        Debug.Log("Dispose");
        oitPass.Cleanup();
    }
}