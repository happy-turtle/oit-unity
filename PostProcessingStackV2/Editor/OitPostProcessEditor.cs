using UnityEditor;
using UnityEditor.Rendering.PostProcessing;
using UnityEngine;

namespace OrderIndependentTransparency.PostProcessingStackV2
{
    [PostProcessEditor(typeof(OitPostProcess))]
    public class OitPostProcessEditor : PostProcessEffectEditor<OitPostProcess>
    {
        SerializedParameterOverride fullscreenShader;
        SerializedParameterOverride resetShader;
        
        public override void OnEnable()
        {
            fullscreenShader = FindParameterOverride(x => x.fullscreenShader);
            resetShader = FindParameterOverride(x => x.resetShader);
            fullscreenShader.value.objectReferenceValue = AssetDatabase.LoadAssetAtPath<Shader>("Packages/org.happy-turtle.order-independent-transparency/Shaders/OitFullscreenRender.shader");
            resetShader.value.objectReferenceValue = AssetDatabase.LoadAssetAtPath<ComputeShader>("Packages/org.happy-turtle.order-independent-transparency/Shaders/OitComputeUtils.compute");
        }

        public override void OnInspectorGUI()
        {
            PropertyField(fullscreenShader);
            PropertyField(resetShader);
        }
    }
}