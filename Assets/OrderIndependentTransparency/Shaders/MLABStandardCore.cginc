// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

#ifndef OIT_STANDARD_CORE_FORWARD_INCLUDED
#define OIT_STANDARD_CORE_FORWARD_INCLUDED

#if defined(UNITY_NO_FULL_STANDARD_SHADER)
#   define UNITY_STANDARD_SIMPLE 1
#endif

#include "UnityStandardConfig.cginc"

#if UNITY_STANDARD_SIMPLE
    #include "UnityStandardCoreForwardSimple.cginc"
    #include "MLABCreation.cginc"
    VertexOutputBaseSimple vertBase (VertexInput v) { return vertForwardBaseSimple(v); }
    VertexOutputForwardAddSimple vertAdd (VertexInput v) { return vertForwardAddSimple(v); }
    [earlydepthstencil]
    half4 fragBase (VertexOutputBaseSimple i, uint uCoverage : SV_COVERAGE) : SV_Target 
    { 
        float4 col = fragForwardBaseSimpleInternal(i); 
        createFragmentEntry(col, i.pos.xyz, uCoverage);
        return col;
    }
    [earlydepthstencil]
    half4 fragAdd (VertexOutputForwardAddSimple i, uint uCoverage : SV_COVERAGE) : SV_Target 
    {
        col = fragForwardAddSimpleInternal(i); 
        createFragmentEntry(col, i.pos.xyz, uCoverage);
        return col;
    }
#else
    #include "UnityStandardCore.cginc"
    #include "MLABCreation.cginc"
    VertexOutputForwardBase vertBase (VertexInput v) { return vertForwardBase(v); }
    VertexOutputForwardAdd vertAdd (VertexInput v) { return vertForwardAdd(v); }
    [earlydepthstencil]
    half4 fragBase (VertexOutputForwardBase i, uint uCoverage : SV_COVERAGE) : SV_Target 
    { 
        float4 col = fragForwardBaseInternal(i); 
        createFragmentEntry(col, i.pos.xyz, uCoverage);
        return col;
    }
    [earlydepthstencil]
    half4 fragAdd (VertexOutputForwardAdd i, uint uCoverage : SV_COVERAGE) : SV_Target 
    { 
        float4 col = fragForwardAddInternal(i); 
        createFragmentEntry(col, i.pos.xyz, uCoverage);
        return col;
    }
#endif

#endif // OIT_STANDARD_CORE_FORWARD_INCLUDED
