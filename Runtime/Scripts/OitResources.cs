using UnityEngine;

namespace OrderIndependentTransparency
{
    public sealed class OitResources : ScriptableObject
    {
        public Shader oitFullscreenRender;
        public ComputeShader oitComputeUtils;
    }
}