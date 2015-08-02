using UnityEngine;
using System.Collections;

public class Think : MonoBehaviour 
{


	// Use this for initialization
	void Start () 
	{
		GetComponent<Renderer>().material.shader = Shader.Find ("Specular");
		// Set red specular highlights
		GetComponent<Renderer>().material.SetColor ("_Color", Color.cyan);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnCollisionEnter(Collision collision) 
	{
		// Debug-draw all contact points and normals
		foreach (ContactPoint contactPt  in collision.contacts) 
		{
			Debug.DrawRay(contactPt.point, contactPt.normal, Color.white);
		}

		GetComponent<Renderer>().material.shader = Shader.Find ("Specular");
		// Set red specular highlights
		GetComponent<Renderer>().material.SetColor ("_Color", Color.red);
	}
}
