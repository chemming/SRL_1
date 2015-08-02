using UnityEngine;
using System.Collections;

public class ItemThink : MonoBehaviour 
{
	public bool carried = false; 
	public string param_weight = "light";

	
	// Use this for initialization
	void Start () 
	{
		GetComponent<Renderer>().material.shader = Shader.Find ("Specular");
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(carried == true)
		{
			GetComponent<Renderer>().material.shader = Shader.Find ("Specular");
			GetComponent<Renderer>().material.SetColor ("_Color", Color.red);
		}

		else if(carried == false)
		{
			GetComponent<Renderer>().material.shader = Shader.Find ("Specular");
			if(param_weight == "light")
				GetComponent<Renderer>().material.SetColor ("_Color", new Color(1f,1f,1f,0.5f));
			else
				GetComponent<Renderer>().material.SetColor ("_Color", new Color(0.5f,0.5f,0.5f,1f));
		}

	}
}
