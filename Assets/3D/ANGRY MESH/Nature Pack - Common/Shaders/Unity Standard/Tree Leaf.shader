// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ANGRYMESH/Nature Pack/Standard/Tree Leaf"
{
	Properties
	{
		_Cutoff("Alpha Cutoff", Range( 0 , 1)) = 0.5
		[Header(Base)]_Glossiness("Base Smoothness", Range( 0 , 1)) = 0.5
		_OcclusionStrength("Base Tree AO", Range( 0 , 1)) = 0.5
		_BumpScale("Base Normal Intensity", Range( 0 , 2)) = 1
		_Color("Base Color", Color) = (1,1,1,1)
		[NoScaleOffset]_MainTex("Base Albedo (A Opacity)", 2D) = "gray" {}
		[NoScaleOffset][Normal]_BumpMap("Base NormalMap", 2D) = "bump" {}
		[Header(Backface)]_BackFaceColor("Back Face Color", Color) = (1,1,1,0)
		[Header(Tint Color)]_TintColor1("Tint Color 1", Color) = (1,1,1,0)
		_TintColor2("Tint Color 2", Color) = (1,1,1,0)
		_TintNoiseTile("Tint Noise Tile", Range( 0.001 , 30)) = 10
		[Header(Translucency)]
		_Translucency("Strength", Range( 0 , 50)) = 1
		_TransNormalDistortion("Normal Distortion", Range( 0 , 1)) = 0.1
		_TransScattering("Scaterring Falloff", Range( 1 , 50)) = 2
		_TransDirect("Direct", Range( 0 , 1)) = 1
		_TransAmbient("Ambient", Range( 0 , 1)) = 0.2
		_TransShadow("Shadow", Range( 0 , 1)) = 0.9
		[Header(Wind Trunk (use common settings for both materials))]_WindTrunkAmplitude("Wind Trunk Amplitude", Range( 0 , 3)) = 1
		_WindTrunkStiffness("Wind Trunk Stiffness", Range( 1 , 3)) = 3
		[Header(Wind Leaf)]_WindLeafAmplitude("Wind Leaf Amplitude", Range( 0 , 3)) = 1
		_WindLeafSpeed("Wind Leaf Speed", Range( 0 , 10)) = 2
		_WindLeafScale("Wind Leaf Scale", Range( 0 , 30)) = 15
		_WindLeafStiffness("Wind Leaf Stiffness", Range( 0 , 2)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf StandardCustom keepalpha addshadow fullforwardshadows exclude_path:deferred novertexlights nolightmap  nodynlightmap nodirlightmap nometa dithercrossfade vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			half ASEVFace : VFACE;
			half4 vertexToFrag18_g118;
			float4 vertexColor : COLOR;
		};

		struct SurfaceOutputStandardCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			half3 Translucency;
		};

		uniform sampler2D AG_TintNoiseTexture;
		uniform half _Cutoff;
		uniform half AGW_WindScale;
		uniform half AGW_WindSpeed;
		uniform half AGW_WindToggle;
		uniform half _WindTrunkAmplitude;
		uniform half AGW_WindAmplitude;
		uniform half _WindTrunkStiffness;
		uniform half AGW_WindTreeStiffness;
		uniform half _WindLeafScale;
		uniform half _WindLeafSpeed;
		uniform half _WindLeafAmplitude;
		uniform half _WindLeafStiffness;
		uniform half3 AGW_WindDirection;
		uniform sampler2D _BumpMap;
		uniform half _BumpScale;
		uniform half4 _Color;
		uniform sampler2D _MainTex;
		uniform half4 _TintColor1;
		uniform half4 _TintColor2;
		uniform half AG_TintNoiseTile;
		uniform half _TintNoiseTile;
		uniform half AG_TintNoiseContrast;
		uniform half AG_TintToggle;
		uniform half4 _BackFaceColor;
		uniform half _Glossiness;
		uniform half _OcclusionStrength;
		uniform half AG_TreesAO;
		uniform half _Translucency;
		uniform half _TransNormalDistortion;
		uniform half _TransScattering;
		uniform half _TransDirect;
		uniform half _TransAmbient;
		uniform half _TransShadow;
		uniform half AG_TranslucencyIntensity;
		uniform half AG_TranslucencyDistance;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			half temp_output_99_0_g116 = ( 1.0 * AGW_WindScale );
			half temp_output_101_0_g116 = ( 1.0 * AGW_WindSpeed );
			half mulTime10_g116 = _Time.y * temp_output_101_0_g116;
			half temp_output_73_0_g116 = ( AGW_WindToggle * _WindTrunkAmplitude * AGW_WindAmplitude );
			half VColor_Red1622 = v.color.r;
			half temp_output_1428_0 = pow( abs( VColor_Red1622 ) , _WindTrunkStiffness );
			half temp_output_48_0_g116 = temp_output_1428_0;
			half temp_output_28_0_g116 = ( ( sin( ( ( ase_worldPos.z * temp_output_99_0_g116 ) + ( ( temp_output_101_0_g116 * _SinTime.z ) + mulTime10_g116 ) ) ) * temp_output_73_0_g116 ) * temp_output_48_0_g116 );
			half temp_output_49_0_g116 = 0.0;
			half3 appendResult63_g116 = (half3(temp_output_28_0_g116 , ( ( sin( ( ( temp_output_99_0_g116 * ase_worldPos.y ) + mulTime10_g116 ) ) * temp_output_73_0_g116 ) * temp_output_49_0_g116 ) , temp_output_28_0_g116));
			half3 Wind_Trunk1629 = ( appendResult63_g116 + ( temp_output_73_0_g116 * ( temp_output_48_0_g116 + temp_output_49_0_g116 ) * ( 1.0 * (1.0 + (AGW_WindTreeStiffness - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) ) ) );
			half temp_output_99_0_g117 = ( _WindLeafScale * AGW_WindScale );
			half temp_output_101_0_g117 = ( _WindLeafSpeed * AGW_WindSpeed );
			half mulTime10_g117 = _Time.y * temp_output_101_0_g117;
			half VColor_Blue1625 = v.color.b;
			half temp_output_73_0_g117 = ( AGW_WindToggle * ( VColor_Blue1625 * _WindLeafAmplitude ) * AGW_WindAmplitude );
			half Wind_HorizontalAnim1432 = temp_output_1428_0;
			half temp_output_48_0_g117 = Wind_HorizontalAnim1432;
			half temp_output_28_0_g117 = ( ( sin( ( ( ase_worldPos.z * temp_output_99_0_g117 ) + ( ( temp_output_101_0_g117 * _SinTime.z ) + mulTime10_g117 ) ) ) * temp_output_73_0_g117 ) * temp_output_48_0_g117 );
			half temp_output_49_0_g117 = VColor_Blue1625;
			half3 appendResult63_g117 = (half3(temp_output_28_0_g117 , ( ( sin( ( ( temp_output_99_0_g117 * ase_worldPos.y ) + mulTime10_g117 ) ) * temp_output_73_0_g117 ) * temp_output_49_0_g117 ) , temp_output_28_0_g117));
			half3 Wind_Leaf1630 = ( appendResult63_g117 + ( temp_output_73_0_g117 * ( temp_output_48_0_g117 + temp_output_49_0_g117 ) * ( (2.0 + (_WindLeafStiffness - 0.0) * (0.0 - 2.0) / (2.0 - 0.0)) * (1.0 + (AGW_WindTreeStiffness - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) ) ) );
			half3 Output_Wind548 = mul( unity_WorldToObject, half4( ( ( Wind_Trunk1629 + Wind_Leaf1630 ) * AGW_WindDirection ) , 0.0 ) ).xyz;
			v.vertex.xyz += Output_Wind548;
			v.vertex.w = 1;
			half4 temp_cast_2 = (1.0).xxxx;
			half2 appendResult8_g118 = (half2(ase_worldPos.x , ase_worldPos.z));
			half4 lerpResult17_g118 = lerp( _TintColor1 , _TintColor2 , saturate( ( tex2Dlod( AG_TintNoiseTexture, float4( ( appendResult8_g118 * ( 0.001 * AG_TintNoiseTile * _TintNoiseTile ) ), 0, 0.0) ).r * (0.001 + (AG_TintNoiseContrast - 0.001) * (60.0 - 0.001) / (10.0 - 0.001)) ) ));
			half4 lerpResult19_g118 = lerp( temp_cast_2 , lerpResult17_g118 , AG_TintToggle);
			o.vertexToFrag18_g118 = lerpResult19_g118;
		}

		inline half4 LightingStandardCustom(SurfaceOutputStandardCustom s, half3 viewDir, UnityGI gi )
		{
			#if !defined(DIRECTIONAL)
			float3 lightAtten = gi.light.color;
			#else
			float3 lightAtten = lerp( _LightColor0.rgb, gi.light.color, _TransShadow );
			#endif
			half3 lightDir = gi.light.dir + s.Normal * _TransNormalDistortion;
			half transVdotL = pow( saturate( dot( viewDir, -lightDir ) ), _TransScattering );
			half3 translucency = lightAtten * (transVdotL * _TransDirect + gi.indirect.diffuse * _TransAmbient) * s.Translucency;
			half4 c = half4( s.Albedo * translucency * _Translucency, 0 );

			SurfaceOutputStandard r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Metallic = s.Metallic;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandard (r, viewDir, gi) + c;
		}

		inline void LightingStandardCustom_GI(SurfaceOutputStandardCustom s, UnityGIInput data, inout UnityGI gi )
		{
			#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
				gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
			#else
				UNITY_GLOSSY_ENV_FROM_SURFACE( g, s, data );
				gi = UnityGlobalIllumination( data, s.Occlusion, s.Normal, g );
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandardCustom o )
		{
			float2 uv_BumpMap1127 = i.uv_texcoord;
			half3 break105_g119 = UnpackScaleNormal( tex2D( _BumpMap, uv_BumpMap1127 ), _BumpScale );
			half switchResult107_g119 = (((i.ASEVFace>0)?(break105_g119.z):(-break105_g119.z)));
			half3 appendResult108_g119 = (half3(break105_g119.x , break105_g119.y , switchResult107_g119));
			half3 normalizeResult136 = normalize( appendResult108_g119 );
			half3 Output_Normal1110 = normalizeResult136;
			o.Normal = Output_Normal1110;
			float2 uv_MainTex162 = i.uv_texcoord;
			half4 tex2DNode162 = tex2D( _MainTex, uv_MainTex162 );
			half4 TintColor1462 = i.vertexToFrag18_g118;
			half4 temp_output_163_0 = ( _Color * tex2DNode162 * TintColor1462 );
			half4 switchResult320 = (((i.ASEVFace>0)?(temp_output_163_0):(( temp_output_163_0 * _BackFaceColor ))));
			half4 Output_Albedo1053 = switchResult320;
			o.Albedo = Output_Albedo1053.rgb;
			half Output_Smoothness223 = (-1.0 + (_Glossiness - 0.0) * (1.0 - -1.0) / (1.0 - 0.0));
			o.Smoothness = Output_Smoothness223;
			half VColor_Alpha1626 = i.vertexColor.a;
			half lerpResult201 = lerp( 1.0 , VColor_Alpha1626 , ( _OcclusionStrength * AG_TreesAO ));
			half Output_AO207 = saturate( lerpResult201 );
			o.Occlusion = Output_AO207;
			float3 ase_worldPos = i.worldPos;
			half lerpResult14_g120 = lerp( 0.0 , AG_TranslucencyIntensity , saturate( ( ( 1.0 - (0.0 + (distance( ase_worldPos , _WorldSpaceCameraPos ) - 0.0) * (1.0 - 0.0) / (AG_TranslucencyDistance - 0.0)) ) * VColor_Alpha1626 ) ));
			half Output_Transluncency1126 = lerpResult14_g120;
			half3 temp_cast_1 = (Output_Transluncency1126).xxx;
			o.Translucency = temp_cast_1;
			o.Alpha = 1;
			half Alpha_Albedo317 = tex2DNode162.a;
			half Alpha_Color1103 = _Color.a;
			half Output_OpacityMask1642 = ( Alpha_Albedo317 * Alpha_Color1103 );
			clip( Output_OpacityMask1642 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=18935
1920;78;1920;1019;7580.522;3276.283;5.883889;True;False
Node;AmplifyShaderEditor.VertexColorNode;1621;-5248,640;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;1622;-4992,640;Half;False;VColor_Red;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1627;-4608,640;Inherit;False;1622;VColor_Red;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;1662;-4352,640;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1484;-4608,736;Half;False;Property;_WindTrunkStiffness;Wind Trunk Stiffness;21;0;Create;True;0;0;0;False;0;False;3;2;1;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1625;-4992,736;Half;False;VColor_Blue;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;1428;-4224,640;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1391;-3200,832;Half;False;Property;_WindLeafAmplitude;Wind Leaf Amplitude;22;0;Create;True;0;0;0;False;1;Header(Wind Leaf);False;1;1;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1432;-3968,640;Half;False;Wind_HorizontalAnim;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1628;-3200,736;Inherit;False;1625;VColor_Blue;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1442;-3200,1120;Half;False;Property;_WindLeafStiffness;Wind Leaf Stiffness;25;0;Create;True;0;0;0;False;0;False;0;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1485;-4608,832;Half;False;Property;_WindTrunkAmplitude;Wind Trunk Amplitude;20;0;Create;True;0;0;0;False;1;Header(Wind Trunk (use common settings for both materials));False;1;1;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1389;-3200,928;Half;False;Property;_WindLeafScale;Wind Leaf Scale;24;0;Create;True;0;0;0;False;0;False;15;15;0;30;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;1571;-2816,1024;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;2;False;3;FLOAT;2;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1458;-5248,-576;Half;False;Property;_TintColor2;Tint Color 2;11;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1390;-3200,1024;Half;False;Property;_WindLeafSpeed;Wind Leaf Speed;23;0;Create;True;0;0;0;False;0;False;2;2;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1433;-3200,640;Inherit;False;1432;Wind_HorizontalAnim;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1459;-5248,-384;Half;False;Property;_TintNoiseTile;Tint Noise Tile;12;0;Create;True;0;0;0;False;0;False;10;10;0.001;30;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1457;-5248,-768;Half;False;Property;_TintColor1;Tint Color 1;10;0;Create;True;0;0;0;False;1;Header(Tint Color);False;1,1,1,0;1,1,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1400;-2816,768;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1656;-3968,768;Inherit;False;AG Global Wind - Tree;-1;;116;b3a3869a2b12ed246ae10bcce7d41e6e;0;6;48;FLOAT;0;False;49;FLOAT;0;False;96;FLOAT;1;False;94;FLOAT;1;False;95;FLOAT;1;False;97;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;1655;-2560,640;Inherit;False;AG Global Wind - Tree;-1;;117;b3a3869a2b12ed246ae10bcce7d41e6e;0;6;48;FLOAT;0;False;49;FLOAT;0;False;96;FLOAT;1;False;94;FLOAT;1;False;95;FLOAT;1;False;97;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;1660;-4864,-768;Inherit;False;AG Global Tint Color;0;;118;1dcc860732522ee469468f952b4e8aa1;0;3;27;COLOR;0,0,0,0;False;28;COLOR;0,0,0,0;False;22;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1629;-3584,768;Half;False;Wind_Trunk;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1462;-4480,-768;Half;False;TintColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1630;-2176,640;Half;False;Wind_Leaf;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;914;-5248,-1888;Half;False;Global;AG_TreesAO;AG_TreesAO;21;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1463;-2814.538,-1760;Inherit;False;1462;TintColor;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;289;-2816,-1280;Half;False;Property;_BumpScale;Base Normal Intensity;5;0;Create;False;0;0;0;False;0;False;1;2;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;165;-2816,-2176;Half;False;Property;_Color;Base Color;6;0;Create;False;0;0;0;False;0;False;1,1,1,1;1,1,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;203;-5248,-1984;Half;False;Property;_OcclusionStrength;Base Tree AO;4;0;Create;False;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1626;-4992,832;Half;False;VColor_Alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1631;-1792,640;Inherit;False;1629;Wind_Trunk;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1632;-1792,736;Inherit;False;1630;Wind_Leaf;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;162;-2816,-1984;Inherit;True;Property;_MainTex;Base Albedo (A Opacity);7;1;[NoScaleOffset];Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1127;-2432,-1280;Inherit;True;Property;_BumpMap;Base NormalMap;8;2;[NoScaleOffset];[Normal];Create;False;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;913;-4864,-2000;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1636;-5248,-2080;Inherit;False;1626;VColor_Alpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;231;-5248,-2176;Half;False;Constant;_White1;White1;19;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;163;-2432,-2048;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1396;-1536,640;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;317;-2432,-1920;Half;False;Alpha_Albedo;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;332;-2432,-1792;Half;False;Property;_BackFaceColor;Back Face Color;9;0;Create;True;0;0;0;False;1;Header(Backface);False;1,1,1,0;1,1,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;1103;-2432,-2176;Half;False;Alpha_Color;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;1374;-1536,768;Half;False;Global;AGW_WindDirection;AGW_WindDirection;20;0;Create;True;0;0;0;False;0;False;0,0,0;0.9382213,-0.3195158,-0.1328554;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;363;-2048,-2000;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;319;-5248,128;Inherit;False;317;Alpha_Albedo;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1663;-2048,-1280;Inherit;False;AG Flip Normals;-1;;119;02ae90bb716acd647b8ac9db8603316a;0;1;110;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;216;-5248,-1280;Half;False;Property;_Glossiness;Base Smoothness;3;0;Create;False;0;0;0;False;1;Header(Base);False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1105;-5248,224;Inherit;False;1103;Alpha_Color;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;201;-4736,-2176;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1373;-1280,640;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldToObjectMatrix;1376;-1280,768;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.GetLocalVarNode;1637;-2816,-768;Inherit;False;1626;VColor_Alpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1375;-1024,640;Inherit;False;2;2;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;1653;-2560,-768;Inherit;False;AG Global Transluncency;-1;;120;478210ebafb74104ba79a094e66bfe96;0;1;15;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;136;-1808,-1280;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1099;-4992,128;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;713;-4864,-1280;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;915;-4480,-2176;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwitchByFaceNode;320;-1920,-2176;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1126;-2176,-768;Half;False;Output_Transluncency;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;548;-768,640;Half;False;Output_Wind;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;207;-4224,-2176;Half;False;Output_AO;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1642;-4736,128;Half;False;Output_OpacityMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1053;-1664,-2176;Half;False;Output_Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;223;-4608,-1280;Half;False;Output_Smoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1110;-1536,-1280;Half;False;Output_Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;224;0,-1984;Inherit;False;223;Output_Smoothness;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;208;0,-1888;Inherit;False;207;Output_AO;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1115;0,-1504;Half;False;Property;_Cutoff;Alpha Cutoff;2;0;Create;False;0;0;0;True;0;False;0.5;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1638;0,-1696;Inherit;False;1642;Output_OpacityMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1109;0,-2176;Inherit;False;1053;Output_Albedo;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1111;0,-2080;Inherit;False;1110;Output_Normal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;549;0,-1600;Inherit;False;548;Output_Wind;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;899;0,-1792;Inherit;False;1126;Output_Transluncency;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;640,-2176;Half;False;True;-1;2;;0;0;Standard;ANGRYMESH/Nature Pack/Standard/Tree Leaf;False;False;False;False;False;True;True;True;True;False;True;False;True;False;True;False;True;False;False;False;True;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.3;True;True;0;False;TransparentCutout;;AlphaTest;ForwardOnly;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;8.6;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;5;False;-1;10;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;13;-1;-1;0;False;0;0;False;-1;-1;0;True;1115;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;1453;-5248,-896;Inherit;False;1020.68;100;;0;// Tint Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1112;-2816,-896;Inherit;False;1017.246;100;;0;// Translucency;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1641;-5248,0;Inherit;False;769.246;100;;0;// Alpha Cutout;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;237;-2816,-2304;Inherit;False;1406.922;100;;0;// Albedo Map;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;240;-2816,-1408;Inherit;False;1533.996;100;;0;// Normal Map;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;889;-5248,512;Inherit;False;4733.918;100;;0;// Vertex Wind;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;230;-5248,-1408;Inherit;False;894.165;100;;0;// Smoothness;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1643;0,-2304;Inherit;False;894.5891;100;;0;// Outputs;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;210;-5248,-2304;Inherit;False;1280.537;100;;0;// AO;1,1,1,1;0;0
WireConnection;1622;0;1621;1
WireConnection;1662;0;1627;0
WireConnection;1625;0;1621;3
WireConnection;1428;0;1662;0
WireConnection;1428;1;1484;0
WireConnection;1432;0;1428;0
WireConnection;1571;0;1442;0
WireConnection;1400;0;1628;0
WireConnection;1400;1;1391;0
WireConnection;1656;48;1428;0
WireConnection;1656;96;1485;0
WireConnection;1655;48;1433;0
WireConnection;1655;49;1628;0
WireConnection;1655;96;1400;0
WireConnection;1655;94;1389;0
WireConnection;1655;95;1390;0
WireConnection;1655;97;1571;0
WireConnection;1660;27;1457;0
WireConnection;1660;28;1458;0
WireConnection;1660;22;1459;0
WireConnection;1629;0;1656;0
WireConnection;1462;0;1660;0
WireConnection;1630;0;1655;0
WireConnection;1626;0;1621;4
WireConnection;1127;5;289;0
WireConnection;913;0;203;0
WireConnection;913;1;914;0
WireConnection;163;0;165;0
WireConnection;163;1;162;0
WireConnection;163;2;1463;0
WireConnection;1396;0;1631;0
WireConnection;1396;1;1632;0
WireConnection;317;0;162;4
WireConnection;1103;0;165;4
WireConnection;363;0;163;0
WireConnection;363;1;332;0
WireConnection;1663;110;1127;0
WireConnection;201;0;231;0
WireConnection;201;1;1636;0
WireConnection;201;2;913;0
WireConnection;1373;0;1396;0
WireConnection;1373;1;1374;0
WireConnection;1375;0;1376;0
WireConnection;1375;1;1373;0
WireConnection;1653;15;1637;0
WireConnection;136;0;1663;0
WireConnection;1099;0;319;0
WireConnection;1099;1;1105;0
WireConnection;713;0;216;0
WireConnection;915;0;201;0
WireConnection;320;0;163;0
WireConnection;320;1;363;0
WireConnection;1126;0;1653;0
WireConnection;548;0;1375;0
WireConnection;207;0;915;0
WireConnection;1642;0;1099;0
WireConnection;1053;0;320;0
WireConnection;223;0;713;0
WireConnection;1110;0;136;0
WireConnection;0;0;1109;0
WireConnection;0;1;1111;0
WireConnection;0;4;224;0
WireConnection;0;5;208;0
WireConnection;0;7;899;0
WireConnection;0;10;1638;0
WireConnection;0;11;549;0
ASEEND*/
//CHKSM=FC4014D43793A997B761A3CB1696F12214C89931