using UnityEngine;
using System.Collections;

public class OverheadCameraFollow : ShipSubsystem 
{
	GameObject spacePath;
	float cameraEasing = 3f;
	override protected void Initalize ()
	{ 
		SubDisplayName= "Overhead Sensors";
		SubDescription= "Records the 2d Image \n of the universe.";
		SubCostEnergyPassive=50f;
		SubShowOnPanel = true;
		SubStatus=Status.active;
		//NA
	}

	override protected void  Think ()
	{
		//NA
	}

	override protected void ThinkFast ()
	{
		DroneCameraFollow();
	}

	
	public void DroneCameraFollow()
	{
		if (spacePath == null)
		{
			spacePath = GameObject.Find ("SpacePath");
			if (spacePath == null)
				return;
		}
		//Vector3 theUp = posBeacon.transform.up;
		Vector3 upVector;
		Vector3 forwardVector;

		Vector3 beaconPos = (spacePath.GetComponent<SpacePath> ()).GetNearestPoint (ship.transform.position,out upVector, out forwardVector);
		
		Vector3 pos;
		Vector3 lookTarget;
		Vector3 relativeLookPos;
		relativeLookPos = ship.transform.position - beaconPos;
		float shipDistance = relativeLookPos.magnitude;

		//relativeLookPos = relativeLookPos*((Mathf.Max(shipDistance,100f))/shipDistance);
		//lookTarget = posBeacon.transform.position + relativeLookPos/2f;
		
		lookTarget = beaconPos + relativeLookPos/cameraEasing; 
		
		pos = beaconPos;
		pos += upVector*100;
		
		Vector3 theForward = (lookTarget - pos);
		Vector3 theUp = Vector3.Cross(forwardVector,upVector);
		theForward.Normalize ();
		
		//this.transform.position = pos + forwardVector.normalized * 40f;;
	//	this.transform.position = pos + ship.transform.forward.normalized * 10f;
		Vector3 sideVector = Vector3.Cross(forwardVector,upVector);

		this.transform.position = pos 
			+ Vector3.Project(ship.transform.forward.normalized,forwardVector.normalized) * 40f
				+ Vector3.Project(ship.transform.forward.normalized,sideVector.normalized) * 10f;

		this.transform.rotation = Quaternion.LookRotation (theForward, theUp);
	}


}
