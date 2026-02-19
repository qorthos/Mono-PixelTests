#if OPENGL
#define SV_TARGET0 COLOR0
#define PS_SHADERMODEL ps_3_0
#endif

// Parameters from C#
float2 TextureSize;
float4 SourceRect; // x, y, width, height in pixels

sampler2D SpriteTextureSampler : register(s0);

struct VertexData
{
    float4 Position : SV_Position;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

float4 MainPS(VertexData input) : SV_TARGET0
{
    // 1. Convert normalized UV (0-1) to Texel Space (0-TextureWidth)
    float2 pixel = input.TexCoord * TextureSize;

    // 2. The Smoothing Math (The "Fat Pixel")
    // This creates a tiny linear ramp (the width of 1 screen pixel) 
    // between the chunky game pixels.
    float2 fatPixel = floor(pixel) + 0.5;
    float2 delta = fwidth(pixel);
    float2 uv = fatPixel + clamp((pixel - fatPixel) / delta, -0.5, 0.5);

    // 3. Clamp to SourceRect
    // We add a tiny 0.01 margin to prevent sampling the neighbor tile's edge
    float2 minUV = SourceRect.xy + 0.01;
    float2 maxUV = SourceRect.xy + SourceRect.zw - 0.01;
    float2 finalUV = clamp(uv, minUV, maxUV);

    return tex2D(SpriteTextureSampler, finalUV / TextureSize) * input.Color;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};