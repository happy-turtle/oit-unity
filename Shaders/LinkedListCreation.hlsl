#ifndef OIT_LINKED_LIST_INCLUDED
#define OIT_LINKED_LIST_INCLUDED

#include "LinkedListStruct.hlsl"

RWStructuredBuffer<FragmentAndLinkBuffer_STRUCT> FLBuffer : register(u1);
RWByteAddressBuffer StartOffsetBuffer : register(u2);

// PackRGBA takes a float4 value and packs it into a UINT (8 bits / float)
uint PackRGBA(float4 unpackedInput)
{
	uint4 u = (uint4)(saturate(unpackedInput) * 255 + 0.5);
	uint packedOutput = (u.w << 24UL) | (u.z << 16UL) | (u.y << 8UL) | u.x;
	return packedOutput;
}

uint PackDepthSampleIdx(float depth, uint uSampleIdx) {
	uint d = (uint)(saturate(depth) * (pow(2, 24) - 1));
	return d << 8UL | uSampleIdx;
}

// Z buffer to linear 0..1 depth
inline float OitLinear01Depth( float z )
{
	return 1.0 / (_ZBufferParams.x * z + _ZBufferParams.y);
}

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
    Element.uDepthSampleIdx = PackDepthSampleIdx(OitLinear01Depth(pos.z), uSampleIdx);
    Element.next = uOldStartOffset;
    FLBuffer[uPixelCount] = Element;
}

#endif // OIT_LINKED_LIST_INCLUDED