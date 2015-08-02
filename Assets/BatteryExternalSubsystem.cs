using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BatteryExternalSubsystem : ShipSubsystem {
	
	// Use this for initialization
	List<GameObject> healthBars;
	protected override void Initalize () 
	{
		healthBars = new List<GameObject> ();
		SubDisplayName= "Battery Subsystem <External>";
		SubDescription= "Stores Energy";
		SubCostEnergyPassive=0f;
		SubShowOnPanel = true;
		SubStatus=Status.active;
		SubStoreMaxEnergy = 50000;
		SubStoreEnergy = 50000;
		//SubStoreMaxEnergy = 5000;
		//SubStoreEnergy = 5000;

	}

	
	// Update is called once per frame
	protected override void Think () 
	{
	}
	
	protected override void ThinkFast () 
	{
	}
	
}
