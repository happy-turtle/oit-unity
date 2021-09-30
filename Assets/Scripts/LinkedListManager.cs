using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class LinkedListManager : MonoBehaviour
{

    public Shader listCreationShader = null;
    public Shader listRenderingShader = null;

    private Camera m_camera;

    private struct FragmentAndLinkBuffer_STRUCT
    {
        public Vector4 pixelColor;
        public float depth;
        public uint next;
    };

    private FragmentAndLinkBuffer_STRUCT[] m_perPixelLinkedList;

    private ComputeBuffer m_rwStructuredBuffer;
    private ComputeBuffer m_rwStartOffsetBuffer;
    private uint[] m_resetTable;
    private int m_bufferSize;
    private int m_bufferStride;
    private Material m_linkedListMaterial;
    private Material m_depthMaterial;

    // Use this for initialization
    void Awake()
    {
        m_camera = GetComponent<Camera>();

        m_camera.depthTextureMode = DepthTextureMode.Depth;

        //material for blending and rendering at the end
        m_linkedListMaterial = new Material(listRenderingShader);
        m_linkedListMaterial.hideFlags = HideFlags.DontSave;

        //size and stride of the structuredBuffer
        m_bufferSize = Screen.width * Screen.height * 3;
        if (m_bufferSize == 0)
            return;
        m_bufferStride = sizeof(float) * 5 + sizeof(uint);
        //the structured buffer contains all information about the transparent fragments
        //this is the per pixel linked list on the gpu
        m_rwStructuredBuffer = new ComputeBuffer(m_bufferSize, m_bufferStride, ComputeBufferType.Counter);
        //data container for the per pixel linked list
        m_perPixelLinkedList = new FragmentAndLinkBuffer_STRUCT[m_bufferSize];

        //size and stride of the address buffer
        m_bufferSize = Screen.width * Screen.height;
        m_bufferStride = sizeof(uint);

        //create buffer for addresses, this is the head of the linked list
        //the reset table is for clearing the address buffer
        m_rwStartOffsetBuffer = new ComputeBuffer(m_bufferSize, m_bufferStride, ComputeBufferType.Raw);

        //set randomwrite targets
        Graphics.SetRandomWriteTarget(1, m_rwStructuredBuffer, false);
        Graphics.SetRandomWriteTarget(2, m_rwStartOffsetBuffer);
    }

    private void OnDestroy()
    {
        m_rwStructuredBuffer.Dispose();
        m_rwStartOffsetBuffer.Dispose();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //reset StartOffsetBuffer to zeros and reset counter of the StructuredBuffer
        m_rwStructuredBuffer.SetCounterValue(0);

        //render all opaque objects
        m_camera.targetTexture = source;
        m_camera.cullingMask = ~(1 << LayerMask.NameToLayer("Transparent"));
        m_camera.Render();

        //set buffer data in shaders
        m_rwStructuredBuffer.SetData(m_perPixelLinkedList);
        Shader.SetGlobalBuffer("FLBuffer", m_rwStructuredBuffer);
        Shader.SetGlobalBuffer("StartOffsetBuffer", m_rwStartOffsetBuffer);

        //render into listCreationShader, only transparent objects
        //the rwStructuredBuffer is filled with pixel information
        m_camera.cullingMask = 1 << LayerMask.NameToLayer("Transparent");
        m_camera.RenderWithShader(listCreationShader, null);

        //calculate the final pixelColors with the written StructuredBuffer
        //blend opaque and transparent objects together
        Graphics.Blit(source, destination, m_linkedListMaterial);
    }
}
