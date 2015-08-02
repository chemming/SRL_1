using UnityEngine;
using System.Collections;

public class SpaceSceneThink : MonoBehaviourThink 
{
	GameObject anchor;
	override protected void Initalize ()
	{
		anchor = GameObject.Find("Robot");
	}
	
	override protected void Think ()
	{

	}

	override protected void ThinkFast ()
	{
		if(anchor == null)
			anchor = GameObject.Find("Robot");
		if (anchor == null)
			return;
		//this.transform.parent = anchor.transform;
		this.transform.position = anchor.transform.position;
	}


}
