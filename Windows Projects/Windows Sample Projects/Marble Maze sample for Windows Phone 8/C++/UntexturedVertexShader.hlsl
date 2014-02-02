struct VertexShaderInput
{
    float3 pos : POSITION;
    float3 color : COLOR0;
};

struct VertexShaderOutput
{
    float4 pos : SV_POSITION;
    float3 color : COLOR0;
};

VertexShaderOutput main(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.pos = float4(input.pos, 1.0f);

    // Pass through the color without modification.
    output.color = input.color;

    return output;
}
