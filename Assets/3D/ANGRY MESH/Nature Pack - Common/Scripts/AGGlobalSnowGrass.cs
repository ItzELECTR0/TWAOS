// AG Global Snow Grass - ANGRY MESH
// Copyright Paul Turc - ANGRY MESH - contact@angrymesh.com

using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class AGGlobalSnowGrass : MonoBehaviour {

	[Range(0.0f, 1.0f)]
	[Header("Global Snow for Grass")]
	public float SnowGrassIntensity = 1.0f;
	[Range(0.0f, 1.0f)]
	public float SnowGrassOffset = 1.0f;
	[Range(0.0f, 1.0f)]
	public float SnowGrassContrast = 1.0f;

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
		Shader.SetGlobalFloat ("AGG_SnowIntensity", SnowGrassIntensity);
		Shader.SetGlobalFloat ("AGG_SnowOffset", SnowGrassOffset);
		Shader.SetGlobalFloat ("AGG_SnowContrast", SnowGrassContrast);
	}
}
