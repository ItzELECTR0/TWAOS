// AG Global Snow Props - ANGRY MESH
// Copyright Paul Turc - ANGRY MESH - contact@angrymesh.com

using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class AGGlobalSnowProps : MonoBehaviour {

	[Range(0.0f, 1.0f)]
	[Header("Global Snow for Props")]
	public float SnowPropsIntensity = 1.0f;
	[Range(0.0f, 1.0f)]
	public float SnowPropsOffset = 1.0f;
	[Range(0.0f, 1.0f)]
	public float SnowPropsContrast = 1.0f;

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

		// Send informations to shaders
		Shader.SetGlobalFloat ("AGP_SnowIntensity", SnowPropsIntensity);
		Shader.SetGlobalFloat ("AGP_SnowOffset", SnowPropsOffset);
		Shader.SetGlobalFloat ("AGP_SnowContrast", SnowPropsContrast);
	}
}
