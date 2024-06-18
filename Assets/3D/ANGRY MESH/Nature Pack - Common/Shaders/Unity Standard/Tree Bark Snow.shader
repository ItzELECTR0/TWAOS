// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ANGRYMESH/Nature Pack/Standard/Tree Bark Snow"
{
	Properties
	{
		[Header(Base)]_Glossiness("Base Smoothness", Range( 0 , 1)) = 0.5
		_OcclusionStrength("Base Tree AO", Range( 0 , 1)) = 0.5
		_BaseBarkAO("Base Bark AO", Range( 0 , 1)) = 0.5
		_BumpScale("Base Normal Intensity", Range( 0 , 2)) = 1
		_Color("Base Color", Color) = (1,1,1,1)
		[NoScaleOffset]_MainTex("Base Albedo (A Smoothness)", 2D) = "gray" {}
		[Normal][NoScaleOffset]_BumpMap("Base NormalMap", 2D) = "bump" {}
		[NoScaleOffset]_OcclusionMap("Base AO (A Height)", 2D) = "white" {}
		[Header(Top)]_TopSmoothness("Top Smoothness", Range( 0 , 1)) = 1
		_TopUVScale("Top UV Scale", Range( 1 , 30)) = 10
		_TopIntensity("Top Intensity", Range( 0 , 1)) = 1
		_TopOffset("Top Offset", Range( 0 , 1)) = 0.5
		_TopContrast("Top Contrast", Range( 0 , 2)) = 1
		_TopArrowDirection("Top Arrow Direction", Range( 0 , 2)) = 0
		_DetailNormalMapScale("Top Normal Intensity", Range( 0 , 2)) = 1
		_TopColor("Top Color", Color) = (1,1,1,0)
		[NoScaleOffset]_TopAlbedoASmoothness("Top Albedo (A Smoothness)", 2D) = "gray" {}
		[Normal][NoScaleOffset]_TopNormalMap("Top NormalMap", 2D) = "bump" {}
		[Header(Wind Trunk (use common settings for both materials))]_WindTrunkAmplitude("Wind Trunk Amplitude", Range( 0 , 3)) = 1
		_WindTrunkStiffness("Wind Trunk Stiffness", Range( 1 , 3)) = 2
		[HideInInspector] _texcoord4( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows novertexlights nolightmap  nodynlightmap nodirlightmap nometa dithercrossfade vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv4_texcoord4;
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			half3 worldNormal;
			INTERNAL_DATA
		};

		uniform half AGW_WindScale;
		uniform half AGW_WindSpeed;
		uniform half AGW_WindToggle;
		uniform half _WindTrunkAmplitude;
		uniform half AGW_WindAmplitude;
		uniform half _WindTrunkStiffness;
		uniform half AGW_WindTreeStiffness;
		uniform half3 AGW_WindDirection;
		uniform sampler2D _BumpMap;
		uniform half _BumpScale;
		uniform sampler2D _TopNormalMap;
		uniform half _TopUVScale;
		uniform half _DetailNormalMapScale;
		uniform half _TopOffset;
		uniform half AGT_SnowOffset;
		uniform half _TopContrast;
		uniform half AGT_SnowContrast;
		uniform half _TopIntensity;
		uniform half AGT_SnowIntensity;
		uniform half _TopArrowDirection;
		uniform half AGT_SnowArrow;
		uniform sampler2D _OcclusionMap;
		uniform half AGH_SnowMinimumHeight;
		uniform half AGH_SnowFadeHeight;
		uniform sampler2D _MainTex;
		uniform half4 _Color;
		uniform sampler2D _TopAlbedoASmoothness;
		uniform half4 _TopColor;
		uniform half _Glossiness;
		uniform half _TopSmoothness;
		uniform half _BaseBarkAO;
		uniform half _OcclusionStrength;
		uniform half AG_TreesAO;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			half temp_output_99_0_g40 = ( 1.0 * AGW_WindScale );
			half temp_output_101_0_g40 = ( 1.0 * AGW_WindSpeed );
			half mulTime10_g40 = _Time.y * temp_output_101_0_g40;
			half temp_output_73_0_g40 = ( AGW_WindToggle * _WindTrunkAmplitude * AGW_WindAmplitude );
			half VColor_Red885 = v.color.r;
			half temp_output_48_0_g40 = pow( abs( VColor_Red885 ) , _WindTrunkStiffness );
			half temp_output_28_0_g40 = ( ( sin( ( ( ase_worldPos.z * temp_output_99_0_g40 ) + ( ( temp_output_101_0_g40 * _SinTime.z ) + mulTime10_g40 ) ) ) * temp_output_73_0_g40 ) * temp_output_48_0_g40 );
			half temp_output_49_0_g40 = 0.0;
			half3 appendResult63_g40 = (half3(temp_output_28_0_g40 , ( ( sin( ( ( temp_output_99_0_g40 * ase_worldPos.y ) + mulTime10_g40 ) ) * temp_output_73_0_g40 ) * temp_output_49_0_g40 ) , temp_output_28_0_g40));
			half3 Output_Wind382 = mul( unity_WorldToObject, half4( ( ( appendResult63_g40 + ( temp_output_73_0_g40 * ( temp_output_48_0_g40 + temp_output_49_0_g40 ) * ( 1.0 * (1.0 + (AGW_WindTreeStiffness - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) ) ) ) * AGW_WindDirection ) , 0.0 ) ).xyz;
			v.vertex.xyz += Output_Wind382;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv4_BumpMap437 = i.uv4_texcoord4;
			float2 uv_BumpMap96 = i.uv_texcoord;
			half VColor_Blue887 = i.vertexColor.b;
			half Blend_Mask319 = ( 1.0 - VColor_Blue887 );
			half3 lerpResult321 = lerp( UnpackScaleNormal( tex2D( _BumpMap, uv4_BumpMap437 ), _BumpScale ) , UnpackScaleNormal( tex2D( _BumpMap, uv_BumpMap96 ), _BumpScale ) , Blend_Mask319);
			half2 temp_output_195_0 = ( i.uv_texcoord * _TopUVScale );
			half2 Top_UVScale197 = temp_output_195_0;
			half3 NormalMap166 = lerpResult321;
			half3 Wind_Direction616 = AGW_WindDirection;
			half dotResult631 = dot( (WorldNormalVector( i , NormalMap166 )) , Wind_Direction616 );
			half GlobalSnowIntensity818 = AGT_SnowIntensity;
			float2 uv4_OcclusionMap443 = i.uv4_texcoord4;
			half4 tex2DNode443 = tex2D( _OcclusionMap, uv4_OcclusionMap443 );
			float2 uv_OcclusionMap200 = i.uv_texcoord;
			half4 tex2DNode200 = tex2D( _OcclusionMap, uv_OcclusionMap200 );
			half lerpResult553 = lerp( tex2DNode443.a , tex2DNode200.a , Blend_Mask319);
			half Blend_Height554 = lerpResult553;
			float3 ase_worldPos = i.worldPos;
			half Top_Mask168 = ( saturate( ( saturate( ( pow( abs( ( saturate( (WorldNormalVector( i , NormalMap166 )).y ) + ( _TopOffset * AGT_SnowOffset ) ) ) , ( (1.0 + (_TopContrast - 0.0) * (20.0 - 1.0) / (1.0 - 0.0)) * AGT_SnowContrast ) ) * ( _TopIntensity * AGT_SnowIntensity ) ) ) + ( _TopIntensity * pow( abs( ( ( 1.0 - dotResult631 ) * ( _TopArrowDirection * AGT_SnowArrow * GlobalSnowIntensity818 ) * Blend_Height554 ) ) , 30.0 ) ) ) ) * saturate( (0.0 + (ase_worldPos.y - AGH_SnowMinimumHeight) * (1.0 - 0.0) / (( AGH_SnowMinimumHeight + AGH_SnowFadeHeight ) - AGH_SnowMinimumHeight)) ) );
			half3 lerpResult158 = lerp( lerpResult321 , BlendNormals( lerpResult321 , UnpackScaleNormal( tex2D( _TopNormalMap, Top_UVScale197 ), _DetailNormalMapScale ) ) , Top_Mask168);
			half3 normalizeResult136 = normalize( lerpResult158 );
			half3 Output_Normal598 = normalizeResult136;
			o.Normal = Output_Normal598;
			float2 uv4_MainTex176 = i.uv4_texcoord4;
			half4 tex2DNode176 = tex2D( _MainTex, uv4_MainTex176 );
			float2 uv_MainTex162 = i.uv_texcoord;
			half4 tex2DNode162 = tex2D( _MainTex, uv_MainTex162 );
			half4 lerpResult187 = lerp( ( tex2DNode176 * _Color ) , ( _Color * tex2DNode162 ) , Blend_Mask319);
			half4 tex2DNode172 = tex2D( _TopAlbedoASmoothness, temp_output_195_0 );
			half4 lerpResult157 = lerp( lerpResult187 , ( tex2DNode172 * _TopColor ) , Top_Mask168);
			half4 Output_Albedo596 = lerpResult157;
			o.Albedo = Output_Albedo596.rgb;
			half Alpha_SmoothnessCH3452 = tex2DNode176.a;
			half Alpha_SmoothnessCH1212 = tex2DNode162.a;
			half lerpResult453 = lerp( Alpha_SmoothnessCH3452 , Alpha_SmoothnessCH1212 , Blend_Mask319);
			half Top_Smoothness220 = tex2DNode172.a;
			half lerpResult217 = lerp( ( lerpResult453 + (-1.0 + (_Glossiness - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) ) , ( Top_Smoothness220 + (-1.0 + (_TopSmoothness - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) ) , Top_Mask168);
			half Output_Smoothness223 = lerpResult217;
			o.Smoothness = Output_Smoothness223;
			half4 temp_cast_1 = (1.0).xxxx;
			half4 lerpResult201 = lerp( tex2DNode443 , tex2DNode200 , Blend_Mask319);
			half4 lerpResult444 = lerp( temp_cast_1 , lerpResult201 , _BaseBarkAO);
			half VColor_Alpha886 = i.vertexColor.a;
			half lerpResult335 = lerp( 1.0 , VColor_Alpha886 , ( _OcclusionStrength * AG_TreesAO ));
			half AO_BaseTree458 = saturate( lerpResult335 );
			half4 Output_AO207 = ( lerpResult444 * AO_BaseTree458 );
			o.Occlusion = Output_AO207.r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=18935
1920;78;1920;1019;8147.275;1096.825;3.577438;True;False
Node;AmplifyShaderEditor.VertexColorNode;884;-2688,512;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;887;-2432,608;Half;False;VColor_Blue;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;455;-2176,608;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;319;-1984,608;Half;False;Blend_Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;189;-2688,-512;Half;False;Property;_BumpScale;Base Normal Intensity;3;0;Create;False;0;0;0;False;0;False;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;287;-2688,-384;Float;True;Property;_BumpMap;Base NormalMap;6;2;[Normal];[NoScaleOffset];Create;False;0;0;0;False;0;False;None;None;True;bump;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;96;-2304,-288;Inherit;True;Property;_Test;Test;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;322;-1920,-320;Inherit;False;319;Blend_Mask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;437;-2304,-512;Inherit;True;Property;_TextureSample0;Texture Sample 0;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;3;True;bump;Auto;True;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;321;-1664,-512;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;166;-1408,-384;Half;False;NormalMap;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;167;-6064,512;Inherit;False;166;NormalMap;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;107;-5760,864;Half;False;Property;_TopContrast;Top Contrast;12;0;Create;True;0;0;0;False;0;False;1;2;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;103;-5760,768;Half;False;Property;_TopOffset;Top Offset;11;0;Create;True;0;0;0;False;0;False;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;277;-5760,-1408;Float;True;Property;_OcclusionMap;Base AO (A Height);7;1;[NoScaleOffset];Create;False;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.Vector3Node;765;-720,768;Half;False;Global;AGW_WindDirection;AGW_WindDirection;20;0;Create;True;0;0;0;False;0;False;0,0,0;0.9382213,-0.3195158,-0.1328554;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TFHCRemapNode;114;-5376,768;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;20;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;246;-5760,672;Half;False;Property;_TopIntensity;Top Intensity;10;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;93;-5760,512;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;446;-5376,-1184;Inherit;False;319;Blend_Mask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;200;-5376,-1088;Inherit;True;Property;_Texture;Texture;5;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;443;-5376,-1408;Inherit;True;Property;_TextureSample5;Texture Sample 5;5;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;3;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;616;-256,768;Half;False;Wind_Direction;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;636;-6064,1024;Inherit;False;166;NormalMap;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;896;-5120,512;Inherit;False;AG Global Snow - Tree;-1;;37;0af770cdce085fc40bbf5b8250612a37;0;4;22;FLOAT;0;False;24;FLOAT;0;False;23;FLOAT;0;False;25;FLOAT;0;False;2;FLOAT;0;FLOAT;33
Node;AmplifyShaderEditor.RegisterLocalVarNode;818;-4736,576;Half;False;GlobalSnowIntensity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;630;-5760,1024;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;633;-5760,1216;Inherit;False;616;Wind_Direction;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;553;-4864,-1088;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;595;-5504,1248;Half;False;Global;AGT_SnowArrow;AGT_SnowArrow;19;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;668;-5504,1152;Half;False;Property;_TopArrowDirection;Top Arrow Direction;13;0;Create;True;0;0;0;False;0;False;0;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;631;-5504,1024;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;819;-5504,1344;Inherit;False;818;GlobalSnowIntensity;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;554;-4608,-1088;Half;False;Blend_Height;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;674;-5200,1152;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;635;-5376,1024;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;555;-5213,1285;Inherit;False;554;Blend_Height;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;673;-4992,1024;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;899;-4736,1024;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;669;-4992,1152;Half;False;Constant;_FloatContrast;Float Contrast;20;0;Create;True;0;0;0;False;0;False;30;30;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;649;-4608,1024;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;653;-4480,672;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;194;-2688,-1408;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;196;-2688,-1216;Half;False;Property;_TopUVScale;Top UV Scale;9;0;Create;True;0;0;0;False;0;False;10;1;1;30;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;518;-5760,-1888;Half;False;Global;AG_TreesAO;AG_TreesAO;21;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;195;-2432,-1360;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;330;-5760,-1984;Half;False;Property;_OcclusionStrength;Base Tree AO;1;0;Create;False;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;438;-2688,-2176;Float;True;Property;_MainTex;Base Albedo (A Smoothness);5;1;[NoScaleOffset];Create;False;0;0;0;False;0;False;None;None;False;gray;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RegisterLocalVarNode;886;-2432,704;Half;False;VColor_Alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;885;-2432,512;Half;False;VColor_Red;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;513;-4224,512;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;332;-5760,-2176;Half;False;Constant;_Float2;Float 2;19;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;197;-2176,-1184;Half;False;Top_UVScale;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;517;-5376,-2000;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;890;-1536,512;Inherit;False;885;VColor_Red;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;888;-5760,-2080;Inherit;False;886;VColor_Alpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;162;-2176,-1664;Inherit;True;Property;_TextureSample4;Texture Sample 4;16;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;gray;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;176;-2176,-2176;Inherit;True;Property;_TileAlbedo;Tile Albedo;16;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;3;False;gray;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;143;-4096,512;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;901;-4224,640;Inherit;False;AG Global Snow - Height;-1;;39;792d553d9b3743f498843a42559debdb;0;0;1;FLOAT;43
Node;AmplifyShaderEditor.GetLocalVarNode;198;-2688,-64;Inherit;False;197;Top_UVScale;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;335;-5248,-2176;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;139;-2688,32;Half;False;Property;_DetailNormalMapScale;Top Normal Intensity;14;0;Create;False;0;0;0;False;0;False;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;212;-1664,-1600;Half;False;Alpha_SmoothnessCH1;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;898;-1280,512;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;825;-1536,608;Half;False;Property;_WindTrunkStiffness;Wind Trunk Stiffness;19;0;Create;True;0;0;0;False;0;False;2;2;1;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;172;-2176,-1408;Inherit;True;Property;_TopAlbedoASmoothness;Top Albedo (A Smoothness);16;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;900;-3840,512;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;452;-1664,-2049.351;Half;False;Alpha_SmoothnessCH3;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;214;-5760,-512;Inherit;False;452;Alpha_SmoothnessCH3;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;119;-2304,-64;Inherit;True;Property;_TopNormalMap;Top NormalMap;17;2;[Normal];[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;456;-5760,-320;Inherit;False;319;Blend_Mask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;826;-1536,704;Half;False;Property;_WindTrunkAmplitude;Wind Trunk Amplitude;18;0;Create;True;0;0;0;False;1;Header(Wind Trunk (use common settings for both materials));False;1;1;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;165;-2176,-1952;Half;False;Property;_Color;Base Color;4;0;Create;False;0;0;0;False;0;False;1,1,1,1;1,1,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;220;-1664,-1219;Half;False;Top_Smoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;216;-5760,-224;Half;False;Property;_Glossiness;Base Smoothness;0;0;Create;False;0;0;0;False;1;Header(Base);False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;794;-1152,512;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;168;-3584,512;Half;False;Top_Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;520;-4864,-2176;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;454;-5760,-416;Inherit;False;212;Alpha_SmoothnessCH1;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;218;-5760,-32;Half;False;Property;_TopSmoothness;Top Smoothness;8;0;Create;True;0;0;0;False;1;Header(Top);False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;231;-4608,-1312;Half;False;Constant;_White1;White1;19;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;453;-5376,-512;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;175;-2176,-1088;Half;False;Property;_TopColor;Top Color;15;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;458;-4608,-2176;Half;False;AO_BaseTree;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;221;-5760,-128;Inherit;False;220;Top_Smoothness;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;201;-4864,-1408;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;440;-1664,-2176;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;897;-896,512;Inherit;False;AG Global Wind - Tree;-1;;40;b3a3869a2b12ed246ae10bcce7d41e6e;0;6;48;FLOAT;0;False;49;FLOAT;0;False;96;FLOAT;1;False;94;FLOAT;1;False;95;FLOAT;1;False;97;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BlendNormalsNode;252;-1408,-80;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;203;-4608,-1216;Half;False;Property;_BaseBarkAO;Base Bark AO;2;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;310;-5376,-352;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;320;-1408,-1856;Inherit;False;319;Blend_Mask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;163;-1664,-1872;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;171;-1408,32;Inherit;False;168;Top_Mask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;309;-5376,-64;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;308;-5120,-128;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;158;-1152,-512;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;173;-1664,-1408;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;609;-4224,-1280;Inherit;False;458;AO_BaseTree;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldToObjectMatrix;824;-256,640;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;311;-5120,-512;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;222;-5120,0;Inherit;False;168;Top_Mask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;170;-1661.242,-1312;Inherit;False;168;Top_Mask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;766;-384,512;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;444;-4224,-1408;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;187;-1152,-2176;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;767;0,512;Inherit;False;2;2;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;608;-3968,-1408;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;217;-4864,-512;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;157;-896,-1440;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalizeNode;136;-896,-512;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;598;-640,-512;Half;False;Output_Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;223;-4608,-512;Half;False;Output_Smoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;207;-3712,-1408;Half;False;Output_AO;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;382;256,512;Half;False;Output_Wind;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;596;-640,-1440;Half;False;Output_Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;397;256,-1792;Inherit;False;382;Output_Wind;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;599;256,-2080;Inherit;False;598;Output_Normal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;602;256,-1888;Inherit;False;207;Output_AO;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;597;256,-2176;Inherit;False;596;Output_Albedo;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;224;256,-1984;Inherit;False;223;Output_Smoothness;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;640,-2176;Half;False;True;-1;2;;0;0;Standard;ANGRYMESH/Nature Pack/Standard/Tree Bark Snow;False;False;False;False;False;True;True;True;True;False;True;False;True;False;True;False;True;False;False;False;True;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;1;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;8.6;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;230;-5760,-640;Inherit;False;1537.111;100;;0;// Smoothness;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;237;-2688,-2304;Inherit;False;2311.763;100;;0;// Blend Albedo Maps;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;600;-5760,-1536;Inherit;False;2297.873;100;;0;// AO Maps;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;240;-2688,-640;Inherit;False;2301.541;100;;0;// Blend Normal Maps;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;210;-5760,-2304;Inherit;False;1405.242;100;;0;// AO VAlpha;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;241;-5760,384;Inherit;False;2431.659;100;;0;// Top World Mapping;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;892;256,-2304;Inherit;False;635.7682;100;;0;// Outputs;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;603;-2688,384;Inherit;False;3138.556;100;;0;// Vertex Wind;1,1,1,1;0;0
WireConnection;887;0;884;3
WireConnection;455;0;887;0
WireConnection;319;0;455;0
WireConnection;96;0;287;0
WireConnection;96;5;189;0
WireConnection;437;0;287;0
WireConnection;437;5;189;0
WireConnection;321;0;437;0
WireConnection;321;1;96;0
WireConnection;321;2;322;0
WireConnection;166;0;321;0
WireConnection;114;0;107;0
WireConnection;93;0;167;0
WireConnection;200;0;277;0
WireConnection;443;0;277;0
WireConnection;616;0;765;0
WireConnection;896;22;93;2
WireConnection;896;24;246;0
WireConnection;896;23;103;0
WireConnection;896;25;114;0
WireConnection;818;0;896;33
WireConnection;630;0;636;0
WireConnection;553;0;443;4
WireConnection;553;1;200;4
WireConnection;553;2;446;0
WireConnection;631;0;630;0
WireConnection;631;1;633;0
WireConnection;554;0;553;0
WireConnection;674;0;668;0
WireConnection;674;1;595;0
WireConnection;674;2;819;0
WireConnection;635;0;631;0
WireConnection;673;0;635;0
WireConnection;673;1;674;0
WireConnection;673;2;555;0
WireConnection;899;0;673;0
WireConnection;649;0;899;0
WireConnection;649;1;669;0
WireConnection;653;0;246;0
WireConnection;653;1;649;0
WireConnection;195;0;194;0
WireConnection;195;1;196;0
WireConnection;886;0;884;4
WireConnection;885;0;884;1
WireConnection;513;0;896;0
WireConnection;513;1;653;0
WireConnection;197;0;195;0
WireConnection;517;0;330;0
WireConnection;517;1;518;0
WireConnection;162;0;438;0
WireConnection;176;0;438;0
WireConnection;143;0;513;0
WireConnection;335;0;332;0
WireConnection;335;1;888;0
WireConnection;335;2;517;0
WireConnection;212;0;162;4
WireConnection;898;0;890;0
WireConnection;172;1;195;0
WireConnection;900;0;143;0
WireConnection;900;1;901;43
WireConnection;452;0;176;4
WireConnection;119;1;198;0
WireConnection;119;5;139;0
WireConnection;220;0;172;4
WireConnection;794;0;898;0
WireConnection;794;1;825;0
WireConnection;168;0;900;0
WireConnection;520;0;335;0
WireConnection;453;0;214;0
WireConnection;453;1;454;0
WireConnection;453;2;456;0
WireConnection;458;0;520;0
WireConnection;201;0;443;0
WireConnection;201;1;200;0
WireConnection;201;2;446;0
WireConnection;440;0;176;0
WireConnection;440;1;165;0
WireConnection;897;48;794;0
WireConnection;897;96;826;0
WireConnection;252;0;321;0
WireConnection;252;1;119;0
WireConnection;310;0;216;0
WireConnection;163;0;165;0
WireConnection;163;1;162;0
WireConnection;309;0;218;0
WireConnection;308;0;221;0
WireConnection;308;1;309;0
WireConnection;158;0;321;0
WireConnection;158;1;252;0
WireConnection;158;2;171;0
WireConnection;173;0;172;0
WireConnection;173;1;175;0
WireConnection;311;0;453;0
WireConnection;311;1;310;0
WireConnection;766;0;897;0
WireConnection;766;1;765;0
WireConnection;444;0;231;0
WireConnection;444;1;201;0
WireConnection;444;2;203;0
WireConnection;187;0;440;0
WireConnection;187;1;163;0
WireConnection;187;2;320;0
WireConnection;767;0;824;0
WireConnection;767;1;766;0
WireConnection;608;0;444;0
WireConnection;608;1;609;0
WireConnection;217;0;311;0
WireConnection;217;1;308;0
WireConnection;217;2;222;0
WireConnection;157;0;187;0
WireConnection;157;1;173;0
WireConnection;157;2;170;0
WireConnection;136;0;158;0
WireConnection;598;0;136;0
WireConnection;223;0;217;0
WireConnection;207;0;608;0
WireConnection;382;0;767;0
WireConnection;596;0;157;0
WireConnection;0;0;597;0
WireConnection;0;1;599;0
WireConnection;0;4;224;0
WireConnection;0;5;602;0
WireConnection;0;11;397;0
ASEEND*/
//CHKSM=E9A589D8EF79DB9D14B87193D23DE6B8F17B8469