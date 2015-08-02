using UnityEngine;
using System.Collections;

public class GoalThink : MonoBehaviour 
{


	// Use this for initialization
	void Start () 
	{
		GetComponent<Renderer>().material.shader = Shader.Find ("Specular");
		// Set red specular highlights
		GetComponent<Renderer>().material.SetColor ("_Color", Color.yellow);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnCollisionEnter(Collision collision) 
	{
	}
}
