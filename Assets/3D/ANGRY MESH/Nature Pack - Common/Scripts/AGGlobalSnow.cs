// AG Global Wind - ANGRY MESH
// Copyright Paul Turc - ANGRY MESH - contact@angrymesh.com

using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class AGGlobalSnow : MonoBehaviour {

	[Header("Global Wind")]
	public bool EnableGlobalWind = true;
	float WindToggle = 0.0f;
	[Range(0.0f, 1.0f)]
	public float WindGrassAmplitude = 0.5f;
	[Range(0.0f, 10.0f)]
	public float WindGrassSpeed = 4.0f;
	[Range(0.0f, 3.0f)]
	public float WindGrassScale = 0.5f;
	[Range(0.0f, 1.0f)]
	public float WindGrassStiffness = 0.5f;
	[Header("Tint Color")]
	public bool EnableTintColor = true;
	float TintToggle = 0.0f;
	public Texture2D TintNoiseTexture = null;
	[Range(0.001f, 10.0f)]
	public float TintNoiseTile = 1.0f;
	[Range(0.001f, 10.0f)]
	public float TintNoiseContrast = 1.0f;
	[Range(0.0f, 1.0f)]
	[Header("Global Snow for Props")]
	public float SnowPropsIntensity = 1.0f;
	[Range(0.0f, 1.0f)]
	public float SnowPropsOffset = 1.0f;
	[Range(0.0f, 1.0f)]
	public float SnowPropsContrast = 1.0f;
	[Header("Global Snow Height")]
	public float SnowMinimumHeight = -100.0f;
	[Range(0.0f, 30.0f)]
	public float SnowFadeHeight = 1.0f;

//	[HideInInspector]
	void Awake(){

		// Disable Arrow in play mode
		if (Application.isPlaying == true) 
		{
			gameObject.GetComponent<MeshRenderer> ().enabled = false;
		} 
		else 
		{
			gameObject.GetComponent<MeshRenderer> ().enabled = true;
		}
	}

	void Update () {

		// Enable/Disable Wind
		if (EnableGlobalWind == true)
		{
			WindToggle = 1.0f;
		}
		else
		{
			WindToggle = 0.0f;
		}

		// Enable/Disable Tint Color
		if (EnableTintColor == true)
		{
			TintToggle = 1.0f;
		}
		else
		{
			TintToggle = 0.0f;
		}

		// Send informations to shaders
		Shader.SetGlobalVector ("AGW_WindDirection", gameObject.transform.forward);
		Shader.SetGlobalFloat ("AGW_WindToggle", WindToggle);
		Shader.SetGlobalFloat ("AGW_WindGrassAmplitude", WindGrassAmplitude);
		Shader.SetGlobalFloat ("AGW_WindGrassSpeed", WindGrassSpeed);
		Shader.SetGlobalFloat ("AGW_WindGrassScale", WindGrassScale);
		Shader.SetGlobalFloat ("AGW_WindGrassStiffness", WindGrassStiffness);
		Shader.SetGlobalTexture ("AG_TintNoiseTexture", TintNoiseTexture);
		Shader.SetGlobalFloat ("AG_TintToggle", TintToggle);
		Shader.SetGlobalFloat ("AG_TintNoiseTile", TintNoiseTile);
		Shader.SetGlobalFloat ("AG_TintNoiseContrast", TintNoiseContrast);
		Shader.SetGlobalFloat ("AGP_SnowIntensity", SnowPropsIntensity);
		Shader.SetGlobalFloat ("AGP_SnowOffset", SnowPropsOffset);
		Shader.SetGlobalFloat ("AGP_SnowContrast", SnowPropsContrast);
		Shader.SetGlobalFloat ("AGH_SnowMinimumHeight", SnowMinimumHeight);
		Shader.SetGlobalFloat ("AGH_SnowFadeHeight", SnowFadeHeight);
	}
}
