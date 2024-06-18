// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ANGRYMESH/Nature Pack/Standard/Grass"
{
	Properties
	{
		_Cutoff("Alpha Cutoff", Range( 0 , 1)) = 0.5
		[Header(Base)]_Glossiness("Base Smoothness", Range( 0 , 1)) = 0
		_Color("Base Color", Color) = (1,1,1,1)
		_BaseBottomColor("Base Bottom Color", Color) = (1,1,1,0)
		_BaseBottomOffset("Base Bottom Offset", Range( 0 , 5)) = 5
		[NoScaleOffset]_MainTex("Base Albedo (A Opacity)", 2D) = "gray" {}
		[NoScaleOffset][Normal]_BumpMap("Base Normal Map", 2D) = "bump" {}
		[Header(Tint Color)]_TintColor1("Tint Color 1", Color) = (1,1,1,0)
		_TintColor2("Tint Color 2", Color) = (1,1,1,0)
		_TintNoiseTile("Tint Noise Tile", Range( 0.001 , 30)) = 10
		[Header(Translucency)]_Strenght("Strenght", Range( 0 , 10)) = 2
		[Header(Wind)]_WindAmplitude("Wind Amplitude", Range( 0 , 3)) = 1
		_WindSpeed("Wind Speed", Range( 0 , 10)) = 5
		_WindScale("Wind Scale", Range( 0 , 30)) = 10
		_WindGrassStiffness("Wind Grass Stiffness", Range( 0 , 2)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf StandardCustom keepalpha addshadow fullforwardshadows exclude_path:deferred novertexlights nolightmap  nodynlightmap nodirlightmap nometa dithercrossfade vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			half4 vertexToFrag18_g33;
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
			half3 Transmission;
		};

		uniform sampler2D AG_TintNoiseTexture;
		uniform half _Cutoff;
		uniform half _WindScale;
		uniform half AGW_WindGrassScale;
		uniform half AGW_WindGrassSpeed;
		uniform half _WindSpeed;
		uniform half AGW_WindToggle;
		uniform half _WindAmplitude;
		uniform half AGW_WindGrassAmplitude;
		uniform half _WindGrassStiffness;
		uniform half AGW_WindGrassStiffness;
		uniform half3 AGW_WindDirection;
		uniform sampler2D _BumpMap;
		uniform half4 _BaseBottomColor;
		uniform half4 _Color;
		uniform sampler2D _MainTex;
		uniform half4 _TintColor1;
		uniform half4 _TintColor2;
		uniform half AG_TintNoiseTile;
		uniform half _TintNoiseTile;
		uniform half AG_TintNoiseContrast;
		uniform half AG_TintToggle;
		uniform half _BaseBottomOffset;
		uniform half _Glossiness;
		uniform half _Strenght;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			half mulTime32_g35 = _Time.y * ( AGW_WindGrassSpeed * _WindSpeed );
			half temp_output_9_0_g35 = ( AGW_WindToggle * _WindAmplitude * AGW_WindGrassAmplitude * 0.5 );
			half temp_output_1_0_g35 = v.color.r;
			half temp_output_3_0_g35 = ( ( ( sin( ( ( ase_worldPos.z * ( _WindScale * AGW_WindGrassScale ) ) + mulTime32_g35 ) ) * temp_output_9_0_g35 ) * temp_output_1_0_g35 ) + ( temp_output_9_0_g35 * temp_output_1_0_g35 * ( (5.0 + (_WindGrassStiffness - 0.0) * (0.0 - 5.0) / (2.0 - 0.0)) * (1.0 + (AGW_WindGrassStiffness - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) ) ) );
			half3 appendResult70_g35 = (half3(temp_output_3_0_g35 , 0.0 , temp_output_3_0_g35));
			half3 Output_Wind159 = mul( unity_WorldToObject, half4( ( appendResult70_g35 * AGW_WindDirection ) , 0.0 ) ).xyz;
			v.vertex.xyz += Output_Wind159;
			v.vertex.w = 1;
			half4 temp_cast_2 = (1.0).xxxx;
			half2 appendResult8_g33 = (half2(ase_worldPos.x , ase_worldPos.z));
			half4 lerpResult17_g33 = lerp( _TintColor1 , _TintColor2 , saturate( ( tex2Dlod( AG_TintNoiseTexture, float4( ( appendResult8_g33 * ( 0.001 * AG_TintNoiseTile * _TintNoiseTile ) ), 0, 0.0) ).r * (0.001 + (AG_TintNoiseContrast - 0.001) * (60.0 - 0.001) / (10.0 - 0.001)) ) ));
			half4 lerpResult19_g33 = lerp( temp_cast_2 , lerpResult17_g33 , AG_TintToggle);
			o.vertexToFrag18_g33 = lerpResult19_g33;
		}

		inline half4 LightingStandardCustom(SurfaceOutputStandardCustom s, half3 viewDir, UnityGI gi )
		{
			half3 transmission = max(0 , -dot(s.Normal, gi.light.dir)) * gi.light.color * s.Transmission;
			half4 d = half4(s.Albedo * transmission , 0);

			SurfaceOutputStandard r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Metallic = s.Metallic;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandard (r, viewDir, gi) + d;
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
			float2 uv_BumpMap23 = i.uv_texcoord;
			half3 Output_Normal96 = UnpackNormal( tex2D( _BumpMap, uv_BumpMap23 ) );
			o.Normal = Output_Normal96;
			float2 uv_MainTex1 = i.uv_texcoord;
			half4 tex2DNode1 = tex2D( _MainTex, uv_MainTex1 );
			half4 TintColor229 = i.vertexToFrag18_g33;
			half4 temp_output_33_0 = ( _Color * tex2DNode1 * TintColor229 );
			half4 lerpResult8 = lerp( ( _BaseBottomColor * temp_output_33_0 ) , temp_output_33_0 , saturate( pow( abs( ( i.vertexColor.r * 2.0 ) ) , _BaseBottomOffset ) ));
			half4 Output_Albedo97 = lerpResult8;
			o.Albedo = Output_Albedo97.rgb;
			o.Smoothness = _Glossiness;
			half3 temp_cast_1 = (_Strenght).xxx;
			o.Transmission = temp_cast_1;
			o.Alpha = 1;
			half Output_OpacityMask58 = ( _Color.a * tex2DNode1.a );
			clip( Output_OpacityMask58 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=18935
1920;78;1920;1019;2349.33;954.7175;1.21975;True;False
Node;AmplifyShaderEditor.ColorNode;225;256,704;Half;False;Property;_TintColor2;Tint Color 2;10;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;224;256,512;Half;False;Property;_TintColor1;Tint Color 1;9;0;Create;True;0;0;0;False;1;Header(Tint Color);False;1,1,1,0;1,1,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;226;256,896;Half;False;Property;_TintNoiseTile;Tint Noise Tile;11;0;Create;True;0;0;0;False;0;False;10;10;0.001;30;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;150;-1792,-320;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;156;-1792,-128;Half;False;Constant;_Float0;Float 0;11;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;295;640,512;Inherit;False;AG Global Tint Color;0;;33;1dcc860732522ee469468f952b4e8aa1;0;3;27;COLOR;0,0,0,0;False;28;COLOR;0,0,0,0;False;22;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;229;1024,512;Half;False;TintColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;155;-1408,-320;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;245;-1792,992;Half;False;Property;_WindGrassStiffness;Wind Grass Stiffness;16;0;Create;True;0;0;0;False;0;False;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;149;-1792,512;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;147;-1792,896;Half;False;Property;_WindSpeed;Wind Speed;14;0;Create;True;0;0;0;False;0;False;5;5;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;148;-1792,704;Half;False;Property;_WindAmplitude;Wind Amplitude;13;0;Create;True;0;0;0;False;1;Header(Wind);False;1;1;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;230;-1792,-432;Inherit;False;229;TintColor;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-1792,-640;Inherit;True;Property;_MainTex;Base Albedo (A Opacity);7;1;[NoScaleOffset];Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.AbsOpNode;296;-1152,-320;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-1408,-192;Half;False;Property;_BaseBottomOffset;Base Bottom Offset;6;0;Create;True;0;0;0;False;0;False;5;5;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;260;-1408,896;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;2;False;3;FLOAT;5;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;32;-1792,-832;Half;False;Property;_Color;Base Color;4;0;Create;False;0;0;0;False;0;False;1,1,1,1;1,1,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;176;-1792,800;Half;False;Property;_WindScale;Wind Scale;15;0;Create;True;0;0;0;False;0;False;10;10;0;30;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-1408,-640;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector3Node;241;-1152,704;Half;False;Global;AGW_WindDirection;AGW_WindDirection;20;0;Create;True;0;0;0;False;0;False;0,0,0;0.9382213,-0.3195158,-0.1328554;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PowerNode;154;-1024,-320;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;6;-1792,-1024;Half;False;Property;_BaseBottomColor;Base Bottom Color;5;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;297;-1152,512;Inherit;False;AG Global Wind - Grass;-1;;35;ab99ce4e769429241ac615381e930fc9;0;5;1;FLOAT;0;False;54;FLOAT;0;False;48;FLOAT;0;False;41;FLOAT;0;False;55;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-896,-1024;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldToObjectMatrix;243;-768,640;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.SaturateNode;13;-768,-320;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;242;-768,512;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;23;256,-1008;Inherit;True;Property;_BumpMap;Base Normal Map;8;2;[NoScaleOffset];[Normal];Create;False;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;8;-640,-1024;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;244;-512,512;Inherit;False;2;2;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;-1408,-768;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;58;-1152,-768;Half;False;Output_OpacityMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;97;-384,-1024;Half;False;Output_Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;159;-256,512;Half;False;Output_Wind;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;96;768,-1008;Half;False;Output_Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;231;1664,-736;Half;False;Property;_Strenght;Strenght;12;0;Create;True;0;0;0;False;1;Header(Translucency);False;2;2;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;157;1664,-1024;Inherit;False;97;Output_Albedo;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;217;1664,-448;Half;False;Property;_Cutoff;Alpha Cutoff;2;0;Create;False;0;0;0;True;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;134;1664,-832;Half;False;Property;_Glossiness;Base Smoothness;3;0;Create;False;0;0;0;False;1;Header(Base);False;0;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;160;1664,-544;Inherit;False;159;Output_Wind;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;57;1664,-640;Inherit;False;58;Output_OpacityMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;158;1664,-928;Inherit;False;96;Output_Normal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2176,-1024;Half;False;True;-1;2;;0;0;Standard;ANGRYMESH/Nature Pack/Standard/Grass;False;False;False;False;False;True;True;True;True;False;True;False;True;False;True;False;True;False;False;False;True;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;ForwardOnly;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;217;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;279;1664,-1152;Inherit;False;767.1542;100;;0;// Outputs;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;257;256,384;Inherit;False;1023;100;;0;// Tint Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;189;-1792,384;Inherit;False;1793.364;100;;0;// Vertex Wind;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;60;-1792,-1152;Inherit;False;1662.288;100;;0;// Albedo Map;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;59;256,-1136;Inherit;False;767.1542;100;;0;// Normal Map;1,1,1,1;0;0
WireConnection;295;27;224;0
WireConnection;295;28;225;0
WireConnection;295;22;226;0
WireConnection;229;0;295;0
WireConnection;155;0;150;1
WireConnection;155;1;156;0
WireConnection;296;0;155;0
WireConnection;260;0;245;0
WireConnection;33;0;32;0
WireConnection;33;1;1;0
WireConnection;33;2;230;0
WireConnection;154;0;296;0
WireConnection;154;1;11;0
WireConnection;297;1;149;1
WireConnection;297;54;148;0
WireConnection;297;48;176;0
WireConnection;297;41;147;0
WireConnection;297;55;260;0
WireConnection;34;0;6;0
WireConnection;34;1;33;0
WireConnection;13;0;154;0
WireConnection;242;0;297;0
WireConnection;242;1;241;0
WireConnection;8;0;34;0
WireConnection;8;1;33;0
WireConnection;8;2;13;0
WireConnection;244;0;243;0
WireConnection;244;1;242;0
WireConnection;99;0;32;4
WireConnection;99;1;1;4
WireConnection;58;0;99;0
WireConnection;97;0;8;0
WireConnection;159;0;244;0
WireConnection;96;0;23;0
WireConnection;0;0;157;0
WireConnection;0;1;158;0
WireConnection;0;4;134;0
WireConnection;0;6;231;0
WireConnection;0;10;57;0
WireConnection;0;11;160;0
ASEEND*/
//CHKSM=B810975D77E4BF0F688CE67CBDEE86D915A69796