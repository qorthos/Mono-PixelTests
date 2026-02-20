float BloomIntensity = 1.2f;
float BaseIntensity = 1.0f;
float BloomSaturation = 1.0f;
float BaseSaturation = 1.0f;

texture Texture : register(t0); // SpriteBatch binds here
texture BloomTexture : register(t1); // We will bind manually

sampler BaseSampler = sampler_state
{
    Texture = <Texture>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

sampler BloomSampler = sampler_state
{
    Texture = <BloomTexture>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinate : TEXCOORD0;
};

float4 AdjustSaturation(float4 color, float saturation)
{
    float grey = dot(color.rgb, float3(0.3, 0.59, 0.11));
    return float4(lerp(grey.xxx, color.rgb, saturation), color.a);
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{
    float2 texCoord = input.TextureCoordinate;

    float4 baseColor = tex2D(BaseSampler, texCoord);
    float4 bloomColor = tex2D(BloomSampler, texCoord);

    baseColor = AdjustSaturation(baseColor, BaseSaturation) * BaseIntensity;
    bloomColor = AdjustSaturation(bloomColor, BloomSaturation) * BloomIntensity;

    return baseColor + bloomColor;
}

technique BloomCombine
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 MainPS();
    }
}