//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved
////----------------------------------------------------------------------

Texture2D tex2D;

cbuffer cb0
{
    uniform float4 colors[4];
}

struct PS_INPUT
{
    float4 Pos : SV_POSITION;
    float4 Sample : TEXCOORD0;
    float Y : TEXCOORD1;
};


//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------
float4 main(PS_INPUT input) : SV_Target
{   
    float4 sample = input.Sample > input.Y;
    float4 enable = ceil(sample);
    sample *= enable;

    // Only allow one color set, precedence is r > g > b > a.
    sample.gba -= sample.r;
    sample.ba -= sample.g;
    sample.a -= sample.b;

    sample = saturate(sample);

    if (input.Y >= 0.5 && input.Y <= 0.52)
    {
        return float4(1.0, 1.0, 1.0, 1.0);
    }

    // Map to colors.
    return (
        colors[0] * sample.r +
        colors[1] * sample.g +
        colors[2] * sample.b +
        colors[3] * sample.a
        );
}
