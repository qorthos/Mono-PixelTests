// SuperSampling.fx
// Simple super sampling pixel art shader for smooth scaling/positioning

sampler2D TextureSampler : register(s0);

float2 TextureSize;
float Scale;
float2 Position;

struct VS_OUTPUT
{
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
};

float4 PixelShader(VS_OUTPUT input) : SV_TARGET
{
    // Calculate the center of the pixel in screen space
    float2 screenPos = input.Position.xy / Scale - Position;

    // Map to texture coordinates
    float2 texCoord = screenPos / TextureSize;

    // Super sample: average 4 samples around the texCoord
    float2 offset = 0.5 / TextureSize;
    float4 color =
        tex2D(TextureSampler, texCoord + float2(-offset.x, -offset.y)) +
        tex2D(TextureSampler, texCoord + float2( offset.x, -offset.y)) +
        tex2D(TextureSampler, texCoord + float2(-offset.x,  offset.y)) +
        tex2D(TextureSampler, texCoord + float2( offset.x,  offset.y));
    color /= 4.0;

    return color;
}

technique SuperSampling
{
    pass P0
    {
        PixelShader = compile ps_4_0_level_9_1 PixelShader();
    }
}