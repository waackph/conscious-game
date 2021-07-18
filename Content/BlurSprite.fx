#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	//return tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color;
	float2 uv = input.texCoord.xy;

    float amount = 0.0;

    amount = (1.0 + sin(time*6.0)) * 0.5;
    amount *= 1.0 + sin(time*16.0) * 0.5;
    amount *= 1.0 + sin(time*19.0) * 0.5;
    amount *= 1.0 + sin(time*27.0) * 0.5;
    amount = pow(amount, 3.0);

    amount *= 0.05;

    float3 col;
    col.r = tex2D(texture1, float2(uv.x + amount, uv.y)).r;
    col.g = tex2D(texture1, uv).g;
    col.b = tex2D(texture1, float2(uv.x - amount, uv.y)).b;

    col *= (1.0 - amount * 0.5);

    return float4(col, 1.0);
}

technique ChromaticAberation
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};