using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MetalExternalSubsystem : ShipSubsystem {
	
	// Use this for initialization
	protected override void Initalize () 
	{
		SubDisplayName= "Metal Subsystem <External>";
		SubDescription= "Stores liquid metal found \n during scavanging ";
		SubCostEnergyPassive=0f;
		SubShowOnPanel = true;
		SubStatus=Status.active;
		SubStoreMaxMetal = 1000;
		SubStoreMetal = 1000;
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
