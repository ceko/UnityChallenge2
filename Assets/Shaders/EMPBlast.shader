// Per pixel bumped refraction.
// Uses a normal map to distort the image behind, and
// an additional texture to tint the color.

Shader "Custom/EMPBlast" {
	Properties {		
		_BumpAmt  ("Distortion", range (0,128)) = 10
		_RimColor ("Rim Color", Color) = (1,0,0,0)				
		_RimWidth ("Rim Width", range(.001,1)) = .3
		_InnerColor ("Inner Color", Color) = (1,1,1,0)
		_BumpMap ("Normalmap", 2D) = "bump" {}		
		_CrawlSpeed ("Distortion Crawl Speed", float) = .03
	}

	Category {

		// We must be transparent, so other objects are drawn before this one.
		Tags { "Queue"="Transparent+1" "RenderType"="Transparent" }
		Blend One Zero

		SubShader {
			
			// This pass grabs the screen behind the object into a texture.
			// We can access the result in the next pass as _GrabTexture
			GrabPass {
				Name "BASE"
				Tags { "LightMode" = "Always" }
			}
																													
			// Main pass: Take the texture grabbed above and use the bumpmap to perturb it
			// on to the screen
			Pass {
				Name "BASE"
				Tags { "LightMode" = "Always" }
				
	CGPROGRAM
	#pragma target 3.0	
	#pragma vertex vert
	#pragma fragment frag
	#pragma multi_compile_fog
	#include "UnityCG.cginc"

	struct appdata_t {
		float4 vertex : POSITION;
		float2 texcoord: TEXCOORD0;
		float3 normal : NORMAL;
	};

	struct v2f {
		float4 vertex : SV_POSITION;
		float4 uvgrab : TEXCOORD0;
		float2 uv : TEXCOORD1;	
		float2 uvbump : TEXCOORD2;		
		float3 normal : NORMAL;
		float3 viewDir: VECTOR;
		UNITY_FOG_COORDS(3)
	};

	float _BumpAmt;
	float4 _BumpMap_ST;	
	float _CrawlSpeed;

	v2f vert (appdata_t v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		#if UNITY_UV_STARTS_AT_TOP
		float scale = -1.0;
		#else
		float scale = 1.0;
		#endif
		o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
		o.uvgrab.zw = o.vertex.zw;		
		o.uvbump = TRANSFORM_TEX( v.texcoord, _BumpMap ) + _Time * _CrawlSpeed;	
		o.normal = v.normal;
				
		o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
		UNITY_TRANSFER_FOG(o,o.vertex);
		return o;
	}

	sampler2D _GrabTexture;
	float4 _GrabTexture_TexelSize;
	sampler2D _BumpMap;	
	float4 _RimColor;
	float _RimWidth;
	float4 _InnerColor;
	
	half4 frag (v2f i) : COLOR
	{	
			
		// calculate perturbed coordinates
		float4 bumpColor = tex2D( _BumpMap, i.uvbump + _BumpMap_ST.xy);
		float bumpGrey = (bumpColor.r/3 + bumpColor.g/3 + bumpColor.b/3) +.1;
		half2 bump = UnpackNormal(bumpColor).rg; // we could optimize this by just reading the x & y without reconstructing the Z			
		float2 offset = bump * _BumpAmt * _GrabTexture_TexelSize.xy;
		
		float dotProduct = 1 - dot(i.normal, i.viewDir);
		
		float rimInfluence = smoothstep(.6-_RimWidth, 1.0, dotProduct);
		i.uvgrab.xy = offset * rimInfluence * i.uvgrab.z + i.uvgrab.xy;
		
		half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
		half4 tint = lerp(_InnerColor, _RimColor, rimInfluence);	
		col *= tint;
		UNITY_APPLY_FOG(i.fogCoord, col);
				
		return col;
	}
	ENDCG
			}
		}

	}

}