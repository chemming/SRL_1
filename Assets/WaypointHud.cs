using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointHud : ShipSubsystem 
{
	GameObject nextWpArrow;
	List<GameObject> targetArrows;
	List<GameObject> targetObjects;

	GameObject shipDisplay;
	GameObject overCam;
	GameObject spacePath;
	bool overheadMode;

	float lrBorder;
	float udBorder;
	float widthFocusDistance;
	float heightFocusDistance;
	float iconForwardDistance;

	// Use this for initialization
	protected override void Initalize () 
	{
		targetArrows = new List<GameObject> ();
		targetObjects = new List<GameObject> ();

		SubDisplayName= "Waypoint Subsystem <Internal>";
		SubDescription= "Shows indicators on the 3d screen";
		SubCostEnergyPassive=3f;
		SubShowOnPanel = true;
		SubStatus=Status.active;
		SubStoreMaxMetal = 0;
		SubStoreMetal = 0;
		//SubStoreMaxEnergy = 5000;
		//SubStoreEnergy = 5000;
		nextWpArrow = GameObject.Instantiate(Resources.Load("components/ship/WaypointArrow")) as GameObject;
		nextWpArrow.transform.parent = this.transform;
		SystemSetup ();


		lrBorder = 0.4f;
		udBorder = 0.15f;
		widthFocusDistance = 120f;
		heightFocusDistance = 90f;
		iconForwardDistance = 0.7f;
	}
	

	public void TrackObject(GameObject obj)
	{
		targetObjects.Add (obj);
		targetArrows.Add (GameObject.Instantiate (Resources.Load ("components/ship/WaypointArrow")) as GameObject);

	}

	public void UntrackObject(GameObject obj)
	{
		int index = targetObjects.IndexOf (obj);
		if(index > -1)
		{
			targetObjects.RemoveAt (index);
			GameObject arrow = targetArrows[index];
			targetArrows.RemoveAt (index);
			GameObject.Destroy(arrow);
		}
	}

	private bool SystemSetup()
	{
		if (spacePath == null)
		{
			spacePath = GameObject.Find ("SpacePath");
			if (spacePath == null)
				return false;
		}
		if(shipDisplay == null)
		{
			shipDisplay = system ["Sensor"];

			if(shipDisplay == null)
				return false;
		}
		if(overCam == null)
		{
			overCam = shipDisplay.GetComponent<ShipDisplay>().DroneCamera();
		}
		if(overCam == null)
			return false;
		return true;
	}

	// Update is called once per frame
	protected override void Think () 
	{

	}


	void UpdateWaypoint(Vector3 position,GameObject waypoint)
	{
		Vector3 spacepoint = overCam.transform.position; 
		Vector3 upVector; 
		Vector3 forwardVector;
		
		if (!SystemSetup ()) return;
		PlayerController.controlMode cMode = PlayerController.controlMode.manual;
		if(pController as PlayerController != null)
			cMode = (pController as PlayerController).CurrentMode();
		
		
		if(cMode == PlayerController.controlMode.sensor)
		{
			// Make sure the arror is parented
			waypoint.transform.parent = overCam.transform;
			
			// Setting Next Position Values
			Vector3 nextPos = position;//spacePath.GetComponent<SpacePath>().GetDestinationPoint(spacepoint);
			Vector3 nextPosHigh =nextPos; 
			nextPosHigh.y = (overCam.transform.position + overCam.transform.forward * iconForwardDistance).y;
			
			// Displacement values
			Vector3 targetDisplacement = nextPos - overCam.transform.position;
			Vector3 lrDist = Vector3.Project(targetDisplacement,overCam.transform.right );
			Vector3 udDist = Vector3.Project(targetDisplacement,overCam.transform.up);
			
			// Scale the displacement into a position
			targetDisplacement = targetDisplacement.normalized*iconForwardDistance;
			
			if(lrDist.magnitude < lrBorder*widthFocusDistance  && udDist.magnitude < udBorder*heightFocusDistance)
			{
				waypoint.transform.position = overCam.transform.position + targetDisplacement + overCam.transform.up*0.05f;
				waypoint.transform.LookAt(waypoint.transform.position - overCam.transform.up*0.05f);
				//		nextWpArrow.transform.LookAt(nextPosHigh);
			}
			else if(lrDist.magnitude > lrBorder*widthFocusDistance)
			{
				waypoint.transform.position = overCam.transform.position + overCam.transform.forward * iconForwardDistance;
				waypoint.transform.position += lrDist.normalized*lrBorder;
				waypoint.transform.position -= Vector3.Project(targetDisplacement,overCam.transform.up);
				waypoint.transform.LookAt(nextPosHigh);
			}
			else if(udDist.magnitude > udBorder*heightFocusDistance)
			{
				waypoint.transform.position = overCam.transform.position + overCam.transform.forward * iconForwardDistance;
				waypoint.transform.position -= Vector3.Project(targetDisplacement,overCam.transform.right);
				waypoint.transform.position += udDist.normalized*udBorder;
				waypoint.transform.LookAt(nextPosHigh);
			}
			else
			{
				waypoint.transform.parent = overCam.transform;
				waypoint.transform.position = overCam.transform.position + overCam.transform.forward * iconForwardDistance;
				waypoint.transform.position += udDist.normalized*udBorder;
				waypoint.transform.position += lrDist.normalized*lrBorder;
				waypoint.transform.LookAt(nextPosHigh);
			}
		}
		if(cMode == PlayerController.controlMode.manual)
		{
			Vector3 relPos;
			Vector3 startPos;
			Camera shipCam = overCam.transform.parent.GetComponent<Camera>();

			waypoint.transform.parent = shipCam.transform;
			startPos = shipCam.transform.position 
				+ (position-shipCam.transform.position).normalized*iconForwardDistance;//shipCam.transform.position+ posRay.direction* ( iconForwardDistance + shipCam.nearClipPlane);

			relPos = (position-shipCam.transform.position).normalized*iconForwardDistance;

			Vector3 distUp = Vector3.Project(relPos,shipCam.transform.up);
			Vector3 distRight = Vector3.Project(relPos,shipCam.transform.right);
			Vector3 distFwd = Vector3.Project(relPos,shipCam.transform.forward);

			waypoint.transform.position = startPos;

			bool d_up = false;
			bool d_right = false;
			bool d_forward = false;

			Vector3 aim = Vector3.zero;

			if((distUp.normalized - shipCam.transform.up).magnitude < 1f)
				d_up = true;
			if((distRight.normalized - shipCam.transform.right).magnitude < 1f)
				d_right = true;
			if((distFwd.normalized - shipCam.transform.forward).magnitude < 1f)
				d_forward = true;


			if(d_forward == false)
			{
				if(distUp.magnitude > distRight.magnitude)
				{
					distUp/=distUp.magnitude;
					distRight/=distUp.magnitude;
				}
				else
				{
					distUp/=distRight.magnitude;
					distRight/=distRight.magnitude;
				}
			}

			waypoint.transform.position = shipCam.transform.position;
			waypoint.transform.position += shipCam.transform.forward*iconForwardDistance;

			if(distUp.magnitude> 0.12f && d_up == false)
			{
				aim += distUp.normalized;
				waypoint.transform.position += distUp.normalized * 0.12f; 
			}
			else if(distUp.magnitude> 0.4f && d_up == true)
			{
				aim += distUp.normalized;
				waypoint.transform.position += distUp.normalized * 0.4f; 
			}
			else
				waypoint.transform.position += distUp;

			if(distRight.magnitude > 0.45f && d_up == true && distUp.magnitude < 0.20f)
			{
				aim += distRight.normalized;
				waypoint.transform.position += distRight.normalized * 0.45f; 
			}
			else if(distRight.magnitude > 0.35f && d_up == true && distUp.magnitude > 0.20f)
			{
				aim += distRight.normalized;
				waypoint.transform.position += distRight.normalized * 0.35f; 
			}
			else if(distRight.magnitude > 0.3f )
			{
				aim += distRight.normalized;
				waypoint.transform.position += distRight.normalized * 0.3f; 
			}
			else
				waypoint.transform.position += distRight;

			if(aim.magnitude < 0.01f)
			{

				aim = -shipCam.transform.up;
				waypoint.transform.position += shipCam.transform.up*0.025f;
			}
			waypoint.transform.LookAt(waypoint.transform.position + aim,-shipCam.transform.forward);


		}
		


	}

	protected override void ThinkFast () 
	{
		UpdateWaypoint (spacePath.GetComponent<SpacePath> ().GetDestinationPoint (ship.transform.position), nextWpArrow);
		for (int i = 0; i < targetArrows.Count; i++)
		{
			UpdateWaypoint(targetObjects[i].transform.position,targetArrows[i]);
		}
	}
	
}