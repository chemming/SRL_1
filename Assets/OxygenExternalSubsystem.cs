using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OxygenExternalSubsystem : ShipSubsystem {
	
	// Use this for initialization
	protected override void Initalize () 
	{
		SubDisplayName= "Oxygen Tank Subsystem <External>";
		SubDescription= "Liquid oxygen. Do not depelete. \n Recharge on M class planets.";
		SubCostEnergyPassive=0f;
		SubShowOnPanel = true;
		SubStatus=Status.active;
		SubStoreMaxOxygen = 1000;
		SubStoreOxygen = 1000;

	}
	
	
	// Update is called once per frame
	protected override void Think () 
	{
	}
	
	protected override void ThinkFast () 
	{
	}
	
}
