using System;
using UnityEngine.Rendering.Universal;

[Serializable]
public class OitRendererURP : ScriptableRendererFeature
{
    private OitPassURP oitPass;

    public override void Create()
    {
        oitPass = new OitPassURP();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(oitPass);
    }
}