using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[Serializable]
public class OitRendererURP : ScriptableRendererFeature
{
    private OitPassURP oitPass;

    public override void Create()
    {
        oitPass?.Cleanup();
        oitPass = new OitPassURP();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(oitPass);
    }

    protected override void Dispose(bool disposing)
    {
        oitPass.Cleanup();
    }
}