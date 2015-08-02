using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ShipDisplay : ShipSubsystem 
{

	public GameObject sensorTarget;
	List<SensorHologram> holograms;

	int rescanRate = 1;
	int updateHoloRate = 1;
	int time = 0;
	int timeMax = 1;
	int scanRange = 1;
	bool systemEnabled = false;

	float scale = 1f/10f;
	Vector3 displayOffset;
	Camera droneCamera;
	Camera shipCamera;

	// Use this for initialization
	protected override void Initalize () 
	{
		SubDisplayName= "Display System";
		SubDescription= "Displays the sensor feed  \n in 2D for the pilot.";
		SubCostEnergyPassive=20f;
		SubShowOnPanel = true;
		SubStatus=Status.active;

		holograms = new List<SensorHologram>();
		displayOffset = new Vector3 (0, 0.005f, 0);
		scale = 1f/50f;
		rescanRate = 10;
		updateHoloRate = 1;
		time = 0;
		timeMax = 1000;
		scanRange = 50;
		droneCamera = (GameObject.Instantiate(Resources.Load("OverheadCamera") as GameObject) 
		               as GameObject).GetComponent<Camera>();
		droneCamera.name = "OverheadCamera";

		shipCamera = GameObject.Find("ShipCamera").GetComponent<Camera>();
		droneCamera.transform.parent = shipCamera.transform;

	}

	public void Deactivate()
	{
		systemEnabled = false;
		KillHolograms ();
		shipCamera.depth = 10;
		droneCamera.depth = 1;
	}
	public void Activate()
	{
		shipCamera.depth = 1;
		droneCamera.depth = 10;
		systemEnabled = false;
	}


	public float GetScale()
	{
		return scale;
	}

	public void AttachSensor(GameObject obj)
	{
		if(obj == null)
			Debug.Log ("Attached Sensor to Null object!");
		sensorTarget = obj;
	}

	private void AdjustHologram(SensorHologram holo,bool force)
	{
		//clone.transform.position = new Vector3(0f,0f,0f);
		//clone.transform.position = this.gameObject.transform.position + displayOffset;
		//Find minimized velocites and scale
		GameObject detectedObj = holo.reading;
		if (detectedObj == null) //Whatever it died
						return;
		GameObject clone = holo.hologram;
		Vector3 objv = new Vector3 (0, 0, 0);
		Vector3 shpv = new Vector3 (0, 0, 0);
		if(detectedObj.GetComponent<Rigidbody>() != null)
		{
			objv = detectedObj.GetComponent<Rigidbody>().velocity;
			shpv = this.transform.parent.parent.parent.gameObject.GetComponent<Rigidbody>().velocity;
		}
		Vector3 shpr = this.gameObject.transform.rotation.eulerAngles;

		Vector3 relVel;
		Vector3 newPos;
		Vector3 newVec;

		//Find hologram pos
		newPos = (detectedObj.transform.position - this.gameObject.transform.position) * scale;


		//Find new velocity
		objv = new Vector3(objv.x * scale,objv.y*scale,objv.z*scale);
		shpv = new Vector3(shpv.x * scale,shpv.y*scale,shpv.z*scale);
		relVel = Quaternion.Euler(shpr.x, shpr.y, shpr.z) * (objv - shpv);
		newVec = this.transform.parent.parent.parent.gameObject.GetComponent<Rigidbody>().velocity+relVel;

		Quaternion ea = this.gameObject.transform.rotation;
		Quaternion cEa = detectedObj.transform.rotation;
		//clone.transform.rotation = Quaternion.Euler( cEa.x - ea.x, cEa.y- ea.y, cEa.z - ea.z);
		//relVel = Quaternion.Euler(shpr.x, shpr.y, shpr.z) * relVel;

		clone.transform.localPosition = newPos;
		clone.transform.rotation =  ea*cEa ;
		//clone.rigidbody.velocity = newVec;
		//clone.rigidbody.angularVelocity = detectedObj.rigidbody.angularVelocity; 
	}
	private void KillHolograms()
	{
		while (holograms.Count > 0)
		{
			SensorHologram holo = holograms[0];
			holograms.RemoveAt(0);
			Destroy(holo.hologram);
		}
		holograms.Clear();
	}


	private void DoScan()
	{
		KillHolograms ();
		//Delete old ones

		//Do a fresh scan, and create new ones
		GameObject[] found = FindGameObjectsInsideRange(sensorTarget.transform.position,scanRange);
		foreach(GameObject detectedObj in found)
		{
			if(detectedObj != this && detectedObj.tag == "robot") //&& detectedObj.tag != "hologram")
			{
				GameObject clone;

				//if(detectedObj.tag != "robot" )
				//	clone = Instantiate (detectedObj) as GameObject;
				//if(detectedObj.tag == "robot" )
				//{
					//clone = Instantiate (Resources.Load("RobotProject")) as GameObject;
					clone = CompoundObjectFactory.Create("Robot",CompoundObjectFactory.COType.Avatar);

					clone.name = "Avatar";
				//}

				clone.tag = "hologram";
				foreach(MonoBehaviourThink com in clone.GetComponents<MonoBehaviourThink>())
					com.enabled = false;

				clone.GetComponent<Collider>().enabled = false;
				clone.transform.parent = this.transform; // link the hologram to the display
				SensorHologram holo = new SensorHologram(detectedObj,clone);

				Vector3 scaleVec = detectedObj.transform.localScale;
				scaleVec = new Vector3(scaleVec.x * scale,scaleVec.y*scale,scaleVec.z*scale);
				clone.transform.localScale = scaleVec;

				holograms.Add(holo);
				AdjustHologram(holo,true);
			}
		}
		//sensorTarget = null;
	}

	private void UpdateHolograms()
	{
		foreach(SensorHologram holo in holograms)
			AdjustHologram(holo,false);
	}

	// Update is called once per frame
	override protected void ThinkFast ()
	{

		if(systemEnabled ==true)
		{
			//DroneCameraFollow();
			UpdateHolograms ();
		}
	}

	// Update is called once per frame
	protected override void Think () 
	{
		if(systemEnabled ==true)
		{
			time = (++time)%timeMax;
			bool rescan = (time%rescanRate) == 0;
			bool updateHolo = (time%updateHoloRate) == 0;

			if(sensorTarget != null && rescan == true)
				DoScan();

			if(sensorTarget != null && updateHolo == true)
				UpdateHolograms ();
		}

	}


	public GameObject DroneCamera()
	{
		return droneCamera.gameObject;
	}


	GameObject[] FindGameObjectsInsideRange ( Vector3 center, float radius )
	{
		Collider[] cols = Physics.OverlapSphere(center, radius);
		int q = cols.Length; // q = how many colliders were found
		int lastCol = 0;
		/*
		// pack the objects inside range in the beginning of array cols:
		for (int i = 0; i < q; i++)
		{
			float dist = Vector3.Distance(center, cols[i].transform.position);
			if (dist <= radius)
			{ // if object inside range...
				cols[lastCol++] = cols[i]; // save it at the next available position
			}
		}
		// create return array:
		var gos: GameObject[] = new GameObject[lastCol];
		// copy the game objects inside range to it:
		for (i = 0; i < lastCol; i++){
			gos[i] = cols[i].gameObject;
		}
		*/

		GameObject[] goa = new GameObject[cols.Length];
		for (int i = 0; i < cols.Length; i++)
		{
			goa[i] = cols[i].gameObject;
		}
		return goa; // return the GameObject array
	}
}

