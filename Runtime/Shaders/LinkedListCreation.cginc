#ifndef OIT_LINKED_LIST_INCLUDED
#define OIT_LINKED_LIST_INCLUDED

#include "UnityCG.cginc"
#include "OitUtils.cginc"

struct FragmentAndLinkBuffer_STRUCT
{
    uint pixelColor;
    uint uDepthSampleIdx;
    uint next;
};

RWStructuredBuffer<FragmentAndLinkBuffer_STRUCT> FLBuffer : register(u1);
RWByteAddressBuffer StartOffsetBuffer : register(u2);

void createFragmentEntry(float4 col, float3 pos, uint uSampleIdx) {
    //Retrieve current Pixel count and increase counter
    uint uPixelCount = FLBuffer.IncrementCounter();

    //calculate bufferAddress
    uint uStartOffsetAddress = 4 * (_ScreenParams.x * (pos.y - 0.5) + (pos.x - 0.5));
    uint uOldStartOffset;
    StartOffsetBuffer.InterlockedExchange(uStartOffsetAddress, uPixelCount, uOldStartOffset);

    //add new Fragment Entry in FragmentAndLinkBuffer
    FragmentAndLinkBuffer_STRUCT Element;
    Element.pixelColor = PackRGBA(col);
    Element.uDepthSampleIdx = PackDepthSampleIdx(Linear01Depth(pos.z), uSampleIdx);
    Element.next = uOldStartOffset;
    FLBuffer[uPixelCount] = Element;
}

#endif // OIT_LINKED_LIST_INCLUDED