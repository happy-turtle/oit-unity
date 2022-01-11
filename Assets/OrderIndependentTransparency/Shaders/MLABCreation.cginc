#ifndef OIT_MLAB_INCLUDED
#define OIT_MLAB_INCLUDED

#include "OitUtils.cginc"
#define MAX_SORTED_PIXELS 8

struct FragmentBuffer_STRUCT
{
    uint pixelColor;
    uint uDepthCoverage;
};

RasterizerOrderedStructuredBuffer<FragmentBuffer_STRUCT> FragmentBuffer : register(u1);
RasterizerOrderedByteAddressBuffer ClearBuffer : register(u2);

void createLinkedListEntry(float4 col, float3 pos, float2 screenParams, uint uCoverage) {
    //Retrieve current Pixel count and increase counter
    uint uPixelCount = FragmentBuffer.IncrementCounter();

    //calculate bufferAddress
    uint uStartOffsetAddress = 4 * ((screenParams.x * (pos.y - 0.5)) + (pos.x - 0.5));

    //create new Fragment
    FragmentBuffer_STRUCT Element;
    Element.pixelColor = PackRGBA(col);
    Element.uDepthCoverage = PackDepthCoverage(Linear01Depth(pos.z), uCoverage);

    uint clearFlag;
    ClearBuffer.InterlockedCompareExchange(uStartOffsetAddress, 0, 1, clearFlag);
    if (clearFlag == 0) {
        FragmentBuffer[uStartOffsetAddress] = Element;
    } else {
        //Sort pixels in depth
        //with insertion sort
        FragmentBuffer_STRUCT temp, merge;
        for (int i = 0; i < MAX_SORTED_PIXELS; i++)
        {
            if (Linear01Depth(pos.z) > UnpackDepth(FragmentBuffer[uStartOffsetAddress + i].uDepthCoverage))
            {
                FragmentBuffer_STRUCT temp = FragmentBuffer[uStartOffsetAddress + i];
                FragmentBuffer[uStartOffsetAddress + i] = Element;
                Element = temp;
            }
        }
    //TODO: merge
    }

}

#endif // OIT_MLAB_INCLUDED