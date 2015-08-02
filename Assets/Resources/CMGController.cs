using UnityEngine;
using System.Collections;

public class CMGController : ThrusterController
{

	
	// Use this for initialization
	protected override void Initalize () 
	{
		SubDisplayName= "CMG Rotor";
		SubDescription= "Generates Angular Momentum to \n Orient the ship.";
		SubCostEnergyPassive=2f;
		SubShowOnPanel = true;
		SubStatus=Status.active;

		currentPower = 0;
		maxPower = 70;
		goalPower = 0;
		jet = null;
		//ship = this.transform.parent.parent.parent.parent.gameObject;
		
	}
	
	// Update is called once per frame
	protected override void Think ()
	{
		currentPower = goalPower;
		if(currentPower != 0)
		{
			//Debug.Log(currentPower);
			ship.GetComponent<Rigidbody>().AddTorque(currentPower*this.transform.up,ForceMode.Force);
			currentPower = 0;
		}
	}
	protected override void DoDisable()
	{
		goalPower = 0;
	}

	
	// Update is called once per frame
	protected override void ThinkFast ()
	{
		
	}
}
