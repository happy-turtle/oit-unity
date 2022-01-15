using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif

public class OitMultiLayerAlphaBlending : IOrderIndependentTransparency
{
    private ComputeBuffer fragmentBuffer;
    private RenderTexture clearMask;
    private int fragmentBufferId;
    private int clearMaskId;
    private int bufferSize;
    private int bufferStride;
    private Material mlabMaterial;
    private const int MAX_SORTED_PIXELS = 8;

    public OitMultiLayerAlphaBlending(bool postProcess = false)
    {
        mlabMaterial = new Material(Shader.Find("Hidden/MLABRendering"));
        mlabMaterial.EnableKeyword(postProcess ? "POST_PROCESSING" : "BUILT_IN");
        Shader.DisableKeyword("LINKED_LIST");
        Shader.EnableKeyword("MLAB");
        int bufferWidth = Screen.width > 0 ? Screen.width : 1024;
        int bufferHeight = Screen.height > 0 ? Screen.height : 1024;

        int bufferSize = bufferWidth * bufferHeight * MAX_SORTED_PIXELS;
        int bufferStride = sizeof(uint) * 2;
        //the structured buffer contains all information about the transparent fragments
        //this is the per pixel linked list on the gpu
        fragmentBuffer = new ComputeBuffer(bufferSize, bufferStride);
        fragmentBufferId = Shader.PropertyToID("FragmentBuffer");

        clearMask = new RenderTexture(bufferWidth, bufferHeight, 0, UnityEngine.Experimental.Rendering.GraphicsFormat.R32_UInt, 0);
        clearMask.enableRandomWrite = true;
        clearMaskId = Shader.PropertyToID("ClearMask");
    }

    public void PreRender()
    {
        if (fragmentBuffer == null)
            return;

        // set buffers for rendering
        Graphics.SetRandomWriteTarget(1, fragmentBuffer);
        Graphics.SetRandomWriteTarget(2, clearMask);
    }

    public void Render(RenderTexture source, RenderTexture destination)
    {
        if (fragmentBuffer == null || mlabMaterial == null)
            return;

        Graphics.ClearRandomWriteTargets();
        // blend linked list
        mlabMaterial.SetBuffer(fragmentBufferId, fragmentBuffer);
        Graphics.SetRandomWriteTarget(2, clearMask);
        Graphics.Blit(source, destination, mlabMaterial);
        Graphics.ClearRandomWriteTargets();
    }

#if UNITY_POST_PROCESSING_STACK_V2
    public void Render(PostProcessRenderContext context)
    {
        if (fragmentBuffer == null || mlabMaterial == null)
            return;

        context.command.ClearRandomWriteTargets();
        // blend linked list
        mlabMaterial.SetBuffer(fragmentBufferId, fragmentBuffer);
        context.command.SetRandomWriteTarget(2, clearMask);
        context.command.Blit(context.source, context.destination, mlabMaterial);
        context.command.ClearRandomWriteTargets();
    }
#endif

    public void Release()
    {
        fragmentBuffer?.Release();
        clearMask?.Release();
    }
}