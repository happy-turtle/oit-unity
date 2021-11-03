#ifndef OIT_UTILS_INCLUDED
#define OIT_UTILS_INCLUDED

struct FragmentAndLinkBuffer_STRUCT
{
    float4 pixelColor;
    float depth;
    uint next;
    uint coverage;
};

RWStructuredBuffer<FragmentAndLinkBuffer_STRUCT> FLBuffer : register(u1);
RWByteAddressBuffer StartOffsetBuffer : register(u2);

void createLinkedListEntry(float4 col, float3 pos, float2 screenParams, uint uCoverage) {
    //Retrieve current Pixel count and increase counter
    uint uPixelCount = FLBuffer.IncrementCounter();

    //calculate bufferAddress
    uint uStartOffsetAddress = 4 * ((screenParams.x * (pos.y - 0.5)) + (pos.x - 0.5));
    uint uOldStartOffset;
    StartOffsetBuffer.InterlockedExchange(uStartOffsetAddress, uPixelCount, uOldStartOffset);

    //add new Fragment Entry in FragmentAndLinkBuffer
    FragmentAndLinkBuffer_STRUCT Element;
    Element.pixelColor = col;
    Element.depth = Linear01Depth(pos.z);
    Element.next = uOldStartOffset;
    Element.coverage = uCoverage;
    FLBuffer[uPixelCount] = Element;
}

#endif // OIT_UTILS_INCLUDED