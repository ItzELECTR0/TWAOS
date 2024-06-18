// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ANGRYMESH/Nature Pack/Standard/Props"
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
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows dithercrossfade 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _BaseNormalMap;
		uniform half _BumpScale;
		uniform half4 _BaseColor;
		uniform sampler2D _BaseAlbedoASmoothness;
		uniform half _BaseSmoothness;
		uniform sampler2D _BaseAOANoiseMask;
		uniform half _BaseAOIntensity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_BaseNormalMap96 = i.uv_texcoord;
			half3 normalizeResult136 = normalize( UnpackScaleNormal( tex2D( _BaseNormalMap, uv_BaseNormalMap96 ), _BumpScale ) );
			half3 Output_Normal320 = normalizeResult136;
			o.Normal = Output_Normal320;
			float2 uv_BaseAlbedoASmoothness162 = i.uv_texcoord;
			half4 tex2DNode162 = tex2D( _BaseAlbedoASmoothness, uv_BaseAlbedoASmoothness162 );
			half4 Output_Albedo318 = ( _BaseColor * tex2DNode162 );
			o.Albedo = Output_Albedo318.rgb;
			float AlbedoAlphaSmoothness212 = tex2DNode162.a;
			half Output_Smoothness223 = ( AlbedoAlphaSmoothness212 + (-1.0 + (_BaseSmoothness - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) );
			o.Smoothness = Output_Smoothness223;
			half4 temp_cast_1 = (1.0).xxxx;
			float2 uv_BaseAOANoiseMask200 = i.uv_texcoord;
			half4 lerpResult201 = lerp( temp_cast_1 , tex2D( _BaseAOANoiseMask, uv_BaseAOANoiseMask200 ) , _BaseAOIntensity);
			half4 Output_AO322 = lerpResult201;
			o.Occlusion = Output_AO322.r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=18935
1920;78;1920;1019;7712.373;4401.241;5.499691;True;False
Node;AmplifyShaderEditor.SamplerNode;162;-2688,-1984;Inherit;True;Property;_BaseAlbedoASmoothness;Base Albedo (A Smoothness);4;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;216;-5264,-1280;Half;False;Property;_BaseSmoothness;Base Smoothness;0;0;Create;True;0;0;0;False;1;Header(Base);False;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;277;-5248,-2176;Float;True;Property;_BaseAOANoiseMask;Base AO (A NoiseMask);6;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;287;-2688,-1408;Float;True;Property;_BaseNormalMap;Base NormalMap;5;2;[Normal];[NoScaleOffset];Create;True;0;0;0;False;0;False;None;None;True;bump;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RegisterLocalVarNode;212;-2304,-1920;Float;False;AlbedoAlphaSmoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;326;-2688,-1184;Half;False;Property;_BumpScale;Base Normal Intensity;2;0;Create;False;0;0;0;False;0;False;1;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;96;-2304,-1408;Inherit;True;Property;_Test;Test;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;165;-2688,-2176;Half;False;Property;_BaseColor;Base Color;3;0;Create;True;0;0;0;False;0;False;1,1,1,0;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;200;-4864,-2176;Inherit;True;Property;_Texture;Texture;5;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;231;-4864,-1984;Half;False;Constant;_White1;White1;19;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;203;-4864,-1888;Half;False;Property;_BaseAOIntensity;Base AO Intensity;1;0;Create;True;0;0;0;False;0;False;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;214;-5264,-1408;Inherit;False;212;AlbedoAlphaSmoothness;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;310;-4880,-1280;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;163;-2176,-2176;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;201;-4352,-2176;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalizeNode;136;-1920,-1408;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;311;-4624,-1408;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;322;-4096,-2176;Half;False;Output_AO;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;318;-1920,-2176;Half;False;Output_Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;320;-1664,-1408;Half;False;Output_Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;223;-4368,-1408;Half;False;Output_Smoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;319;-768,-2176;Inherit;False;318;Output_Albedo;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;224;-768,-1984;Inherit;False;223;Output_Smoothness;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;324;-768,-1888;Inherit;False;322;Output_AO;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;321;-768,-2080;Inherit;False;320;Output_Normal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-384,-2176;Half;False;True;-1;2;;0;0;Standard;ANGRYMESH/Nature Pack/Standard/Props;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;True;False;False;False;True;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;-0.09;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;8.6;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;210;-5248,-2304;Inherit;False;1403.268;100;;0;// AO;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;240;-2688,-1536;Inherit;False;1280.551;100;;0;// Normal Map;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;325;-768,-2304;Inherit;False;768.5388;100;;0;// Outputs;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;230;-5264,-1536;Inherit;False;1156.316;100;;0;// Smoothness;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;237;-2688,-2304;Inherit;False;1022.575;100;;0;// Albedo Map;1,1,1,1;0;0
WireConnection;212;0;162;4
WireConnection;96;0;287;0
WireConnection;96;5;326;0
WireConnection;200;0;277;0
WireConnection;310;0;216;0
WireConnection;163;0;165;0
WireConnection;163;1;162;0
WireConnection;201;0;231;0
WireConnection;201;1;200;0
WireConnection;201;2;203;0
WireConnection;136;0;96;0
WireConnection;311;0;214;0
WireConnection;311;1;310;0
WireConnection;322;0;201;0
WireConnection;318;0;163;0
WireConnection;320;0;136;0
WireConnection;223;0;311;0
WireConnection;0;0;319;0
WireConnection;0;1;321;0
WireConnection;0;4;224;0
WireConnection;0;5;324;0
ASEEND*/
//CHKSM=BEAAB2269273E0DE0FE13B62D36CA03B8EDA2B75