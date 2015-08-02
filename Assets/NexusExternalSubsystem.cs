using UnityEngine;
using System.Collections;

public class NexusExternalSubsystem : ShipSubsystem {
	
	// Use this for initialization
	float updateDelay = 0.7f;
	float time = 0f;
	int ticks = 0;

	float nexusHealth = 10;
	float nexusMaxHealth = 10;
	float nexusEnergy = 10;
	float nexusMaxEnergy = 10;
	float nexusOxygen = 10;
	float nexusMaxOxygen = 10;
	float nexusMetal = 10;
	float nexusMaxMetal = 10;

	float nexusCharge = 0f; //Charge is simply the tracked rate of change of energy.
	float nexusMaxCharge = 500;

	float changeCharge = 0f;
	float changeOxygen = 0f;
	float changeMetal = 0f;

	protected override void Initalize () 
	{
		SubDisplayName= "Energy Nexus";
		SubDescription= "The Conduit which channels energy, \n metal and Oxygen. Vents Excess.";
		SubCostEnergyPassive=1f;
		SubShowOnPanel = true;
		SubCritical = true;
		SubStatus=Status.active;


	}

	// Update is called once per frame
	protected override void Think () 
	{
		
	}
	
	protected override void ThinkFast () 
	{
		time += Time.deltaTime;
		if (time > updateDelay)
		{
			ProcessThink();
			time = 0;
		}
	}

	protected void  ProcessThink()
	{
		nexusCharge = changeCharge;

		///Boooooooo duplicate code!
		float health = 0;
		float maxHealth = 0;

		float hullPercent = (system ["Body"].GetComponent<ShipSubsystem> ()).SubHealth
						/ (system ["Body"].GetComponent<ShipSubsystem> ()).SubMaxHealth;
		foreach(string key in system.Keys)
		{
			health += (system[key].GetComponent<ShipSubsystem>()).SubHealth; 
			maxHealth += (system[key].GetComponent<ShipSubsystem>()).SubMaxHealth; 
		}
		nexusHealth = health* hullPercent;
		nexusMaxHealth = maxHealth;


		UpdateNexusAttribute ("Energy");
		UpdateNexusAttribute ("Metal");
		UpdateNexusAttribute ("Oxygen");



	}

	//This function sucks because I didn't plan ahead
	//TODO: Generalize Energy,Oxygen, and Metal into general properties in the future, system wide
	private void UpdateNexusAttribute(string type)
	{
		float attribChange=0;
		//BOOOOOOOOOOO!!!!!
		float attrib = 0;
		float maxAttrib = 0;

		if (type == "Energy")
			attribChange = changeCharge;
		if (type == "Oxygen")
			attribChange = changeOxygen;
		if (type == "Metal")
			attribChange = changeMetal;

		foreach(string key in system.Keys)
		{

			float subAttrib=0;// = (system[key].GetComponent<ShipSubsystem>()).SubStoreEnergy;
			float subMaxAttrib=0;// = (system[key].GetComponent<ShipSubsystem>()).SubStoreMaxEnergy;
			if (type == "Energy")
			{
				 subAttrib = (system[key].GetComponent<ShipSubsystem>()).SubStoreEnergy;
				 subMaxAttrib = (system[key].GetComponent<ShipSubsystem>()).SubStoreMaxEnergy;
			}
			if (type == "Oxygen")
			{
				subAttrib = (system[key].GetComponent<ShipSubsystem>()).SubStoreOxygen;
				subMaxAttrib = (system[key].GetComponent<ShipSubsystem>()).SubStoreMaxOxygen;
			}
			if (type == "Metal")
			{
				subAttrib = (system[key].GetComponent<ShipSubsystem>()).SubStoreMetal;
				subMaxAttrib = (system[key].GetComponent<ShipSubsystem>()).SubStoreMaxMetal;
			}


			if(attribChange > 0 )
			{
				float subPotential = subMaxAttrib - subAttrib;
				if(subPotential >= subMaxAttrib)
				{
					subAttrib += changeCharge;
					attribChange = 0;
				}
				else // (changeCharge < subPotential)
				{
					attribChange -= subPotential;
					subAttrib = subMaxAttrib; //Fill up the tank.
				}
			}
			else if(attribChange < 0 )
			{
				if(subAttrib + attribChange > 0)
				{
					subAttrib += attribChange;
					attribChange = 0;
				}
				else
				{
					attribChange += subAttrib;
					subAttrib = 0;
				}
			}
			if (type == "Energy")
			{
				(system[key].GetComponent<ShipSubsystem>()).SubStoreEnergy =  subAttrib;
			}
			if (type == "Oxygen")
			{
				(system[key].GetComponent<ShipSubsystem>()).SubStoreOxygen =  subAttrib;
			}
			if (type == "Metal")
			{
				(system[key].GetComponent<ShipSubsystem>()).SubStoreMetal =  subAttrib;
			}			

			attrib += subAttrib; 
			maxAttrib += subMaxAttrib; 
		}
		attribChange = 0; // Nullify the remaining charge

		if (type == "Energy")
		{
			changeCharge = attribChange;
			nexusEnergy = attrib;
			nexusMaxEnergy = maxAttrib;		
		}
		if (type == "Oxygen")
		{
			changeOxygen = attribChange;
			nexusOxygen = attrib;
			nexusMaxOxygen = maxAttrib;		
		}
		if (type == "Metal")
		{
			changeMetal = attribChange;
			nexusMetal = attrib;
			nexusMaxMetal = maxAttrib;		
		}

		
	}


	public float NexusHealth
	{
		get 
		{
			return nexusHealth;
		}

	
	}
	public float NexusMaxHealth
	{
		get 
		{
			return nexusMaxHealth;
		}
		
		
	}

	public float NexusEnergy
	{
		get 
		{
			return nexusEnergy;
		}
	}
	public float NexusMaxEnergy
	{
		get 
		{
			return nexusMaxEnergy;
		}
	}

	public float NexusCharge
	{
		get 
		{
			return changeCharge;
		}
	}


	public float NexusOxygen
	{
		get 
		{
			return nexusOxygen;
		}
	}
	public float NexusMaxOxygen
	{
		get 
		{
			return nexusMaxOxygen;
		}
	}

	public float NexusMetal
	{
		get 
		{
			return nexusMetal;
		}
	}
	public float NexusMaxMetal
	{
		get 
		{
			return nexusMaxMetal;
		}
	}


	public bool AddEnergyCharge(float energy)
	{
		if (nexusEnergy + energy < 0)
			return false;
		changeCharge += energy;
		//Debug.Log (changeCharge);
		return true;
	}
	public bool AddMetalCharge(float metal)
	{
		if (nexusMetal + metal < 0)
			return false;
		changeMetal += metal;
		return true;
	}
	public bool AddOxygenCharge(float oxygen)
	{
		if( oxygen!=0)
			Debug.Log (nexusOxygen);
		if (nexusOxygen + oxygen < 0)
			return false;
		changeOxygen += oxygen;
		//Debug.Log (changeCharge);
		return true;
	}

}
