#ifndef OIT_LINKED_LIST_INCLUDED
#define OIT_LINKED_LIST_INCLUDED

#include "OitUtils.cginc"

struct FragmentAndLinkBuffer_STRUCT
{
    uint pixelColor;
    uint uDepthSampleIdx;
    uint next;
};

StructuredBuffer<FragmentAndLinkBuffer_STRUCT> FLBuffer : register(t0);
ByteAddressBuffer StartOffsetBuffer : register(t1);

float4 renderLinkedList(float4 col, float2 pos, uint uSampleIndex)
{
    // Fetch offset of first fragment for current pixel
    uint uStartOffsetAddress = 4 * (_ScreenParams.x * (pos.y - 0.5) + (pos.x - 0.5));
    uint uOffset = StartOffsetBuffer.Load(uStartOffsetAddress);

    FragmentAndLinkBuffer_STRUCT SortedPixels[MAX_SORTED_PIXELS];

    // Parse linked list for all pixels at this position
    // and store them into temp array for later sorting
    int nNumPixels = 0;
    while (uOffset != 0)
    {
        // Retrieve pixel at current offset
        FragmentAndLinkBuffer_STRUCT Element = FLBuffer[uOffset];
        uint uSampleIdx = UnpackSampleIdx(Element.uDepthSampleIdx);
        if (uSampleIdx == uSampleIndex)
        {
            SortedPixels[nNumPixels] = Element;
            nNumPixels += 1;
        }

        uOffset = (nNumPixels >= MAX_SORTED_PIXELS) ? 0 : FLBuffer[uOffset].next;
    }

    // Sort pixels in depth
    for (int i = 0; i < nNumPixels - 1; i++)
    {
        for (int j = i + 1; j > 0; j--)
        {
            float depth = UnpackDepth(SortedPixels[j].uDepthSampleIdx);
            float previousElementDepth = UnpackDepth(SortedPixels[j - 1].uDepthSampleIdx);
            if (previousElementDepth < depth)
            {
                FragmentAndLinkBuffer_STRUCT temp = SortedPixels[j - 1];
                SortedPixels[j - 1] = SortedPixels[j];
                SortedPixels[j] = temp;
            }
        }
    }

    // Rendering pixels
    for (int k = 0; k < nNumPixels; k++)
    {
        // Retrieve next unblended furthermost pixel
        float4 vPixColor = UnpackRGBA(SortedPixels[k].pixelColor);

        // Manual blending between current fragment and previous one
        col.rgb = lerp(col.rgb, vPixColor.rgb, vPixColor.a);
    }

    return col;
}
#endif // OIT_LINKED_LIST_INCLUDED