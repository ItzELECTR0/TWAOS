// AG Global Wind - ANGRY MESH
// Copyright Paul Turc - ANGRY MESH - contact@angrymesh.com

using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class AGGlobalWind : MonoBehaviour {

	public bool EnableGlobalWind = true;
	float WindToggle = 0.0f;
	[Range(0.0f, 1.0f)]
	public float WindTreeAmplitude = 0.2f;
	[Range(0.0f, 10.0f)]
	public float WindTreeSpeed = 4.0f;
	[Range(0.0f, 3.0f)]
	public float WindTreeScale = 0.5f;
	[Range(0.0f, 1.0f)]
	public float WindTreeStiffness = 0.5f;
	[Range(0.0f, 1.0f)]
	[Space(20)]
	public float WindGrassAmplitude = 0.5f;
	[Range(0.0f, 10.0f)]
	public float WindGrassSpeed = 4.0f;
	[Range(0.0f, 3.0f)]
	public float WindGrassScale = 0.5f;
	[Range(0.0f, 1.0f)]
	public float WindGrassStiffness = 0.5f;

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

		// Send informations to shaders
		Shader.SetGlobalVector ("AGW_WindDirection", gameObject.transform.forward);
		Shader.SetGlobalFloat ("AGW_WindToggle", WindToggle);
		Shader.SetGlobalFloat ("AGW_WindAmplitude", WindTreeAmplitude);
		Shader.SetGlobalFloat ("AGW_WindSpeed", WindTreeSpeed);
		Shader.SetGlobalFloat ("AGW_WindScale", WindTreeScale);
		Shader.SetGlobalFloat ("AGW_WindTreeStiffness", WindTreeStiffness);
		Shader.SetGlobalFloat ("AGW_WindGrassAmplitude", WindGrassAmplitude);
		Shader.SetGlobalFloat ("AGW_WindGrassSpeed", WindGrassSpeed);
		Shader.SetGlobalFloat ("AGW_WindGrassScale", WindGrassScale);
		Shader.SetGlobalFloat ("AGW_WindGrassStiffness", WindGrassStiffness);
	}
}
