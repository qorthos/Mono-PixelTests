#if OPENGL
#define PS_SHADERMODEL ps_3_0
#else
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
    AddressU = clamp;
    AddressV = clamp;
    MagFilter = point;
    MinFilter = point;
    MipFilter = point;
};

float2 TextureSize; // sprite.Width, sprite.Height
float2 InvTextureSize; // 1 / TextureSize

struct PixelShaderInput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

float4 SharpPixelPS(PixelShaderInput input) : COLOR0
{
    float2 uv = input.TexCoord;

    // Compute screen-space derivative to know texel size on screen
    float2 texelSizeScreen = abs(ddx(uv) + ddy(uv)); // approx how much uv changes per screen pixel
    float texelAreaScreen = texelSizeScreen.x * texelSizeScreen.y;

    // lod-like factor: >1 means texel is smaller than screen pixel (need AA), <1 means bigger (snap harder)
    float sharpness = saturate(1.0 - texelAreaScreen * 0.5); // tune the 0.5 multiplier

    // Base nearest-neighbor sample
    float2 pixel = floor(uv * TextureSize + 0.5) * InvTextureSize;
    float4 nearest = tex2D(SpriteTextureSampler, pixel);

    // Optional: cheap 2x2 bilinear when texels are small (anti-aliased downscale)
    float4 bilinear = tex2D(SpriteTextureSampler, uv);

    // Blend: sharp nearest when zoomed in, more bilinear when zoomed out
    float4 color = lerp(bilinear, nearest, sharpness);

    // Multiply by vertex color (tint, alpha)
    return color * input.Color;
}

technique SharpTechnique
{
    pass Pass1
    {
        PixelShader = compile PS_SHADERMODEL SharpPixelPS();
    }
}