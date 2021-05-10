#ifndef _HOLISTICBARRACUDA_REGION_HLSL_
#define _HOLISTICBARRACUDA_REGION_HLSL_

//
// Pose region tracking structure
//
// size = 24 * 4 byte
//
struct Region
{
    float4 box; // center_x, center_y, size, angle
    float4 dBox;
    float4x4 cropMatrix;
};

#endif
