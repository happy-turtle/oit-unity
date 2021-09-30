using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class LinkedListManager : MonoBehaviour
{

    public Shader listCreationShader = null;
    public Shader listRenderingShader = null;

    private Camera m_camera;

    private RenderTexture m_opaqueTex = null;
    private RenderTexture m_depthTex = null;

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

        if (m_rwStructuredBuffer != null)
            m_rwStructuredBuffer.Dispose();
        if (m_rwStartOffsetBuffer != null)
            m_rwStartOffsetBuffer.Dispose();

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
        m_resetTable = new uint[m_bufferSize];
        foreach (int i in m_resetTable)
        {
            m_resetTable[i] = 0xFFFFFFFF;
        }

        //set randomwrite targets
        Graphics.SetRandomWriteTarget(1, m_rwStructuredBuffer, false);
        Graphics.SetRandomWriteTarget(2, m_rwStartOffsetBuffer);
    }

    private void OnDestroy()
    {
        if (m_rwStructuredBuffer != null)
            m_rwStructuredBuffer.Dispose();
        if (m_rwStartOffsetBuffer != null)
            m_rwStartOffsetBuffer.Dispose();
        Graphics.ClearRandomWriteTargets();
    }

    private void OnPreRender()
    {
        //deactivate automatic Rendering, everything is rendered manually
        m_camera.cullingMask = 0;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //reset StartOffsetBuffer to zeros and reset counter of the StructuredBuffer
        m_rwStartOffsetBuffer.SetData(m_resetTable);
        m_rwStructuredBuffer.SetCounterValue(0);

        //Render Texture for all opaque objects
        m_opaqueTex = RenderTexture.GetTemporary(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);

        //render all opaque objects
        m_camera.SetTargetBuffers(m_opaqueTex.colorBuffer, m_opaqueTex.depthBuffer);
        m_camera.backgroundColor = m_camera.backgroundColor;
        m_camera.clearFlags = m_camera.clearFlags;
        m_camera.cullingMask = ~(1 << LayerMask.NameToLayer("Transparent"));
        m_camera.Render();
        //set opaque texture for final blending
        m_linkedListMaterial.SetTexture("_BackgroundTex", m_opaqueTex);

        //set buffer data in shaders
        m_rwStructuredBuffer.SetData(m_perPixelLinkedList);
        Shader.SetGlobalBuffer("FLBuffer", m_rwStructuredBuffer);
        Shader.SetGlobalBuffer("StartOffsetBuffer", m_rwStartOffsetBuffer);

        //render into listCreationShader, only transparent objects
        //the rwStructuredBuffer is filled with pixel information
        m_camera.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        m_camera.SetTargetBuffers(m_opaqueTex.colorBuffer, m_opaqueTex.depthBuffer);
        m_camera.clearFlags = CameraClearFlags.Nothing;
        m_camera.cullingMask = 1 << LayerMask.NameToLayer("Transparent");
        m_camera.RenderWithShader(listCreationShader, null);

        //this was for debugging
        //Graphics.Blit(m_accumulateTex, destination);

        //calculate the final pixelColors with the written StructuredBuffer
        //blend opaque and transparent objects together
        Graphics.Blit(source, destination, m_linkedListMaterial);

        //release render textures
        RenderTexture.ReleaseTemporary(m_opaqueTex);
        RenderTexture.ReleaseTemporary(m_depthTex);
    }
}
