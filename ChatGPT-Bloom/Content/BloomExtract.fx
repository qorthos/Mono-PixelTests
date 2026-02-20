float BloomThreshold = 0.2f;
float BloomSoftKnee = 0.5f;

texture Texture;
sampler TextureSampler = sampler_state
{
    Texture = <Texture>;
};

float4 MainPS(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(TextureSampler, texCoord);

    float brightness = max(max(color.r, color.g), color.b);

    float knee = BloomThreshold * BloomSoftKnee;
    float soft = brightness - BloomThreshold + knee;
    soft = saturate(soft / (2 * knee));
    float contribution = max(soft, brightness - BloomThreshold);

    return color * contribution;
}

technique BloomExtract
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 MainPS();
    }
}