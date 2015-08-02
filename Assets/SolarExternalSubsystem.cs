using UnityEngine;
using System.Collections;

public class SolarExternalSubsystem : ShipSubsystem 
{

	// Use this for initialization
	protected override void Initalize () 
	{
		SubDisplayName= "Solar Panel <External>";
		SubDescription= "Extract energy automagically. Works best in lit environments.";
		SubCostEnergyPassive=-120f;
		SubShowOnPanel = true;
		SubStatus=Status.active;
//		SubStoreMaxEnergy = 500000;
//		SubStoreEnergy = 500000;
	}
	
	
	// Update is called once per frame
	protected override void Think () 
	{
	}
	
	protected override void ThinkFast () 
	{
	}
}
