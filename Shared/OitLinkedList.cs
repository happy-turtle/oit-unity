using UnityEngine;
using UnityEngine.Rendering;

namespace OrderIndependentTransparency
{
    public class OitLinkedList : IOrderIndependentTransparency
    {
        private int targetWidth, targetHeight;
        private ComputeBuffer fragmentLinkBuffer;
        private readonly int fragmentLinkBufferId;
        private ComputeBuffer startOffsetBuffer;
        private readonly int startOffsetBufferId;
        private readonly Material linkedListMaterial;
        private const int MAX_SORTED_PIXELS = 8;

        private readonly ComputeShader oitComputeUtils;
        private readonly int clearStartOffsetBufferKernel;
        private int dispatchGroupSizeX, dispatchGroupSizeY;

        public OitLinkedList(string shaderName)
        {
            linkedListMaterial = new Material(Resources.Load<Shader>(shaderName));
            fragmentLinkBufferId = Shader.PropertyToID("FLBuffer");
            startOffsetBufferId = Shader.PropertyToID("StartOffsetBuffer");

            oitComputeUtils = Resources.Load<ComputeShader>("OitComputeUtils");
            clearStartOffsetBufferKernel = oitComputeUtils.FindKernel("ClearStartOffsetBuffer");
        }

        public void PreRender(CommandBuffer command, Camera camera)
        {
            // validate the effect itself
            if (camera.scaledPixelWidth != targetWidth || camera.scaledPixelHeight != targetHeight)
            {
                SetupGraphicsBuffers(camera.scaledPixelWidth, camera.scaledPixelHeight);
            }

            //reset StartOffsetBuffer to zeros
            oitComputeUtils.Dispatch(clearStartOffsetBufferKernel, dispatchGroupSizeX, dispatchGroupSizeY, 1);

            // set buffers for rendering
            command.SetRandomWriteTarget(1, fragmentLinkBuffer);
            command.SetRandomWriteTarget(2, startOffsetBuffer);
        }

        public Material Render(CommandBuffer command, RenderTargetIdentifier src, RenderTargetIdentifier dest)
        {
            command.ClearRandomWriteTargets();
            if (linkedListMaterial == null)
            {
                return null;
            }
            // blend linked list
            linkedListMaterial.SetBuffer(fragmentLinkBufferId, fragmentLinkBuffer);
            linkedListMaterial.SetBuffer(startOffsetBufferId, startOffsetBuffer);
            return linkedListMaterial;
        }

        public void Release()
        {
            fragmentLinkBuffer?.Dispose();
            startOffsetBuffer?.Dispose();
        }

        private void SetupGraphicsBuffers(int width, int height)
        {
            Release();
            targetWidth = width;
            targetHeight = height;

            int bufferSize = targetWidth * targetHeight * MAX_SORTED_PIXELS;
            int bufferStride = sizeof(uint) * 3;
            //the structured buffer contains all information about the transparent fragments
            //this is the per pixel linked list on the gpu
            fragmentLinkBuffer = new ComputeBuffer(bufferSize, bufferStride, ComputeBufferType.Counter);

            int bufferSizeHead = targetWidth * targetHeight;
            int bufferStrideHead = sizeof(uint);
            //create buffer for addresses, this is the head of the linked list
            startOffsetBuffer = new ComputeBuffer(bufferSizeHead, bufferStrideHead, ComputeBufferType.Raw);

            oitComputeUtils.SetBuffer(clearStartOffsetBufferKernel, startOffsetBufferId, startOffsetBuffer);
            oitComputeUtils.SetInt("targetWidth", targetWidth);
            dispatchGroupSizeX = Mathf.CeilToInt(targetWidth / 32.0f);
            dispatchGroupSizeY = Mathf.CeilToInt(targetHeight / 32.0f);
        }
    }
}