using UnityEngine;
using UnityEngine.Rendering;

namespace OrderIndependentTransparency
{
    public class OitLinkedList : IOrderIndependentTransparency
    {
        private readonly GraphicsBuffer fragmentLinkBuffer;
        private readonly int fragmentLinkBufferId;
        private readonly GraphicsBuffer startOffsetBuffer;
        private readonly int startOffsetBufferId;
        private readonly Material linkedListMaterial;
        private const int MAX_SORTED_PIXELS = 8;

        private readonly ComputeShader oitComputeUtils;
        private readonly int clearStartOffsetBufferKernel;
        private readonly int dispatchGroupSizeX, dispatchGroupSizeY;

        public OitLinkedList(string shader)
        {
            linkedListMaterial = new Material(Shader.Find(shader));
            int bufferWidth = Screen.width > 0 ? Screen.width : 1024;
            int bufferHeight = Screen.height > 0 ? Screen.height : 1024;

            int bufferSize = bufferWidth * bufferHeight * MAX_SORTED_PIXELS;
            int bufferStride = sizeof(uint) * 3;
            //the structured buffer contains all information about the transparent fragments
            //this is the per pixel linked list on the gpu
            fragmentLinkBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Counter, bufferSize, bufferStride);
            fragmentLinkBufferId = Shader.PropertyToID("FLBuffer");

            int bufferSizeHead = bufferWidth * bufferHeight;
            int bufferStrideHead = sizeof(uint);
            //create buffer for addresses, this is the head of the linked list
            startOffsetBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, bufferSizeHead, bufferStrideHead);
            startOffsetBufferId = Shader.PropertyToID("StartOffsetBuffer");

            oitComputeUtils = Resources.Load<ComputeShader>("OitComputeUtils");
            clearStartOffsetBufferKernel = oitComputeUtils.FindKernel("ClearStartOffsetBuffer");
            oitComputeUtils.SetBuffer(clearStartOffsetBufferKernel, startOffsetBufferId, startOffsetBuffer);
            oitComputeUtils.SetInt("bufferWidth", bufferWidth);
            dispatchGroupSizeX = Mathf.CeilToInt(bufferWidth / 32.0f);
            dispatchGroupSizeY = Mathf.CeilToInt(bufferHeight / 32.0f);
        }

        public void PreRender(CommandBuffer command)
        {
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
    }
}