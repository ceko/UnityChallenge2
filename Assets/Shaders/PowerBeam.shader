Shader "Custom/PowerBeam" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		[HDR]
		_Emission ("Emission", Color) = (1,1,1,1)
		_Churn ("Churn", Range(0,1)) = 0.0		
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull Off
				
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows
		#pragma vertex vert addshadow
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _Emission;
		float _Churn;
		
		void vert (inout appdata_full v) {	
			float3 worldUp = float3(0,1,0);
			float liquidDot = ceil(saturate(dot(worldUp, normalize(v.normal))));
			float3 offset;
			offset = -((worldUp * v.normal * .15) + worldUp * abs(sin(v.vertex.x*3 + _Time*3))) * .5 * liquidDot;
			offset.y += .3 * liquidDot;
			offset.y -= sin(v.vertex.z * 3 + _Time * 20) * liquidDot * .2;
			
			//offset.y -= worldUp * v.normal * (abs(cos(v.vertex.x*3 + _Time*50) * .12)) * liquidDot;
			//offset.x -= cos(v.vertex.z*5 + _Time*100) * .12;
			
			//offset.xy = float2(clamp(offset.x, -.08, .08), clamp(offset.y, -.08, .08));			
			v.vertex += float4(offset, 0) * _Churn;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color			;
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex.xy) * _Color;			
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;		
			o.Emission = _Emission + (c.r * 3);	
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
