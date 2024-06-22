using UnityEditor;

using UnityEngine;

namespace OccaSoftware.LCD.Editor
{
	internal class LCDShaderGUI : ShaderGUI
	{
		public override void OnGUI(MaterialEditor e, MaterialProperty[] properties)
		{
			MaterialProperty emissiveTint = FindProperty("_EmissiveTint", properties, false);
			MaterialProperty mainTex = FindProperty("_MainTex", properties, false);
			MaterialProperty screenBrightnessFlickerOffsetSpeed = FindProperty("_ScreenBrightnessFlickerOffsetSpeed", properties, false);
			MaterialProperty screenBrightnessFlickerIntensity = FindProperty("_ScreenBrightnessFlickerIntensity", properties, false);

			MaterialProperty viewingAngleQuality = FindProperty("_ViewingAngleQuality", properties, false);
			MaterialProperty chromaLoss = FindProperty("_ChromaLoss", properties, false);
			MaterialProperty brightnessLoss = FindProperty("_BrightnessLoss", properties, false);
			MaterialProperty hueShift = FindProperty("_HueShift", properties, false);

			MaterialProperty diodeBorderTexture = FindProperty("_DiodeBorderTexture", properties, false);
			MaterialProperty diodeBorderFadeDistance = FindProperty("_DiodeBorderFadeDistance", properties, false);

			MaterialProperty diodeTexture = FindProperty("_DiodeTexture", properties, false);
			MaterialProperty diodeFadeDistance = FindProperty("_DiodeFadeDistance", properties, false);
			MaterialProperty diodeTiling = FindProperty("_DiodeTiling", properties, false);
			MaterialProperty diodeBrightnessMultiplier = FindProperty("_DiodeBrightnessMultiplier", properties, false);

			MaterialProperty scanlineTexture = FindProperty("_ScanlineTexture", properties, false);
			MaterialProperty scanlineFadeDistance = FindProperty("_ScanlineFadeDistance", properties, false);
			MaterialProperty scanlineTiling = FindProperty("_ScanlineTiling", properties, false);
			MaterialProperty scanlineSpeed = FindProperty("_ScanlineSpeed", properties, false);
			MaterialProperty scanlineIntensity = FindProperty("_ScanlineIntensity", properties, false);

			MaterialProperty microNoiseIntensity = FindProperty("_MicroNoiseIntensity", properties, false);
			MaterialProperty microNoiseTiling = FindProperty("_MicroNoiseTiling", properties, false);
			MaterialProperty microNoiseFadeDistance = FindProperty("_MicroNoiseFadeDistance", properties, false);

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.LabelField("Screen Settings", EditorStyles.boldLabel);
			e.ColorProperty(emissiveTint, "Emissive Tint");
			DrawTextureProperty(mainTex, new GUIContent("Texture"));
			e.DefaultShaderProperty(screenBrightnessFlickerOffsetSpeed, "Brightness Flicker Rate");
			e.DefaultShaderProperty(screenBrightnessFlickerIntensity, "Brightness Flicker Intensity");


			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Viewing Angle Settings", EditorStyles.boldLabel);
			e.DefaultShaderProperty(viewingAngleQuality, "Viewing Angle Quality");
			e.DefaultShaderProperty(chromaLoss, "Chroma Loss (%)");
			e.DefaultShaderProperty(brightnessLoss, "Brightness Loss (%)");
			e.DefaultShaderProperty(hueShift, "Hue Shift (Degrees)");


			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Diode Border Settings", EditorStyles.boldLabel);
			DrawTextureProperty(diodeBorderTexture, new GUIContent("Texture"));
			e.DefaultShaderProperty(diodeBorderFadeDistance, "Fade Distance");


			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Diode Settings", EditorStyles.boldLabel);
			DrawTextureProperty(diodeTexture, new GUIContent("Texture"));
			e.DefaultShaderProperty(diodeFadeDistance, "Fade Distance");
			DrawVector2Property(diodeTiling, new GUIContent("Tiling"));
			e.DefaultShaderProperty(diodeBrightnessMultiplier, "Brightness Multiplier");


			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Scanline Settings", EditorStyles.boldLabel);
			DrawTextureProperty(scanlineTexture, new GUIContent("Texture"));
			e.DefaultShaderProperty(scanlineFadeDistance, "Fade Distance");
			e.DefaultShaderProperty(scanlineTiling, "Tiling");
			e.DefaultShaderProperty(scanlineSpeed, "Speed");
			e.DefaultShaderProperty(scanlineIntensity, "Intensity");


			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Micro Noise Settings", EditorStyles.boldLabel);
			e.DefaultShaderProperty(microNoiseIntensity, "Intensity");
			e.DefaultShaderProperty(microNoiseFadeDistance, "Fade Distance");
			DrawVector2Property(microNoiseTiling, new GUIContent("Tiling"));
		}

		private static void DrawTextureProperty(MaterialProperty p, GUIContent c)
		{
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = p.hasMixedValue;
			Texture t = (Texture)EditorGUILayout.ObjectField(c, p.textureValue, typeof(Texture), false, GUILayout.Height(EditorGUIUtility.singleLineHeight));
			if (EditorGUI.EndChangeCheck())
			{
				p.textureValue = t;
			}
			EditorGUI.showMixedValue = false;
		}


		private static void DrawVector2Property(MaterialProperty p, GUIContent c)
		{
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = p.hasMixedValue;
			Vector2 v = EditorGUILayout.Vector2Field(c, p.vectorValue);
			if (EditorGUI.EndChangeCheck())
			{
				p.vectorValue = v;
			}
			EditorGUI.showMixedValue = false;
		}
	}




}