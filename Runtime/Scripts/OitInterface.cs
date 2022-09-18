using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif

public enum OitMode
{
    LinkedList,
}

public interface IOrderIndependentTransparency
{
    void PreRender();
    void Render(RenderTexture source, RenderTexture destination);
#if UNITY_POST_PROCESSING_STACK_V2
    void Render(PostProcessRenderContext context);
#endif
    void Release();
}