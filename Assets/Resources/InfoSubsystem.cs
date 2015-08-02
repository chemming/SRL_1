using UnityEngine;
using System.Collections;

public class InfoSubsystem : ShipSubsystem {

	// Use this for initialization
	TextMesh txtHull;
	TextMesh txtSpeed;
	TextMesh txtShield;

	protected override void Initalize () 
	{

	//	MeshRenderer meshRenderer = theText.AddComponent("MeshRenderer") as MeshRenderer;

		for(int i = 0; i<3;i++)
		{
			string target= "Hull";
			if(i == 0) target = "Hull";
			if(i == 1) target = "Velocity";
			if(i == 2) target = "Shield";


			GameObject theText = Instantiate (Resources.Load ("ConsoleText")) as GameObject;

			Transform hullTransform = this.transform.FindChild ("InfoPanel").FindChild (target);

			theText.GetComponent<TextMesh>().text = "Hello World!";
			theText.transform.parent = hullTransform;
			theText.transform.position = hullTransform.position;
			theText.transform.rotation = hullTransform.rotation;
			theText.transform.localScale = Vector3.one * 0.07f;

			if(i == 0) txtHull = theText.GetComponent<TextMesh> ();
			if(i == 1) txtSpeed = theText.GetComponent<TextMesh> ();
			if(i == 2) txtShield = theText.GetComponent<TextMesh> ();
		}
	}
	
	// Update is called once per frame
	protected override void Think () 
	{
		txtSpeed.text = ((int)(ship.GetComponent<Rigidbody>().velocity.magnitude)) + "c"; 
		txtShield.text = "o))))";
		txtHull.text = "0]]]]";

	}

	protected override void ThinkFast () 
	{
	}

}
