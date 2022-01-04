#ifndef OIT_UTILS_INCLUDED
#define OIT_UTILS_INCLUDED

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
	uint  packedOutput = (u.w << 24UL) | (u.z << 16UL) | (u.y << 8UL) | u.x;
	return packedOutput;
}

#endif // OIT_UTILS_INCLUDED