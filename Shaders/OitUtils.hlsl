#ifndef OIT_UTILS_INCLUDED
#define OIT_UTILS_INCLUDED

// Unity's HLSL seems not to support dynamic array size, so we can only set this before compilation
#define MAX_SORTED_PIXELS 8

//https://github.com/GameTechDev/AOIT-Update/blob/master/OIT_DX11/AOIT%20Technique/AOIT.hlsl
// UnpackRGBA takes a uint value and converts it to a float4
float4 UnpackRGBA(uint packedInput)
{
	float4 unpackedOutput;
	uint4 p = uint4((packedInput & 0xFFUL),
		(packedInput >> 8UL) & 0xFFUL,
		(packedInput >> 16UL) & 0xFFUL,
		(packedInput >> 24UL));

	unpackedOutput = ((float4)p) / 255;
	return unpackedOutput;
}

// PackRGBA takes a float4 value and packs it into a UINT (8 bits / float)
uint PackRGBA(float4 unpackedInput)
{
	uint4 u = (uint4)(saturate(unpackedInput) * 255 + 0.5);
	uint packedOutput = (u.w << 24UL) | (u.z << 16UL) | (u.y << 8UL) | u.x;
	return packedOutput;
}

float UnpackDepth(uint uDepthSampleIdx) {
	return (float)(uDepthSampleIdx >> 8UL) / (pow(2, 24) - 1);
}

uint UnpackSampleIdx(uint uDepthSampleIdx) {
	return uDepthSampleIdx & 0xFFUL;
}

uint PackDepthSampleIdx(float depth, uint uSampleIdx) {
	uint d = (uint)(saturate(depth) * (pow(2, 24) - 1));
	return d << 8UL | uSampleIdx;
}

#endif // OIT_UTILS_INCLUDED