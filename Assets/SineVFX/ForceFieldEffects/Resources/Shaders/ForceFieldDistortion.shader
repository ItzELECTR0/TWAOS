// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SineVFX/ForceFieldAndShieldEffects/ForceFieldDistortion"
{
	Properties
	{
		_MaskFresnelExp("Mask Fresnel Exp", Range( 0.2 , 8)) = 4
		[Toggle] _MaskDepthFadeEnabled("Mask Depth Fade Enabled", Float) = 1.0
		_MaskDepthFadeDistance("Mask Depth Fade Distance", Float) = 6
		_MaskDepthFadeExp("Mask Depth Fade Exp", Range( 0.2 , 10)) = 4
		[Toggle] _MaskSoftBorders("Mask Soft Borders", Float) = 0.0
		_MaskSoftBordersMultiply("Mask Soft Borders Multiply", Float) = 1.01
		_MaskSoftBordersExp("Mask Soft Borders Exp", Float) = 20
		_NormalVertexOffset("Normal Vertex Offset", Float) = 0
		[Toggle] _LocalNoisePosition("Local Noise Position", Float) = 0.0
		_NoiseMaskPower("Noise Mask Power", Range( 0 , 10)) = 1
		_Noise01("Noise 01", 2D) = "white" {}
		_Noise01Tiling("Noise 01 Tiling", Float) = 1
		_Noise01ScrollSpeed("Noise 01 Scroll Speed", Float) = 0.25
		[Toggle] _Noise02Enabled("Noise 02 Enabled", Float) = 0.0
		_Noise02("Noise 02", 2D) = "white" {}
		_Noise02Tiling("Noise 02 Tiling", Float) = 1
		_Noise02ScrollSpeed("Noise 02 Scroll Speed", Float) = 0.25
		[Toggle] _NoiseDistortionEnabled("Noise Distortion Enabled", Float) = 1.0
		_NoiseDistortion("Noise Distortion", 2D) = "white" {}
		_NoiseDistortionPower("Noise Distortion Power", Range( 0 , 2)) = 0.5
		_NoiseDistortionTiling("Noise Distortion Tiling", Float) = 0.5
		_ScreenDistortionPower("Screen Distortion Power", Range( 0 , 0.2)) = 0.025
		_MaskAppearLocalYRemap("Mask Appear Local Y Remap", Float) = 0.5
		_MaskAppearLocalYAdd("Mask Appear Local Y Add", Float) = 0
		[Toggle] _MaskAppearInvert("Mask Appear Invert", Float) = 0.0
		_MaskAppearProgress("Mask Appear Progress", Range( -2 , 2)) = 0
		_MaskAppearNoise("Mask Appear Noise", 2D) = "white" {}
		_MaskAppearRamp("Mask Appear Ramp", 2D) = "white" {}
		[Toggle] _MaskAppearNoiseTriplanar("Mask Appear Noise Triplanar", Float) = 0.0
		_MaskAppearNoiseTriplanarTiling("Mask Appear Noise Triplanar Tiling", Float) = 0.2
		[Toggle] _MaskAppearUseWorldPosition("Mask Appear Use World Position", Float) = 0.0
		[Toggle] _VisualizeMaskDebug("Visualize Mask Debug", Float) = 0.0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ }
		CGPROGRAM
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma shader_feature _MASKDEPTHFADEENABLED_ON
		#pragma shader_feature _MASKSOFTBORDERS_ON
		#pragma shader_feature _LOCALNOISEPOSITION_ON
		#pragma shader_feature _NOISEDISTORTIONENABLED_ON
		#pragma shader_feature _NOISE02ENABLED_ON
		#pragma shader_feature _VISUALIZEMASKDEBUG_ON
		#pragma shader_feature _MASKAPPEARUSEWORLDPOSITION_ON
		#pragma shader_feature _MASKAPPEARINVERT_ON
		#pragma shader_feature _MASKAPPEARNOISETRIPLANAR_ON
		#pragma surface surf Unlit alpha:fade keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nometa noforwardadd vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			fixed ASEVFace : VFACE;
			float4 screenPos;
			float2 uv_texcoord;
		};

		uniform float _MaskFresnelExp;
		uniform sampler2D _CameraDepthTexture;
		uniform float _MaskDepthFadeDistance;
		uniform float _MaskDepthFadeExp;
		uniform float _MaskSoftBordersMultiply;
		uniform float _MaskSoftBordersExp;
		uniform sampler2D _GrabTexture;
		uniform sampler2D _Noise01;
		uniform float _Noise01Tiling;
		uniform float _Noise01ScrollSpeed;
		uniform sampler2D _NoiseDistortion;
		uniform float _NoiseDistortionTiling;
		uniform float _NoiseDistortionPower;
		uniform sampler2D _Noise02;
		uniform float _Noise02Tiling;
		uniform float _Noise02ScrollSpeed;
		uniform float _NoiseMaskPower;
		uniform float _ScreenDistortionPower;
		uniform sampler2D _MaskAppearRamp;
		uniform float _MaskAppearLocalYRemap;
		uniform float _MaskAppearLocalYAdd;
		uniform float _MaskAppearProgress;
		uniform sampler2D _MaskAppearNoise;
		uniform float _MaskAppearNoiseTriplanarTiling;
		uniform float4 _MaskAppearNoise_ST;
		uniform float _NormalVertexOffset;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		inline float4 TriplanarSamplingSF( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float tilling, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= projNormal.x + projNormal.y + projNormal.z;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = ( tex2D( topTexMap, tilling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = ( tex2D( topTexMap, tilling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			zNorm = ( tex2D( topTexMap, tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			v.vertex.xyz += ( ase_worldNormal * _NormalVertexOffset );
		}

		inline fixed4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return fixed4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Normal = float3(0,0,1);
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 switchResult27 = (((i.ASEVFace>0)?(float3(0,0,1)):(float3(0,0,-1))));
			float dotResult1 = dot( ase_worldViewDir , WorldNormalVector( i , switchResult27 ) );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth11 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth11 = abs( ( screenDepth11 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _MaskDepthFadeDistance ) );
			float clampResult19 = clamp( ( 1.0 - distanceDepth11 ) , 0.0 , 1.0 );
			#ifdef _MASKDEPTHFADEENABLED_ON
				float staticSwitch167 = pow( clampResult19 , _MaskDepthFadeExp );
			#else
				float staticSwitch167 = 0.0;
			#endif
			float temp_output_10_0 = max( pow( ( 1.0 - dotResult1 ) , _MaskFresnelExp ) , staticSwitch167 );
			float clampResult80 = clamp( temp_output_10_0 , 0.0 , 1.0 );
			float clampResult125 = clamp( ( clampResult80 - pow( ( clampResult80 * _MaskSoftBordersMultiply ) , _MaskSoftBordersExp ) ) , 0.0 , 1.0 );
			#ifdef _MASKSOFTBORDERS_ON
				float staticSwitch128 = clampResult125;
			#else
				float staticSwitch128 = clampResult80;
			#endif
			float ResultOpacity93 = staticSwitch128;
			float4 temp_cast_0 = (ResultOpacity93).xxxx;
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 temp_output_57_0 = abs( ase_worldNormal );
			float3 temp_output_60_0 = ( temp_output_57_0 * temp_output_57_0 );
			float4 transform130 = mul(unity_ObjectToWorld,float4(0,0,0,1));
			float3 appendResult132 = (float3(transform130.x , transform130.y , transform130.z));
			#ifdef _LOCALNOISEPOSITION_ON
				float3 staticSwitch134 = ( ase_worldPos - appendResult132 );
			#else
				float3 staticSwitch134 = ase_worldPos;
			#endif
			float3 FinalWorldPosition135 = staticSwitch134;
			float4 triplanar34 = TriplanarSamplingSF( _NoiseDistortion, ase_worldPos, ase_worldNormal, 1.0, _NoiseDistortionTiling, 0 );
			float4 temp_cast_2 = (0.0).xxxx;
			#ifdef _NOISEDISTORTIONENABLED_ON
				float4 staticSwitch42 = ( triplanar34 * _NoiseDistortionPower );
			#else
				float4 staticSwitch42 = temp_cast_2;
			#endif
			float2 appendResult64 = (float2(( float4( ( FinalWorldPosition135 * _Noise01Tiling ) , 0.0 ) + ( _Time.y * _Noise01ScrollSpeed ) + staticSwitch42 ).y , ( float4( ( FinalWorldPosition135 * _Noise01Tiling ) , 0.0 ) + ( _Time.y * _Noise01ScrollSpeed ) + staticSwitch42 ).z));
			float2 appendResult65 = (float2(( float4( ( FinalWorldPosition135 * _Noise01Tiling ) , 0.0 ) + ( _Time.y * _Noise01ScrollSpeed ) + staticSwitch42 ).z , ( float4( ( FinalWorldPosition135 * _Noise01Tiling ) , 0.0 ) + ( _Time.y * _Noise01ScrollSpeed ) + staticSwitch42 ).x));
			float2 appendResult67 = (float2(( float4( ( FinalWorldPosition135 * _Noise01Tiling ) , 0.0 ) + ( _Time.y * _Noise01ScrollSpeed ) + staticSwitch42 ).x , ( float4( ( FinalWorldPosition135 * _Noise01Tiling ) , 0.0 ) + ( _Time.y * _Noise01ScrollSpeed ) + staticSwitch42 ).y));
			float3 weightedBlendVar73 = temp_output_60_0;
			float weightedBlend73 = ( weightedBlendVar73.x*UnpackNormal( tex2D( _Noise01, appendResult64 ) ).r + weightedBlendVar73.y*UnpackNormal( tex2D( _Noise01, appendResult65 ) ).r + weightedBlendVar73.z*UnpackNormal( tex2D( _Noise01, appendResult67 ) ).r );
			float2 appendResult59 = (float2(( float4( ( FinalWorldPosition135 * _Noise02Tiling ) , 0.0 ) + ( _Time.y * _Noise02ScrollSpeed ) + staticSwitch42 ).y , ( float4( ( FinalWorldPosition135 * _Noise02Tiling ) , 0.0 ) + ( _Time.y * _Noise02ScrollSpeed ) + staticSwitch42 ).z));
			float2 appendResult56 = (float2(( float4( ( FinalWorldPosition135 * _Noise02Tiling ) , 0.0 ) + ( _Time.y * _Noise02ScrollSpeed ) + staticSwitch42 ).z , ( float4( ( FinalWorldPosition135 * _Noise02Tiling ) , 0.0 ) + ( _Time.y * _Noise02ScrollSpeed ) + staticSwitch42 ).x));
			float2 appendResult54 = (float2(( float4( ( FinalWorldPosition135 * _Noise02Tiling ) , 0.0 ) + ( _Time.y * _Noise02ScrollSpeed ) + staticSwitch42 ).x , ( float4( ( FinalWorldPosition135 * _Noise02Tiling ) , 0.0 ) + ( _Time.y * _Noise02ScrollSpeed ) + staticSwitch42 ).y));
			float3 weightedBlendVar69 = temp_output_60_0;
			float weightedBlend69 = ( weightedBlendVar69.x*UnpackNormal( tex2D( _Noise02, appendResult59 ) ).r + weightedBlendVar69.y*UnpackNormal( tex2D( _Noise02, appendResult56 ) ).r + weightedBlendVar69.z*UnpackNormal( tex2D( _Noise02, appendResult54 ) ).r );
			#ifdef _NOISE02ENABLED_ON
				float staticSwitch75 = weightedBlend69;
			#else
				float staticSwitch75 = 1.0;
			#endif
			float ResultNoise77 = ( weightedBlend73 * staticSwitch75 * _NoiseMaskPower );
			float4 screenColor97 = tex2D( _GrabTexture, ( ase_grabScreenPosNorm + ( ResultNoise77 * _ScreenDistortionPower * ResultOpacity93 ) ).xy );
			#ifdef _VISUALIZEMASKDEBUG_ON
				float4 staticSwitch116 = temp_cast_0;
			#else
				float4 staticSwitch116 = screenColor97;
			#endif
			o.Emission = staticSwitch116.rgb;
			float3 ase_objectScale = float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			#ifdef _MASKAPPEARUSEWORLDPOSITION_ON
				float staticSwitch162 = ( FinalWorldPosition135.y / ase_objectScale.y );
			#else
				float staticSwitch162 = ase_vertex3Pos.y;
			#endif
			float temp_output_146_0 = abs( ( (0.0 + (staticSwitch162 - 0.0) * (1.0 - 0.0) / (_MaskAppearLocalYRemap - 0.0)) + _MaskAppearLocalYAdd ) );
			#ifdef _MASKAPPEARINVERT_ON
				float staticSwitch153 = ( 1.0 - temp_output_146_0 );
			#else
				float staticSwitch153 = temp_output_146_0;
			#endif
			float4 triplanar148 = TriplanarSamplingSF( _MaskAppearNoise, ase_worldPos, ase_worldNormal, 1.0, _MaskAppearNoiseTriplanarTiling, 0 );
			float2 uv_MaskAppearNoise = i.uv_texcoord * _MaskAppearNoise_ST.xy + _MaskAppearNoise_ST.zw;
			#ifdef _MASKAPPEARNOISETRIPLANAR_ON
				float staticSwitch149 = triplanar148.x;
			#else
				float staticSwitch149 = tex2D( _MaskAppearNoise, uv_MaskAppearNoise ).r;
			#endif
			float2 appendResult156 = (float2(( staticSwitch153 + _MaskAppearProgress + -staticSwitch149 ) , 0.0));
			float MaskAppearValue159 = tex2D( _MaskAppearRamp, appendResult156 ).g;
			float clampResult161 = clamp( MaskAppearValue159 , 0.0 , 1.0 );
			o.Alpha = clampResult161;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14201
7;29;1906;1004;4655.582;1221.283;3.046208;True;True
Node;AmplifyShaderEditor.Vector4Node;129;-5085.167,835.3986;Float;False;Constant;_Vector2;Vector 2;22;0;Create;0,0,0,1;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;30;-3139.04,114.9353;Float;False;Constant;_Vector1;Vector 1;3;0;Create;0,0,-1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;29;-3134.244,-49.72601;Float;False;Constant;_Vector0;Vector 0;3;0;Create;0,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;130;-4898.257,836.7032;Float;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;13;-3316.347,386.8741;Float;False;Property;_MaskDepthFadeDistance;Mask Depth Fade Distance;2;0;Create;6;12;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SwitchByFaceNode;27;-2916.827,44.59443;Float;False;2;0;FLOAT3;0.0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldPosInputsNode;131;-4890.282,692.9833;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DepthFade;11;-3044.346,388.8741;Float;False;True;1;0;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;132;-4689.132,838.3857;Float;False;FLOAT3;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;31;-3604.521,1422.811;Float;True;Property;_NoiseDistortion;Noise Distortion;19;0;Create;None;faa1d2cf5dd834f4aa79ca0e14008434;False;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;133;-4333.688,805.3687;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;3;-2733.878,-265.5228;Float;False;World;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;8;-2741.878,42.47702;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.OneMinusNode;12;-2849.346,387.8741;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-3620.341,1615.138;Float;False;Property;_NoiseDistortionTiling;Noise Distortion Tiling;21;0;Create;0.5;0.075;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;34;-3267.413,1472.142;Float;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;0;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;False;8;0;SAMPLER2D;;False;5;FLOAT;1.0;False;1;SAMPLER2D;;False;6;FLOAT;0.0;False;2;SAMPLER2D;;False;7;FLOAT;0.0;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;33;-3182.934,1666.395;Float;False;Property;_NoiseDistortionPower;Noise Distortion Power;20;0;Create;0.5;2;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;1;-2389.876,-173.5228;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;19;-2687.492,388.5258;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-2809.876,508.0444;Float;False;Property;_MaskDepthFadeExp;Mask Depth Fade Exp;3;0;Create;4;2;0.2;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;134;-4127.17,614.1926;Float;False;Property;_LocalNoisePosition;Local Noise Position;8;0;Create;0;False;True;True;;Toggle;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0.0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;137;-2764.385,1888.337;Float;False;135;0;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-2737.115,2276.125;Float;False;Property;_Noise02ScrollSpeed;Noise 02 Scroll Speed;17;0;Create;0.25;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-2709.789,2035.087;Float;False;Property;_Noise02Tiling;Noise 02 Tiling;16;0;Create;1;0.35;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;135;-3840.824,614.9592;Float;False;FinalWorldPosition;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TimeNode;35;-2709.356,2131.524;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;7;-2390.876,-78.52291;Float;False;Property;_MaskFresnelExp;Mask Fresnel Exp;0;0;Create;4;0.2;0.2;8;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;15;-2489.671,436.0445;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;168;-2485.079,615.9754;Float;False;Constant;_Float0;Float 0;33;0;Create;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;5;-2265.875,-170.5229;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-2851.934,1575.395;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;165;1087.537,1650.781;Float;False;135;0;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-2853.14,1677.703;Float;False;Constant;_Float3;Float 3;23;0;Create;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-2747.957,1105.439;Float;False;Property;_Noise01Tiling;Noise 01 Tiling;12;0;Create;1;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-2391.312,2189.024;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-2395.264,1947.28;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;136;-2801.013,939.8776;Float;False;135;0;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-2771.651,1348.873;Float;False;Property;_Noise01ScrollSpeed;Noise 01 Scroll Speed;13;0;Create;0.25;0.25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;164;1330.037,1656.381;Float;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.StaticSwitch;167;-2286.338,501.2999;Float;False;Property;_MaskDepthFadeEnabled;Mask Depth Fade Enabled;1;0;Create;0;True;False;True;;Toggle;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;42;-2636.041,1614.004;Float;False;Property;_NoiseDistortionEnabled;Noise Distortion Enabled;18;0;Create;0;True;True;True;;Toggle;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ObjectScaleNode;166;1343.037,1882.378;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PowerNode;6;-2062.875,-134.5229;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;45;-2747.524,1199.276;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;48;-2192.434,2063.937;Float;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0,0,0;False;2;FLOAT4;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-2433.435,1015.032;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-2429.483,1256.776;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;163;1667.636,1763.865;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;139;1604.757,1892.75;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMaxOpNode;10;-1791.116,87.15874;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;51;-2013.837,2065.614;Float;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;138;1789.083,2185.887;Float;False;Property;_MaskAppearLocalYRemap;Mask Appear Local Y Remap;23;0;Create;0.5;6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;52;-1304.034,1671.071;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ClampOpNode;80;-1289.148,188.9989;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;127;-1427.053,438.101;Float;False;Property;_MaskSoftBordersMultiply;Mask Soft Borders Multiply;5;0;Create;1.01;1.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;53;-2230.604,1131.689;Float;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0,0,0;False;2;FLOAT4;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StaticSwitch;162;1851.936,1812.683;Float;False;Property;_MaskAppearUseWorldPosition;Mask Appear Use World Position;31;0;Create;0;False;True;True;;Toggle;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;56;-1515.999,2086.724;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;59;-1517.379,1932.391;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;54;-1514.62,2239.681;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;123;-1264.202,571.6525;Float;False;Property;_MaskSoftBordersExp;Mask Soft Borders Exp;6;0;Create;20;20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;-1129.729,362.9962;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;58;-2058.817,1124.857;Float;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.AbsOpNode;57;-1080.388,1672.338;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;55;-1591.566,2339.09;Float;True;Property;_Noise02;Noise 02;15;0;Create;None;393ed426cda7a7c4d8881647fbf577a7;True;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;140;2014.197,2283.544;Float;False;Property;_MaskAppearLocalYAdd;Mask Appear Local Y Add;24;0;Create;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;141;2112.316,2017.454;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;63;-1252.089,1896.542;Float;True;Property;_TextureSample2;Texture Sample 2;15;0;Create;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;61;-1252.09,2091.542;Float;True;Property;_TextureSample0;Texture Sample 0;15;0;Create;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;62;-1246.89,2296.943;Float;True;Property;_TextureSample1;Texture Sample 1;15;0;Create;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;142;1540.937,2515.655;Float;False;0;143;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;143;1576.571,2654.796;Float;True;Property;_MaskAppearNoise;Mask Appear Noise;27;0;Create;None;022104b5ac3b9e4419e61d04b021d330;False;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;145;1472.893,2865.109;Float;False;Property;_MaskAppearNoiseTriplanarTiling;Mask Appear Noise Triplanar Tiling;30;0;Create;0.2;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;65;-1554.169,1154.476;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-912.6851,1667.138;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;67;-1552.788,1307.431;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PowerNode;122;-941.491,472.8224;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;64;-1555.548,1000.142;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;144;2346.234,2092.544;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;66;-1629.734,1406.841;Float;True;Property;_Noise01;Noise 01;11;0;Create;None;393ed426cda7a7c4d8881647fbf577a7;True;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;70;-1285.059,1364.694;Float;True;Property;_TextureSample3;Texture Sample 3;15;0;Create;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;124;-770.2318,355.9167;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SummedBlendNode;69;-638.4873,2083.742;Float;False;5;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;146;2546.94,2138.866;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;71;-1290.258,1159.294;Float;True;Property;_TextureSample4;Texture Sample 4;15;0;Create;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;68;-608.4453,2228.068;Float;False;Constant;_Float8;Float 8;24;0;Create;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;72;-1290.257,964.294;Float;True;Property;_TextureSample5;Texture Sample 5;15;0;Create;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TriplanarNode;148;2001.869,2787.498;Float;True;Spherical;World;False;Top Texture 1;_TopTexture1;white;-1;None;Mid Texture 1;_MidTexture1;white;-1;None;Bot Texture 1;_BotTexture1;white;-1;None;Triplanar Sampler;False;8;0;SAMPLER2D;;False;5;FLOAT;1.0;False;1;SAMPLER2D;;False;6;FLOAT;0.0;False;2;SAMPLER2D;;False;7;FLOAT;0.0;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;147;2056.868,2568.798;Float;True;Property;_TextureSample6;Texture Sample 6;32;0;Create;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;74;-594.3628,1697.967;Float;False;Property;_NoiseMaskPower;Noise Mask Power;9;0;Create;1;2;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;125;-621.595,350.2701;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;150;2692.538,2053.065;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;75;-395.7968,2137.46;Float;False;Property;_Noise02Enabled;Noise 02 Enabled;14;0;Create;0;False;False;True;;Toggle;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;149;2442.138,2681.209;Float;False;Property;_MaskAppearNoiseTriplanar;Mask Appear Noise Triplanar;29;0;Create;0;False;True;True;;Toggle;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SummedBlendNode;73;-676.6583,1151.494;Float;False;5;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;151;2892.069,2230.508;Float;False;Property;_MaskAppearProgress;Mask Appear Progress;26;0;Create;0;-1.018694;-2;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;153;2872.5,2110.995;Float;False;Property;_MaskAppearInvert;Mask Appear Invert;25;0;Create;0;False;True;True;;Toggle;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;128;-415.7776,171.1426;Float;False;Property;_MaskSoftBorders;Mask Soft Borders;4;0;Create;0;False;True;True;;Toggle;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;41.76122,1522.642;Float;False;3;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;152;3038.636,2328.154;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;79;-42.28455,-524.7101;Float;False;77;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;77;187.3783,1519.921;Float;False;ResultNoise;-1;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;155;3352.363,2410.409;Float;False;Constant;_Float6;Float 6;28;0;Create;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;113;-46.94867,-350.4143;Float;False;93;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;103;-109.0102,-440.3779;Float;False;Property;_ScreenDistortionPower;Screen Distortion Power;22;0;Create;0.025;0.015;0;0.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;93;-124.7478,171.5571;Float;False;ResultOpacity;-1;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;154;3292.5,2175.994;Float;True;3;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GrabScreenPosition;98;388.4261,-731.6953;Float;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;156;3591.64,2264.705;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;105;377.3232,-548.277;Float;False;3;3;0;FLOAT;0,0,0;False;1;FLOAT;0,0,0;False;2;FLOAT;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;100;678.3059,-641.2544;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;157;3732.961,2241.487;Float;True;Property;_MaskAppearRamp;Mask Appear Ramp;28;0;Create;None;81f58b421b2edce4c997c876e45c49f7;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;160;1252.157,-81.81519;Float;False;159;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;97;827.2896,-701.9072;Float;False;Global;_GrabScreen0;Grab Screen 0;21;0;Create;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;159;4057.054,2287.324;Float;False;MaskAppearValue;-1;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;117;1296.005,194.0876;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;119;1249.683,348.8459;Float;False;Property;_NormalVertexOffset;Normal Vertex Offset;7;0;Create;0;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;161;1487.604,-79.71893;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-1467.06,87.61079;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;118;1537.091,248.8321;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalVertexDataNode;2;-2743.878,-104.5229;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;96;-1776.266,318.2471;Float;False;Property;_NoiseMaskAdd;Noise Mask Add;10;0;Create;0.25;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;17;-1654.117,87.15874;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;116;1254.782,-406.4741;Float;False;Property;_VisualizeMaskDebug;Visualize Mask Debug;32;0;Create;0;False;False;True;;Toggle;2;0;COLOR;0,0,0,0;False;1;COLOR;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1822.864,-109.3223;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;SineVFX/ForceFieldAndShieldEffects/ForceFieldDistortion;False;False;False;False;True;True;True;True;True;False;True;True;False;False;True;False;False;Back;0;0;False;0;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;False;2;SrcAlpha;OneMinusSrcAlpha;0;One;One;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;130;0;129;0
WireConnection;27;0;29;0
WireConnection;27;1;30;0
WireConnection;11;0;13;0
WireConnection;132;0;130;1
WireConnection;132;1;130;2
WireConnection;132;2;130;3
WireConnection;133;0;131;0
WireConnection;133;1;132;0
WireConnection;8;0;27;0
WireConnection;12;0;11;0
WireConnection;34;0;31;0
WireConnection;34;3;32;0
WireConnection;1;0;3;0
WireConnection;1;1;8;0
WireConnection;19;0;12;0
WireConnection;134;0;133;0
WireConnection;134;1;131;0
WireConnection;135;0;134;0
WireConnection;15;0;19;0
WireConnection;15;1;16;0
WireConnection;5;0;1;0
WireConnection;38;0;34;0
WireConnection;38;1;33;0
WireConnection;43;0;35;2
WireConnection;43;1;40;0
WireConnection;41;0;137;0
WireConnection;41;1;37;0
WireConnection;164;0;165;0
WireConnection;167;0;15;0
WireConnection;167;1;168;0
WireConnection;42;0;38;0
WireConnection;42;1;36;0
WireConnection;6;0;5;0
WireConnection;6;1;7;0
WireConnection;48;0;41;0
WireConnection;48;1;43;0
WireConnection;48;2;42;0
WireConnection;50;0;136;0
WireConnection;50;1;44;0
WireConnection;49;0;45;2
WireConnection;49;1;47;0
WireConnection;163;0;164;1
WireConnection;163;1;166;2
WireConnection;10;0;6;0
WireConnection;10;1;167;0
WireConnection;51;0;48;0
WireConnection;80;0;10;0
WireConnection;53;0;50;0
WireConnection;53;1;49;0
WireConnection;53;2;42;0
WireConnection;162;0;163;0
WireConnection;162;1;139;2
WireConnection;56;0;51;2
WireConnection;56;1;51;0
WireConnection;59;0;51;1
WireConnection;59;1;51;2
WireConnection;54;0;51;0
WireConnection;54;1;51;1
WireConnection;126;0;80;0
WireConnection;126;1;127;0
WireConnection;58;0;53;0
WireConnection;57;0;52;0
WireConnection;141;0;162;0
WireConnection;141;2;138;0
WireConnection;63;0;55;0
WireConnection;63;1;59;0
WireConnection;61;0;55;0
WireConnection;61;1;56;0
WireConnection;62;0;55;0
WireConnection;62;1;54;0
WireConnection;65;0;58;2
WireConnection;65;1;58;0
WireConnection;60;0;57;0
WireConnection;60;1;57;0
WireConnection;67;0;58;0
WireConnection;67;1;58;1
WireConnection;122;0;126;0
WireConnection;122;1;123;0
WireConnection;64;0;58;1
WireConnection;64;1;58;2
WireConnection;144;0;141;0
WireConnection;144;1;140;0
WireConnection;70;0;66;0
WireConnection;70;1;67;0
WireConnection;124;0;80;0
WireConnection;124;1;122;0
WireConnection;69;0;60;0
WireConnection;69;1;63;1
WireConnection;69;2;61;1
WireConnection;69;3;62;1
WireConnection;146;0;144;0
WireConnection;71;0;66;0
WireConnection;71;1;65;0
WireConnection;72;0;66;0
WireConnection;72;1;64;0
WireConnection;148;0;143;0
WireConnection;148;3;145;0
WireConnection;147;0;143;0
WireConnection;147;1;142;0
WireConnection;125;0;124;0
WireConnection;150;0;146;0
WireConnection;75;0;69;0
WireConnection;75;1;68;0
WireConnection;149;0;148;1
WireConnection;149;1;147;1
WireConnection;73;0;60;0
WireConnection;73;1;72;1
WireConnection;73;2;71;1
WireConnection;73;3;70;1
WireConnection;153;0;150;0
WireConnection;153;1;146;0
WireConnection;128;0;125;0
WireConnection;128;1;80;0
WireConnection;76;0;73;0
WireConnection;76;1;75;0
WireConnection;76;2;74;0
WireConnection;152;0;149;0
WireConnection;77;0;76;0
WireConnection;93;0;128;0
WireConnection;154;0;153;0
WireConnection;154;1;151;0
WireConnection;154;2;152;0
WireConnection;156;0;154;0
WireConnection;156;1;155;0
WireConnection;105;0;79;0
WireConnection;105;1;103;0
WireConnection;105;2;113;0
WireConnection;100;0;98;0
WireConnection;100;1;105;0
WireConnection;157;1;156;0
WireConnection;97;0;100;0
WireConnection;159;0;157;2
WireConnection;161;0;160;0
WireConnection;95;0;17;0
WireConnection;95;1;96;0
WireConnection;118;0;117;0
WireConnection;118;1;119;0
WireConnection;17;0;10;0
WireConnection;116;0;113;0
WireConnection;116;1;97;0
WireConnection;0;2;116;0
WireConnection;0;9;161;0
WireConnection;0;11;118;0
ASEEND*/
//CHKSM=CCEAF9CC45659B341C292A98BA16047AD881261D