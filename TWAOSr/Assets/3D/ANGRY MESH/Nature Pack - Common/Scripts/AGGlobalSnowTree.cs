// AG Global Snow Tree - ANGRY MESH
// Copyright Paul Turc - ANGRY MESH - contact@angrymesh.com

using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class AGGlobalSnowTree : MonoBehaviour {

	[Range(0.0f, 1.0f)]
	[Header("Global Snow for Trees")]
	public float SnowTreeIntensity = 1.0f;
	[Range(0.0f, 1.0f)]
	public float SnowTreeOffset = 1.0f;
	[Range(0.0f, 1.0f)]
	public float SnowTreeContrast = 1.0f;
	[Range(0.0f, 1.0f)]
	public float SnowTreeArrowDirection = 1.0f;

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
		Shader.SetGlobalFloat ("AGT_SnowIntensity", SnowTreeIntensity);
		Shader.SetGlobalFloat ("AGT_SnowOffset", SnowTreeOffset);
		Shader.SetGlobalFloat ("AGT_SnowContrast", SnowTreeContrast);
		Shader.SetGlobalFloat ("AGT_SnowArrow", SnowTreeArrowDirection);
	}
}
