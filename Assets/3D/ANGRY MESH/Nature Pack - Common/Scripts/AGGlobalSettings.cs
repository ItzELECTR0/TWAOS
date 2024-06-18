// AG Global Settings - ANGRY MESH
// Copyright Paul Turc - ANGRY MESH - contact@angrymesh.com

using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class AGGlobalSettings : MonoBehaviour {

	[Range(0.0f, 2.0f)]
	[Header("Trees Settings")]
	public float AOIntensity = 1.0f;
	[Space(20)]
	public bool EnableTintColor = true;
	float TintToggle = 0.0f;
	public Texture2D TintNoiseTexture = null;
	[Range(0.001f, 10.0f)]
	public float TintNoiseTile = 1.0f;
	[Range(0.001f, 10.0f)]
	public float TintNoiseContrast = 1.0f;

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
		Shader.SetGlobalFloat ("AG_TreesAO", AOIntensity);
		Shader.SetGlobalTexture ("AG_TintNoiseTexture", TintNoiseTexture);
		Shader.SetGlobalFloat ("AG_TintToggle", TintToggle);
		Shader.SetGlobalFloat ("AG_TintNoiseTile", TintNoiseTile);
		Shader.SetGlobalFloat ("AG_TintNoiseContrast", TintNoiseContrast);
	}
}
