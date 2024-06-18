// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ANGRYMESH/Nature Pack/Standard/Tree Cross Snow"
{
	Properties
	{
		_Cutoff("Alpha Cutoff", Range( 0 , 1)) = 0.5
		[Header(Base)]_Color("Base Color", Color) = (1,1,1,0)
		[NoScaleOffset]_MainTex("Base Albedo (A Opacity)", 2D) = "gray" {}
		[NoScaleOffset]_BaseSnowMask("Base Snow Mask", 2D) = "white" {}
		[Header(Top)]_TopIntensity("Top Intensity", Range( 0 , 1)) = 0
		_TopOffset("Top Offset", Range( 0 , 1)) = 0.5
		_TopContrast("Top Contrast", Range( 0 , 2)) = 1
		_TopColor("Top Color", Color) = (1,1,1,0)
		[Header(Tint Color)]_TintColor1("Tint Color 1", Color) = (1,1,1,0)
		_TintColor2("Tint Color 2", Color) = (1,1,1,0)
		_TintNoiseTile("Tint Noise Tile", Range( 0.001 , 30)) = 10
		[Header(Wind)]_WindAmplitude("Wind Amplitude", Range( 0 , 2)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			half4 vertexToFrag1097;
			half3 worldNormal;
			INTERNAL_DATA
			half4 vertexToFrag18_g75;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform sampler2D AG_TintNoiseTexture;
		uniform half _Cutoff;
		uniform half AGW_WindScale;
		uniform half AGW_WindSpeed;
		uniform half AGW_WindToggle;
		uniform half AGW_WindAmplitude;
		uniform half _WindAmplitude;
		uniform sampler2D _MainTex;
		uniform half4 _Color;
		uniform half4 _TintColor1;
		uniform half4 _TintColor2;
		uniform half AG_TintNoiseTile;
		uniform half _TintNoiseTile;
		uniform half AG_TintNoiseContrast;
		uniform half AG_TintToggle;
		uniform half4 _TopColor;
		uniform sampler2D _BaseSnowMask;
		uniform half _TopOffset;
		uniform half AGT_SnowOffset;
		uniform half _TopContrast;
		uniform half AGT_SnowContrast;
		uniform half _TopIntensity;
		uniform half AGT_SnowIntensity;
		uniform half AGH_SnowMinimumHeight;
		uniform half AGH_SnowFadeHeight;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			half mulTime10_g78 = _Time.y * AGW_WindSpeed;
			half temp_output_28_0_g78 = ( ( sin( ( ( ase_worldPos.z * AGW_WindScale ) + ( ( AGW_WindSpeed * _SinTime.z ) + mulTime10_g78 ) ) ) * ( AGW_WindToggle * AGW_WindAmplitude * _WindAmplitude ) ) * v.color.r );
			half3 appendResult63_g78 = (half3(temp_output_28_0_g78 , 0.0 , temp_output_28_0_g78));
			half3 Output_Wind548 = appendResult63_g78;
			v.vertex.xyz += Output_Wind548;
			v.vertex.w = 1;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			half3 ase_worldlightDir = 0;
			#else //aseld
			half3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			half3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			half3 normalizeResult1092 = normalize( ase_worldNormal );
			half dotResult1087 = dot( ase_worldlightDir , normalizeResult1092 );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			half4 ase_lightColor = 0;
			#else //aselc
			half4 ase_lightColor = _LightColor0;
			#endif //aselc
			o.vertexToFrag1097 = ( max( dotResult1087 , 0.0 ) * ase_lightColor );
			half4 temp_cast_0 = (1.0).xxxx;
			half2 appendResult8_g75 = (half2(ase_worldPos.x , ase_worldPos.z));
			half4 lerpResult17_g75 = lerp( _TintColor1 , _TintColor2 , saturate( ( tex2Dlod( AG_TintNoiseTexture, float4( ( appendResult8_g75 * ( 0.001 * AG_TintNoiseTile * _TintNoiseTile ) ), 0, 0.0) ).r * (0.001 + (AG_TintNoiseContrast - 0.001) * (60.0 - 0.001) / (10.0 - 0.001)) ) ));
			half4 lerpResult19_g75 = lerp( temp_cast_0 , lerpResult17_g75 , AG_TintToggle);
			o.vertexToFrag18_g75 = lerpResult19_g75;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float2 uv_MainTex162 = i.uv_texcoord;
			half4 tex2DNode162 = tex2D( _MainTex, uv_MainTex162 );
			half Output_OpacityMask317 = tex2DNode162.a;
			half3 ase_worldNormal = WorldNormalVector( i, half3( 0, 0, 1 ) );
			half3 normalizeResult1092 = normalize( ase_worldNormal );
			UnityGI gi1095 = gi;
			float3 diffNorm1095 = normalizeResult1092;
			gi1095 = UnityGI_Base( data, 1, diffNorm1095 );
			half3 indirectDiffuse1095 = gi1095.indirect.diffuse + diffNorm1095 * 0.0001;
			half4 TintColor1115 = i.vertexToFrag18_g75;
			float2 uv_BaseSnowMask1130 = i.uv_texcoord;
			half SnowMask1131 = tex2D( _BaseSnowMask, uv_BaseSnowMask1130 ).r;
			float3 ase_worldPos = i.worldPos;
			half Top_Mask168 = ( saturate( ( pow( abs( ( saturate( SnowMask1131 ) + ( _TopOffset * AGT_SnowOffset ) ) ) , ( (1.0 + (_TopContrast - 0.0) * (20.0 - 1.0) / (1.0 - 0.0)) * AGT_SnowContrast ) ) * ( _TopIntensity * AGT_SnowIntensity ) ) ) * saturate( (0.0 + (ase_worldPos.y - AGH_SnowMinimumHeight) * (1.0 - 0.0) / (( AGH_SnowMinimumHeight + AGH_SnowFadeHeight ) - AGH_SnowMinimumHeight)) ) );
			half4 lerpResult157 = lerp( ( _Color * tex2DNode162 * TintColor1115 ) , _TopColor , Top_Mask168);
			half4 Diffuse1074 = lerpResult157;
			half4 Output_LightingLambert1070 = ( ( ( i.vertexToFrag1097 * ase_lightAtten ) + half4( indirectDiffuse1095 , 0.0 ) ) * Diffuse1074 );
			c.rgb = Output_LightingLambert1070.rgb;
			c.a = 1;
			clip( Output_OpacityMask317 - _Cutoff );
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows novertexlights nolightmap  nodynlightmap nodirlightmap nometa dithercrossfade vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 customPack2 : TEXCOORD2;
				float4 customPack3 : TEXCOORD3;
				float4 tSpace0 : TEXCOORD4;
				float4 tSpace1 : TEXCOORD5;
				float4 tSpace2 : TEXCOORD6;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.customPack2.xyzw = customInputData.vertexToFrag1097;
				o.customPack3.xyzw = customInputData.vertexToFrag18_g75;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				surfIN.vertexToFrag1097 = IN.customPack2.xyzw;
				surfIN.vertexToFrag18_g75 = IN.customPack3.xyzw;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=18935
1920;78;1920;1019;4524.03;1429.552;1;True;False
Node;AmplifyShaderEditor.SamplerNode;1130;-2304,-128;Inherit;True;Property;_BaseSnowMask;Base Snow Mask;5;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;1131;-1920,-128;Half;False;SnowMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;107;-4736,-864;Half;False;Property;_TopContrast;Top Contrast;8;0;Create;True;0;0;0;False;0;False;1;2;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1132;-4736,-1152;Inherit;False;1131;SnowMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;114;-4352,-896;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;20;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;103;-4736,-960;Half;False;Property;_TopOffset;Top Offset;7;0;Create;True;0;0;0;False;0;False;0.5;0.293;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;246;-4736,-1056;Half;False;Property;_TopIntensity;Top Intensity;6;0;Create;True;0;0;0;False;1;Header(Top);False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1112;-4736,768;Half;False;Property;_TintNoiseTile;Tint Noise Tile;12;0;Create;True;0;0;0;False;0;False;10;6;0.001;30;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;1085;-2304,640;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ColorNode;1113;-4736,384;Half;False;Property;_TintColor1;Tint Color 1;10;0;Create;True;0;0;0;False;1;Header(Tint Color);False;1,1,1,0;1,1,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;1114;-4736,576;Half;False;Property;_TintColor2;Tint Color 2;11;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalizeNode;1092;-1920,704;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;1149;-4096,-992;Inherit;False;AG Global Snow - Height;-1;;77;792d553d9b3743f498843a42559debdb;0;0;1;FLOAT;43
Node;AmplifyShaderEditor.FunctionNode;1147;-4352,384;Inherit;False;AG Global Tint Color;0;;75;1dcc860732522ee469468f952b4e8aa1;0;3;27;COLOR;0,0,0,0;False;28;COLOR;0,0,0,0;False;22;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;1086;-2304,384;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.FunctionNode;1143;-4096,-1152;Inherit;False;AG Global Snow - Tree;-1;;76;0af770cdce085fc40bbf5b8250612a37;0;4;22;FLOAT;0;False;24;FLOAT;0;False;23;FLOAT;0;False;25;FLOAT;0;False;2;FLOAT;0;FLOAT;33
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1148;-3584,-1152;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1115;-3968,384;Half;False;TintColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DotProductOpNode;1087;-1792,384;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1117;-2304,-736;Inherit;False;1115;TintColor;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;1090;-1680,528;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMaxOpNode;1091;-1616,384;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;168;-3328,-1152;Half;False;Top_Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;162;-2304,-960;Inherit;True;Property;_MainTex;Base Albedo (A Opacity);4;1;[NoScaleOffset];Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;165;-2304,-1152;Half;False;Property;_Color;Base Color;3;0;Create;False;0;0;0;False;1;Header(Base);False;1,1,1,0;1,0.9632353,0.9632353,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;175;-1584,-1024;Half;False;Property;_TopColor;Top Color;9;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.9191176,0.9191176,0.9191176,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;163;-1920,-1152;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1094;-1408,384;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;170;-1568,-832;Inherit;False;168;Top_Mask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;157;-1280,-1152;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexToFragmentNode;1097;-1216,384;Inherit;False;False;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightAttenuation;1096;-1216,576;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.IndirectDiffuseLighting;1095;-1248,704;Inherit;False;World;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1098;-960,384;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1074;-1024,-1152;Half;False;Diffuse;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1122;-4736,-64;Half;False;Property;_WindAmplitude;Wind Amplitude;13;0;Create;True;0;0;0;False;1;Header(Wind);False;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1089;-768,384;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;842;-4736,-256;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1067;-800,576;Inherit;False;1074;Diffuse;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1093;-592,384;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;1142;-4352,-256;Inherit;False;AG Global Wind - Tree Cross;-1;;78;113fb90e51be7cd4cb8fd0e2ac81b3e8;0;2;48;FLOAT;0;False;96;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;317;-1920,-896;Half;False;Output_OpacityMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;548;-3968,-256;Half;False;Output_Wind;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1070;-384,384;Half;False;Output_LightingLambert;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;319;0,-1152;Inherit;False;317;Output_OpacityMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;549;0,-944;Inherit;False;548;Output_Wind;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1076;0,-1040;Inherit;False;1070;Output_LightingLambert;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1107;0,-832;Half;False;Property;_Cutoff;Alpha Cutoff;2;0;Create;False;0;0;0;True;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;384,-1152;Half;False;True;-1;2;;0;0;CustomLighting;ANGRYMESH/Nature Pack/Standard/Tree Cross Snow;False;False;False;False;False;True;True;True;True;False;True;False;True;False;True;False;True;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;8.6;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;5;False;-1;10;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;1107;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;889;-2304,256;Inherit;False;2170.888;100;;0;// Lambet Lighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1134;-4736,256;Inherit;False;1026.55;100;;0;// Tint Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1104;-4736,-384;Inherit;False;1021.611;100;;0;// Vertex Wind;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1106;-2304,-256;Inherit;False;641.0284;100;;0;// Top World Mapping;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1105;-2304,-1280;Inherit;False;1534.871;100;;0;// Diffuse;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;241;-4736,-1280;Inherit;False;1664.417;100;;0;// Top World Mapping;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1136;0,-1280;Inherit;False;635.7682;100;;0;// Outputs;1,1,1,1;0;0
WireConnection;1131;0;1130;1
WireConnection;114;0;107;0
WireConnection;1092;0;1085;0
WireConnection;1147;27;1113;0
WireConnection;1147;28;1114;0
WireConnection;1147;22;1112;0
WireConnection;1143;22;1132;0
WireConnection;1143;24;246;0
WireConnection;1143;23;103;0
WireConnection;1143;25;114;0
WireConnection;1148;0;1143;0
WireConnection;1148;1;1149;43
WireConnection;1115;0;1147;0
WireConnection;1087;0;1086;0
WireConnection;1087;1;1092;0
WireConnection;1091;0;1087;0
WireConnection;168;0;1148;0
WireConnection;163;0;165;0
WireConnection;163;1;162;0
WireConnection;163;2;1117;0
WireConnection;1094;0;1091;0
WireConnection;1094;1;1090;0
WireConnection;157;0;163;0
WireConnection;157;1;175;0
WireConnection;157;2;170;0
WireConnection;1097;0;1094;0
WireConnection;1095;0;1092;0
WireConnection;1098;0;1097;0
WireConnection;1098;1;1096;0
WireConnection;1074;0;157;0
WireConnection;1089;0;1098;0
WireConnection;1089;1;1095;0
WireConnection;1093;0;1089;0
WireConnection;1093;1;1067;0
WireConnection;1142;48;842;1
WireConnection;1142;96;1122;0
WireConnection;317;0;162;4
WireConnection;548;0;1142;0
WireConnection;1070;0;1093;0
WireConnection;0;10;319;0
WireConnection;0;13;1076;0
WireConnection;0;11;549;0
ASEEND*/
//CHKSM=B1E67E41B2C5DAD169F6C38414516620481553A5