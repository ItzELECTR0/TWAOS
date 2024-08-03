// AG Global Snow Tree - ANGRY MESH
// Copyright Paul Turc - ANGRY MESH - contact@angrymesh.com

using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class AGGlobalSnowHeight : MonoBehaviour {

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

		// Send informations to shaders
		Shader.SetGlobalFloat ("AGH_SnowMinimumHeight", SnowMinimumHeight);
		Shader.SetGlobalFloat ("AGH_SnowFadeHeight", SnowFadeHeight);
	}
}
