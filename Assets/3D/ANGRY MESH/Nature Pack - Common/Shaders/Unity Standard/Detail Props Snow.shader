// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ANGRYMESH/Nature Pack/Standard/Detail Props Snow"
{
	Properties
	{
		[Header(Base)]_BaseSmoothness("Base Smoothness", Range( 0 , 1)) = 0.5
		_BaseAOIntensity("Base AO Intensity", Range( 0 , 1)) = 0.5
		_BumpScale("Base Normal Intensity", Range( 0 , 2)) = 1
		_BaseColor("Base Color", Color) = (1,1,1,0)
		[NoScaleOffset]_BaseAlbedoASmoothness("Base Albedo (A Smoothness)", 2D) = "gray" {}
		[Normal][NoScaleOffset]_BaseNormalMap("Base NormalMap", 2D) = "bump" {}
		[NoScaleOffset]_BaseAOANoiseMask("Base AO (A NoiseMask)", 2D) = "white" {}
		[Header(Top)][Toggle(_TOPENABLENOISE_ON)] _TopEnableNoise("Top Enable Noise", Float) = 0
		_TopNoiseUVScale("Top Noise UV Scale", Range( 0.2 , 10)) = 1
		_TopSmoothness("Top Smoothness", Range( 0 , 1)) = 1
		_TopUVScale("Top UV Scale", Range( 1 , 30)) = 10
		_TopIntensity("Top Intensity", Range( 0 , 1)) = 0
		_TopOffset("Top Offset", Range( 0 , 1)) = 0.5
		_TopContrast("Top Contrast", Range( 0 , 2)) = 1
		_TopNormalIntensity("Top Normal Intensity", Range( 0 , 2)) = 1
		_TopColor("Top Color", Color) = (1,1,1,0)
		[NoScaleOffset]_TopAlbedoASmoothness("Top Albedo (A Smoothness)", 2D) = "gray" {}
		[Normal][NoScaleOffset]_TopNormalMap("Top NormalMap", 2D) = "bump" {}
		[Header(Detail)]_DetailUVScale("Detail UV Scale", Range( 0 , 40)) = 10
		_DetailAlbedoIntensity("Detail Albedo Intensity", Range( 0 , 1)) = 1
		_DetailNormalMapIntensity("Detail NormalMap Intensity", Range( 0 , 2)) = 1
		[NoScaleOffset]_DetailAlbedo("Detail Albedo", 2D) = "gray" {}
		[Normal][NoScaleOffset]_DetailNormalMap("Detail NormalMap", 2D) = "bump" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma shader_feature_local _TOPENABLENOISE_ON
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows dithercrossfade 
		struct Input
		{
			float2 uv_texcoord;
			half3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
		};

		uniform sampler2D _DetailNormalMap;
		uniform half _DetailUVScale;
		uniform half _DetailNormalMapIntensity;
		uniform sampler2D _BaseNormalMap;
		uniform half _BumpScale;
		uniform sampler2D _TopNormalMap;
		uniform half _TopUVScale;
		uniform half _TopNormalIntensity;
		uniform half _TopOffset;
		uniform half AGP_SnowOffset;
		uniform half _TopContrast;
		uniform half AGP_SnowContrast;
		uniform half _TopIntensity;
		uniform half AGP_SnowIntensity;
		uniform half AGH_SnowMinimumHeight;
		uniform half AGH_SnowFadeHeight;
		uniform sampler2D _BaseAOANoiseMask;
		uniform half _TopNoiseUVScale;
		uniform half4 _BaseColor;
		uniform sampler2D _BaseAlbedoASmoothness;
		uniform sampler2D _DetailAlbedo;
		uniform half _DetailAlbedoIntensity;
		uniform half4 _TopColor;
		uniform sampler2D _TopAlbedoASmoothness;
		uniform half _BaseSmoothness;
		uniform half _TopSmoothness;
		uniform half _BaseAOIntensity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			half2 temp_output_182_0 = ( i.uv_texcoord * _DetailUVScale );
			half2 Detail_UVScale191 = temp_output_182_0;
			float2 uv_BaseNormalMap96 = i.uv_texcoord;
			half3 tex2DNode96 = UnpackScaleNormal( tex2D( _BaseNormalMap, uv_BaseNormalMap96 ), _BumpScale );
			float2 uv_BaseNormalMap288 = i.uv_texcoord;
			half2 temp_output_195_0 = ( i.uv_texcoord * _TopUVScale );
			half2 Top_UVScale197 = temp_output_195_0;
			float3 NormalMap166 = tex2DNode96;
			float3 ase_worldPos = i.worldPos;
			half Top_Mask168 = ( saturate( ( pow( abs( ( saturate( (WorldNormalVector( i , NormalMap166 )).y ) + ( _TopOffset * AGP_SnowOffset ) ) ) , ( (1.0 + (_TopContrast - 0.0) * (20.0 - 1.0) / (1.0 - 0.0)) * AGP_SnowContrast ) ) * ( _TopIntensity * AGP_SnowIntensity ) ) ) * saturate( (0.0 + (ase_worldPos.y - AGH_SnowMinimumHeight) * (1.0 - 0.0) / (( AGH_SnowMinimumHeight + AGH_SnowFadeHeight ) - AGH_SnowMinimumHeight)) ) );
			float4 temp_cast_0 = (( 1.0 - tex2D( _BaseAOANoiseMask, ( i.uv_texcoord * _TopNoiseUVScale ) ).a )).xxxx;
			#ifdef _TOPENABLENOISE_ON
				float4 staticSwitch270 = temp_cast_0;
			#else
				float4 staticSwitch270 = half4(1,1,1,1);
			#endif
			half4 Top_Noise225 = staticSwitch270;
			half3 lerpResult158 = lerp( BlendNormals( UnpackScaleNormal( tex2D( _DetailNormalMap, Detail_UVScale191 ), _DetailNormalMapIntensity ) , tex2DNode96 ) , BlendNormals( UnpackScaleNormal( tex2D( _BaseNormalMap, uv_BaseNormalMap288 ), ( _BumpScale * 0.5 ) ) , UnpackScaleNormal( tex2D( _TopNormalMap, Top_UVScale197 ), _TopNormalIntensity ) ) , ( Top_Mask168 * Top_Noise225 ).rgb);
			half3 normalizeResult136 = normalize( lerpResult158 );
			half3 Output_Normal320 = normalizeResult136;
			o.Normal = Output_Normal320;
			float2 uv_BaseAlbedoASmoothness162 = i.uv_texcoord;
			half4 tex2DNode162 = tex2D( _BaseAlbedoASmoothness, uv_BaseAlbedoASmoothness162 );
			half4 temp_output_163_0 = ( _BaseColor * tex2DNode162 );
			half4 blendOpSrc178 = ( tex2D( _DetailAlbedo, temp_output_182_0 ) * 2.0 );
			half4 blendOpDest178 = temp_output_163_0;
			half4 lerpResult187 = lerp( temp_output_163_0 , ( saturate( (( blendOpDest178 > 0.5 ) ? ( 1.0 - 2.0 * ( 1.0 - blendOpDest178 ) * ( 1.0 - blendOpSrc178 ) ) : ( 2.0 * blendOpDest178 * blendOpSrc178 ) ) )) , _DetailAlbedoIntensity);
			half4 tex2DNode172 = tex2D( _TopAlbedoASmoothness, temp_output_195_0 );
			half4 lerpResult157 = lerp( lerpResult187 , ( _TopColor * tex2DNode172 ) , ( Top_Mask168 * Top_Noise225 ));
			half4 Output_Albedo318 = lerpResult157;
			o.Albedo = Output_Albedo318.rgb;
			float AlbedoAlphaSmoothness212 = tex2DNode162.a;
			half Top_Smoothness220 = tex2DNode172.a;
			half lerpResult217 = lerp( ( AlbedoAlphaSmoothness212 + (-1.0 + (_BaseSmoothness - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) ) , ( Top_Smoothness220 + (-1.0 + (_TopSmoothness - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) ) , Top_Mask168);
			half Output_Smoothness223 = lerpResult217;
			o.Smoothness = Output_Smoothness223;
			half4 temp_cast_3 = (1.0).xxxx;
			float2 uv_BaseAOANoiseMask200 = i.uv_texcoord;
			half4 lerpResult201 = lerp( temp_cast_3 , tex2D( _BaseAOANoiseMask, uv_BaseAOANoiseMask200 ) , _BaseAOIntensity);
			half4 AO207 = lerpResult201;
			half4 temp_cast_4 = (1.0).xxxx;
			half4 lerpResult303 = lerp( AO207 , temp_cast_4 , 0.8);
			half4 lerpResult290 = lerp( AO207 , lerpResult303 , Top_Mask168);
			half4 Output_AO322 = lerpResult290;
			o.Occlusion = Output_AO322.r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=18935
1920;78;1920;1019;4949.289;2550.756;1.496239;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;287;-2688,-256;Float;True;Property;_BaseNormalMap;Base NormalMap;5;2;[Normal];[NoScaleOffset];Create;True;0;0;0;False;0;False;None;None;True;bump;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;326;-2688,-32;Half;False;Property;_BumpScale;Base Normal Intensity;2;0;Create;False;0;0;0;False;0;False;1;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;96;-2048,-288;Inherit;True;Property;_Test;Test;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;166;-1536,-256;Float;False;NormalMap;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;281;-5248,-1792;Half;False;Property;_TopNoiseUVScale;Top Noise UV Scale;8;0;Create;True;0;0;0;False;0;False;1;0.2;0.2;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;279;-5248,-1920;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;107;-4992,736;Half;False;Property;_TopContrast;Top Contrast;13;0;Create;True;0;0;0;False;0;False;1;2;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;167;-5248,384;Inherit;False;166;NormalMap;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;277;-5248,-2176;Float;True;Property;_BaseAOANoiseMask;Base AO (A NoiseMask);6;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;280;-4992,-1920;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;278;-4608,-1760;Inherit;True;Property;_TextureSample1;Texture Sample 1;5;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;103;-4992,640;Half;False;Property;_TopOffset;Top Offset;12;0;Create;True;0;0;0;False;0;False;0.5;0.371;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;93;-4992,384;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;246;-4992,544;Half;False;Property;_TopIntensity;Top Intensity;11;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;181;-2688,-2048;Half;False;Property;_DetailUVScale;Detail UV Scale;18;0;Create;True;0;0;0;False;1;Header(Detail);False;10;3.4;0;40;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;183;-2688,-2176;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;194;-2688,-1280;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;196;-2688,-1152;Half;False;Property;_TopUVScale;Top UV Scale;10;0;Create;True;0;0;0;False;0;False;10;2;1;30;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;114;-4608,640;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;20;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;329;-4224,-1664;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;328;-4352,384;Inherit;False;AG Global Snow - Props;-1;;2;13862ff5366d2ea409a2b82c7b7675e1;0;4;22;FLOAT;0;False;24;FLOAT;0;False;23;FLOAT;0;False;25;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;331;-4352,544;Inherit;False;AG Global Snow - Height;-1;;3;792d553d9b3743f498843a42559debdb;0;0;1;FLOAT;43
Node;AmplifyShaderEditor.ColorNode;269;-4608,-1536;Half;False;Constant;_Color0;Color 0;20;0;Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;182;-2304,-2176;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;195;-2304,-1280;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;270;-3968,-1664;Float;False;Property;_TopEnableNoise;Top Enable Noise;7;0;Create;True;0;0;0;False;1;Header(Top);False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;289;-2688,48;Half;False;Constant;_TopInt;Top Int;22;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;330;-3968,384;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;203;-4608,-1952;Half;False;Property;_BaseAOIntensity;Base AO Intensity;1;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;197;-2048,-896;Half;False;Top_UVScale;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;191;-2048,-1888;Half;False;Detail_UVScale;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;200;-4608,-2176;Inherit;True;Property;_Texture;Texture;5;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;231;-4608,-1856;Half;False;Constant;_White1;White1;19;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;176;-2048,-2176;Inherit;True;Property;_DetailAlbedo;Detail Albedo;21;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;198;-2688,128;Inherit;False;197;Top_UVScale;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;225;-3584,-1664;Half;False;Top_Noise;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;193;-2688,-512;Inherit;False;191;Detail_UVScale;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;165;-2048,-1792;Half;False;Property;_BaseColor;Base Color;3;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.9411765,0.9411765,0.9411765,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;189;-2688,-384;Half;False;Property;_DetailNormalMapIntensity;Detail NormalMap Intensity;20;0;Create;True;0;0;0;False;0;False;1;0.7;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;327;-2304,0;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;139;-2688,224;Half;False;Property;_TopNormalIntensity;Top Normal Intensity;14;0;Create;True;0;0;0;False;0;False;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;162;-2048,-1600;Inherit;True;Property;_BaseAlbedoASmoothness;Base Albedo (A Smoothness);4;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;172;-2048,-1088;Inherit;True;Property;_TopAlbedoASmoothness;Top Albedo (A Smoothness);16;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;316;-2048,-1968;Float;False;Constant;_Multiply_X2;Multiply_X2;22;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;201;-4096,-2176;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;168;-3712,384;Half;False;Top_Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;288;-2048,-64;Inherit;True;Property;_TextureSample2;Texture Sample 2;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;207;-3584,-2176;Half;False;AO;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;265;-1536,96;Inherit;False;225;Top_Noise;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;163;-1536,-1792;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;216;-5248,-384;Half;False;Property;_BaseSmoothness;Base Smoothness;0;0;Create;True;0;0;0;False;1;Header(Base);False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;220;-1664,-992;Half;False;Top_Smoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;171;-1536,0;Inherit;False;168;Top_Mask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;119;-2048,160;Inherit;True;Property;_TopNormalMap;Top NormalMap;17;2;[Normal];[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;218;-5248,-128;Half;False;Property;_TopSmoothness;Top Smoothness;9;0;Create;True;0;0;0;False;0;False;1;0.585;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;315;-1664,-2176;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;212;-1664,-1472;Float;False;AlbedoAlphaSmoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;188;-2048,-512;Inherit;True;Property;_DetailNormalMap;Detail NormalMap;22;2;[Normal];[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;306;-5248,-1184;Half;False;Constant;_White;White;22;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;252;-1536,-128;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;208;-5248,-1280;Inherit;False;207;AO;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;214;-5248,-512;Inherit;False;212;AlbedoAlphaSmoothness;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;309;-4864,-128;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;243;-1408,-928;Inherit;False;225;Top_Noise;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;175;-2048,-1280;Half;False;Property;_TopColor;Top Color;15;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;180;-1536,-1664;Half;False;Property;_DetailAlbedoIntensity;Detail Albedo Intensity;19;0;Create;True;0;0;0;False;0;False;1;0.719;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;258;-1152,0;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendNormalsNode;190;-1536,-512;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TFHCRemapNode;310;-4864,-384;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;221;-5248,-256;Inherit;False;220;Top_Smoothness;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;170;-1408,-1024;Inherit;False;168;Top_Mask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;304;-5248,-1088;Half;False;Constant;_AOTopIntensity;AO Top Intensity;22;0;Create;True;0;0;0;False;0;False;0.8;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;178;-1408,-2176;Inherit;False;Overlay;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;253;-1152,-1152;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;308;-4608,-256;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;303;-4864,-1184;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;291;-4864,-1056;Inherit;False;168;Top_Mask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;222;-4608,-64;Inherit;False;168;Top_Mask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;158;-1024,-512;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;311;-4608,-512;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;187;-1152,-2176;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;173;-1152,-1280;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;217;-4352,-512;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;136;-768,-512;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;157;-896,-2176;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;290;-4608,-1280;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;318;-640,-2176;Half;False;Output_Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;322;-4352,-1280;Half;False;Output_AO;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;223;-4096,-512;Half;False;Output_Smoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;320;-512,-512;Half;False;Output_Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;319;496,-2176;Inherit;False;318;Output_Albedo;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;224;496,-1984;Inherit;False;223;Output_Smoothness;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;321;496,-2080;Inherit;False;320;Output_Normal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;324;496,-1888;Inherit;False;322;Output_AO;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;880,-2176;Half;False;True;-1;2;;0;0;Standard;ANGRYMESH/Nature Pack/Standard/Detail Props Snow;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;True;False;False;False;True;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;-0.09;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;8.6;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;240;-2688,-640;Inherit;False;2429.878;100;;0;// Blend Normal Maps;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;325;496,-2304;Inherit;False;768.5388;100;;0;// Outputs;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;237;-2688,-2304;Inherit;False;2303.162;100;;0;// Blend Albedo Maps;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;230;-5248,-640;Inherit;False;1537.292;100;;0;// Smoothness;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;241;-5248,256;Inherit;False;1790.65;100;;0;// Top World Mapping;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;210;-5248,-2304;Inherit;False;1917.661;100;;0;// AO;1,1,1,1;0;0
WireConnection;96;0;287;0
WireConnection;96;5;326;0
WireConnection;166;0;96;0
WireConnection;280;0;279;0
WireConnection;280;1;281;0
WireConnection;278;0;277;0
WireConnection;278;1;280;0
WireConnection;93;0;167;0
WireConnection;114;0;107;0
WireConnection;329;0;278;4
WireConnection;328;22;93;2
WireConnection;328;24;246;0
WireConnection;328;23;103;0
WireConnection;328;25;114;0
WireConnection;182;0;183;0
WireConnection;182;1;181;0
WireConnection;195;0;194;0
WireConnection;195;1;196;0
WireConnection;270;1;269;0
WireConnection;270;0;329;0
WireConnection;330;0;328;0
WireConnection;330;1;331;43
WireConnection;197;0;195;0
WireConnection;191;0;182;0
WireConnection;200;0;277;0
WireConnection;176;1;182;0
WireConnection;225;0;270;0
WireConnection;327;0;326;0
WireConnection;327;1;289;0
WireConnection;172;1;195;0
WireConnection;201;0;231;0
WireConnection;201;1;200;0
WireConnection;201;2;203;0
WireConnection;168;0;330;0
WireConnection;288;0;287;0
WireConnection;288;5;327;0
WireConnection;207;0;201;0
WireConnection;163;0;165;0
WireConnection;163;1;162;0
WireConnection;220;0;172;4
WireConnection;119;1;198;0
WireConnection;119;5;139;0
WireConnection;315;0;176;0
WireConnection;315;1;316;0
WireConnection;212;0;162;4
WireConnection;188;1;193;0
WireConnection;188;5;189;0
WireConnection;252;0;288;0
WireConnection;252;1;119;0
WireConnection;309;0;218;0
WireConnection;258;0;171;0
WireConnection;258;1;265;0
WireConnection;190;0;188;0
WireConnection;190;1;96;0
WireConnection;310;0;216;0
WireConnection;178;0;315;0
WireConnection;178;1;163;0
WireConnection;253;0;170;0
WireConnection;253;1;243;0
WireConnection;308;0;221;0
WireConnection;308;1;309;0
WireConnection;303;0;208;0
WireConnection;303;1;306;0
WireConnection;303;2;304;0
WireConnection;158;0;190;0
WireConnection;158;1;252;0
WireConnection;158;2;258;0
WireConnection;311;0;214;0
WireConnection;311;1;310;0
WireConnection;187;0;163;0
WireConnection;187;1;178;0
WireConnection;187;2;180;0
WireConnection;173;0;175;0
WireConnection;173;1;172;0
WireConnection;217;0;311;0
WireConnection;217;1;308;0
WireConnection;217;2;222;0
WireConnection;136;0;158;0
WireConnection;157;0;187;0
WireConnection;157;1;173;0
WireConnection;157;2;253;0
WireConnection;290;0;208;0
WireConnection;290;1;303;0
WireConnection;290;2;291;0
WireConnection;318;0;157;0
WireConnection;322;0;290;0
WireConnection;223;0;217;0
WireConnection;320;0;136;0
WireConnection;0;0;319;0
WireConnection;0;1;321;0
WireConnection;0;4;224;0
WireConnection;0;5;324;0
ASEEND*/
//CHKSM=54626929955199834A178F2CB7D85923BC85B137