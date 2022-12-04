using UnityEngine;
using UnityEngine.Rendering;

namespace OrderIndependentTransparency
{
    public class OitLinkedList : IOrderIndependentTransparency
    {
        private int screenWidth, screenHeight;
        private GraphicsBuffer fragmentLinkBuffer;
        private readonly int fragmentLinkBufferId;
        private GraphicsBuffer startOffsetBuffer;
        private readonly int startOffsetBufferId;
        private readonly Material linkedListMaterial;
        private const int MAX_SORTED_PIXELS = 8;

        private readonly ComputeShader oitComputeUtils;
        private readonly int clearStartOffsetBufferKernel;
        private int dispatchGroupSizeX, dispatchGroupSizeY;

        public OitLinkedList(string fullscreenShader)
        {
            linkedListMaterial = new Material(Resources.Load<Shader>(fullscreenShader));
            fragmentLinkBufferId = Shader.PropertyToID("FLBuffer");
            startOffsetBufferId = Shader.PropertyToID("StartOffsetBuffer");

            oitComputeUtils = Resources.Load<ComputeShader>("OitComputeUtils");
            clearStartOffsetBufferKernel = oitComputeUtils.FindKernel("ClearStartOffsetBuffer");
            SetupGraphicsBuffers();
        }

        public void PreRender(CommandBuffer command)
        {
            // validate the effect itself
            if (Screen.width != screenWidth || Screen.height != screenHeight)
            {
                SetupGraphicsBuffers();
            }

            //reset StartOffsetBuffer to zeros
            oitComputeUtils.Dispatch(clearStartOffsetBufferKernel, dispatchGroupSizeX, dispatchGroupSizeY, 1);

            // set buffers for rendering
            command.SetRandomWriteTarget(1, fragmentLinkBuffer);
            command.SetRandomWriteTarget(2, startOffsetBuffer);
        }

        public void Render(CommandBuffer command, RenderTargetIdentifier src, RenderTargetIdentifier dest)
        {
            command.ClearRandomWriteTargets();
            // blend linked list
            linkedListMaterial.SetBuffer(fragmentLinkBufferId, fragmentLinkBuffer);
            linkedListMaterial.SetBuffer(startOffsetBufferId, startOffsetBuffer);
            command.Blit(src, dest, linkedListMaterial);
        }

        public void Release()
        {
            fragmentLinkBuffer?.Dispose();
            startOffsetBuffer?.Dispose();
        }

        private void SetupGraphicsBuffers()
        {
            Release();
            screenWidth = Screen.width;
            screenHeight = Screen.height;

            int bufferSize = screenWidth * screenHeight * MAX_SORTED_PIXELS;
            int bufferStride = sizeof(uint) * 3;
            //the structured buffer contains all information about the transparent fragments
            //this is the per pixel linked list on the gpu
            fragmentLinkBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Counter, bufferSize, bufferStride);

            int bufferSizeHead = screenWidth * screenHeight;
            int bufferStrideHead = sizeof(uint);
            //create buffer for addresses, this is the head of the linked list
            startOffsetBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, bufferSizeHead, bufferStrideHead);

            oitComputeUtils.SetBuffer(clearStartOffsetBufferKernel, startOffsetBufferId, startOffsetBuffer);
            oitComputeUtils.SetInt("screenWidth", screenWidth);
            dispatchGroupSizeX = Mathf.CeilToInt(screenWidth / 32.0f);
            dispatchGroupSizeY = Mathf.CeilToInt(screenHeight / 32.0f);
        }
    }
}