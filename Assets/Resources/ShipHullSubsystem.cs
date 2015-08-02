using UnityEngine;
using System.Collections;

public class ShipHullSubsystem : ShipSubsystem {

	
	override protected void Initalize () 
	{
		g_displayName = "Solid Hull";
		this.g_displayDescription = "The Hull of the spaceship";
		this.g_activeEnergyCost = 0;
		this.g_activeMetalCost = 0;
		this.g_activeOxygenCost = 0;
		this.g_criticalSystem = true;
		this.g_maxEnergy = 0;
		this.g_maxMetal = 0;
		this.g_maxOxygen = 0;
		this.g_maxHealth = 1000;
		this.g_health = 1000;
	}
	override protected void Think () 
	{
//		Debug.Log (SubHealth);
	}
	override protected void ThinkFast () {}
}