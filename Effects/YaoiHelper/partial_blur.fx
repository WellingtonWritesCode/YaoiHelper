#define DECLARE_TEXTURE(Name, index) \
    texture Name: register(t##index); \
    sampler Name##Sampler: register(s##index)

#define SAMPLE_TEXTURE(Name, texCoord) tex2D(Name##Sampler, texCoord)

uniform float Time; // level.TimeActive
uniform float2 CamPos; // level.Camera.Position
uniform float2 Dimensions; // new Vector2(320, 180)

uniform float4x4 TransformMatrix;
uniform float4x4 ViewMatrix;

DECLARE_TEXTURE(text, 0);
float4 SpritePixelShader(float2 uv : TEXCOORD0) : COLOR0
{
    float Pi = 6.28318530718; // Pi*2
    
    // GAUSSIAN BLUR SETTINGS {{{
    float Directions = 16.0; // BLUR DIRECTIONS (Default 16.0 - More is better but slower)
    float Quality = 4.0; // BLUR QUALITY (Default 4.0 - More is better but slower)
    float Size = (1 * sin(Time / 2 + uv.y * 3) + 0.5 * sin(Time / 5 - uv.x * 5) + 0.3 * cos(Time / 3 + uv.x * 4)) * 5;
    // GAUSSIAN BLUR SETTINGS }}}
   
    float2 Radius = Size/Dimensions.xy;
    
    // Normalized pixel coordinates (from 0 to 1)
    // Pixel colour
    float4 Color = SAMPLE_TEXTURE(text, uv);

    // Blur calculations
    for( float d=0.0; d<Pi; d+=Pi/Directions)
    {
		for(float i=1.0/Quality; i<=1.0; i+=1.0/Quality)
        {
			Color += SAMPLE_TEXTURE(text, uv+float2(cos(d),sin(d))*Radius*i);		
        }
    }
    
    // Output to screen
    Color /= Quality * Directions;
    return Color;

}

void SpriteVertexShader(inout float4 color    : COLOR0,
                        inout float2 texCoord : TEXCOORD0,
                        inout float4 position : SV_Position)
{
    position = mul(position, ViewMatrix);
    position = mul(position, TransformMatrix);
}

technique Shader
{
    pass pass0
    {
        VertexShader = compile vs_3_0 SpriteVertexShader();
        PixelShader = compile ps_3_0 SpritePixelShader();
    }
}
