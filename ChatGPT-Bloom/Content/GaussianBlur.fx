float2 TexelSize;
float BlurAmount = 1.0f;

texture Texture;
sampler TextureSampler = sampler_state
{
    Texture = <Texture>;
};

static const float weights[5] =
{
    0.227027f,
    0.1945946f,
    0.1216216f,
    0.054054f,
    0.016216f
};

float4 MainPS(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(TextureSampler, texCoord) * weights[0];

    for (int i = 1; i < 5; i++)
    {
        color += tex2D(TextureSampler, texCoord + TexelSize * i * BlurAmount) * weights[i];
        color += tex2D(TextureSampler, texCoord - TexelSize * i * BlurAmount) * weights[i];
    }

    return color;
}

technique GaussianBlur
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 MainPS();
    }
}