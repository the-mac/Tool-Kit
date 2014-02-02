//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved
////----------------------------------------------------------------------

#define HistorySize 100 // Matches ProfilerGraph::HistorySize

cbuffer cb0
{
    uniform float4 frames[HistorySize];
}

cbuffer cb1
{
    uniform float4 scale;
    uniform float4 offset;
    uniform float4 frameOffset;
};

struct VS_INPUT
{
    float2 Pos : POSITION;
};

struct PS_INPUT
{
    float4 Pos : SV_POSITION;
    float4 Sample : TEXCOORD0;
    float Y : TEXCOORD1;
};

//--------------------------------------------------------------------------------------
// Vertex Shader
//--------------------------------------------------------------------------------------

PS_INPUT main(VS_INPUT input)
{
    PS_INPUT output;

    output.Pos = float4(input.Pos.x, input.Pos.y, 0.1, 1.0);
    output.Pos = mad(output.Pos, scale, offset);

    float index = ((input.Pos.x + frameOffset.x) * (HistorySize - 1) + 1) % HistorySize;
    float4 frame = frames[index];

    // Accumulate profiled durations.
    frame.y += frame.x;
    frame.z += frame.y;
    frame.w += frame.z;

    output.Sample = frame;
    output.Y = input.Pos.y;

    return output;
}
