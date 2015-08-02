using UnityEngine;
using System.Collections;

public class ObstacleThink : MonoBehaviourThink {

	GameObject player;
	// Use this for initialization
	override protected void Initalize () 
	{
		player = GameObject.FindWithTag("robot");
	}
	// Update is called once per frame
	override protected void ThinkFast ()
	{
	}

	// Update is called once per frame
	override protected void Think () 
	{
		Vector3 pos = transform.position;
		if(pos.x > SRLConfiguration.W_length)
			pos.x = 0f;
		if(pos.x < 0f)
			pos.x = SRLConfiguration.W_length;
		if(pos.y > SRLConfiguration.W_height)
			pos.y = 0f;
		if(pos.y < 0f)
			pos.y = SRLConfiguration.W_height;
		if(pos.z > SRLConfiguration.W_width)
			pos.z = 0f;
		if(pos.z < 0f)
			pos.z = SRLConfiguration.W_width;

		if(player != null )
		{
			float diff = player.transform.position.y - transform.position.y;
			if (Mathf.Abs(diff) < 2)
			{
				GetComponent<Renderer>().material.shader = Shader.Find ("Specular");
				GetComponent<Renderer>().material.SetColor ("_Color", Color.red);

			}	
			else
			{
				GetComponent<Renderer>().material.shader = Shader.Find ("Specular");
				GetComponent<Renderer>().material.SetColor ("_Color", Color.grey);

			}
		}



		transform.position = pos;
	}
}
