using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif

public class OitMultiLayerAlphaBlending : IOrderIndependentTransparency
{
    private GraphicsBuffer fragmentBuffer;
    private int fragmentBufferId;
    private int startOffsetBufferId;
    private int bufferSize;
    private int bufferStride;
    private Material mlabMaterial;

    public OitMultiLayerAlphaBlending(int listSizeMultiplier, bool postProcess = false)
    {
        mlabMaterial = new Material(Shader.Find("Hidden/MLABRendering"));
        if (postProcess)
        {
            mlabMaterial.EnableKeyword("POST_PROCESSING");
        }
        int bufferWidth = Screen.width > 0 ? Screen.width : 1024;
        int bufferHeight = Screen.height > 0 ? Screen.height : 1024;

        int bufferSize = bufferWidth * bufferHeight * listSizeMultiplier;
        int bufferStride = sizeof(uint) * 2;
        //the structured buffer contains all information about the transparent fragments
        //this is the per pixel linked list on the gpu
        fragmentBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Counter, bufferSize, bufferStride);
        fragmentBufferId = Shader.PropertyToID("FragmentBuffer");
    }

    public void PreRender()
    {
        if (fragmentBuffer == null)
            return;

        // set buffers for rendering
        Graphics.SetRandomWriteTarget(1, fragmentBuffer);
    }

    public void Render(RenderTexture source, RenderTexture destination)
    {
        if (fragmentBuffer == null || mlabMaterial == null)
            return;

        Graphics.ClearRandomWriteTargets();
        // blend linked list
        mlabMaterial.SetBuffer(fragmentBufferId, fragmentBuffer);
        Graphics.Blit(source, destination, mlabMaterial);
    }

#if UNITY_POST_PROCESSING_STACK_V2
    public void Render(PostProcessRenderContext context)
    {
        if (fragmentBuffer == null || mlabMaterial == null)
            return;

        context.command.ClearRandomWriteTargets();
        // blend linked list
        mlabMaterial.SetBuffer(fragmentBufferId, fragmentBuffer);
        context.command.Blit(context.source, context.destination, mlabMaterial);
    }
#endif

    public void Release()
    {
        fragmentBuffer?.Dispose();
    }
}