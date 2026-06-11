#define DECLARE_TEXTURE(Name, index) \
    texture Name: register(t##index); \
    sampler Name##Sampler: register(s##index)

#define SAMPLE_TEXTURE(Name, texCoord) tex2D(Name##Sampler, texCoord)

uniform float Time; // level.TimeActive
uniform float2 CamPos; // level.Camera.Position
uniform float2 Dimensions; // new Vector2(320, 180)

uniform float4x4 ViewMatrix;
uniform float4x4 TransformMatrix;

DECLARE_TEXTURE(text1, 0);
DECLARE_TEXTURE(text2, 1);
DECLARE_TEXTURE(text3, 2);

float4 SpritePixelShader(float2 uv : TEXCOORD0) : COLOR0
{
	float4 color1 = SAMPLE_TEXTURE(text1, uv);
	float4 color2 = SAMPLE_TEXTURE(text2, uv);
	float4 color3 = SAMPLE_TEXTURE(text3, uv);

	float4 color12 = color2 + (1. - color2.a) * color1; 
	return color3 + (1. - color3.a) * color12;
}

technique Shader
{
    pass pass0
    {
        PixelShader = compile ps_3_0 SpritePixelShader();
    }
}
