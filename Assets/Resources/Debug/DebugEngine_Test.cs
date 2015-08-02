using UnityEngine;
using System.Collections;

public class DebugEngine_Test : ShipSubsystem 
{
	ShipAutopilot sa;
	GameObject beacon;
	Vector3 targetPoint;
	Vector3 targetOrientation;
	Vector3 targetUp;

	override protected void Initalize ()
	{
		beacon = GameObject.Instantiate(Resources.Load("PathBeacon")) as GameObject;
		//Disable the keyboard
	//	(system ["ControlPanel"].GetComponent<ShipControlPanel> ()).DisableThink ();

		targetPoint = new Vector3(5f,0f,20f);
		targetOrientation = new Vector3 (0f, 0f, 1f);
		targetUp = new Vector3 (0f, 1f, 0f);

		//Test the autopilot
		sa = system ["ShipAutopilot"].GetComponent<ShipAutopilot> ();
		ship.GetComponent<Rigidbody>().maxAngularVelocity = 0f;
	}

	override protected void Think ()
	{
		/*
		Vector3 vTarg = targetPoint - ship.transform.position;
		if( vTarg.magnitude < 3)
		{
			targetPoint = new Vector3(15f,0f,0f);
			targetOrientation = new Vector3 (0f, 0f, 1f);
			targetUp = new Vector3 (0f, 1f, 0f);	
		}
*/
		//vTarg.Normalize ();
		//vTarg = vTarg * 5f;

//		sa.SetMovementTarget (vTarg);
//		sa.SetOrientationTarget (targetOrientation);
//		sa.SetOrientationUp (targetUp);

//		beacon.transform.position = targetPoint;
	}
	override protected void ThinkFast ()
	{

	}


}
