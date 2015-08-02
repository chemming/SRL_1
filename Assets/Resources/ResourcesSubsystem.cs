using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ResourcesSubsystem : ShipSubsystem {
	
	// Use this for initialization
	List<GameObject> healthBars;
	List<GameObject> energyBars;

	List<GameObject> oxygenBars;
	List<GameObject> metalBars;


	List<GameObject> chargeBars;


	protected override void Initalize () 
	{
		healthBars = new List<GameObject> ();
		energyBars = new List<GameObject> ();
		chargeBars = new List<GameObject> ();

		oxygenBars = new List<GameObject> ();
		metalBars = new List<GameObject> ();


		SubDisplayName= "Resources Panel";
		SubDescription= "Reports the current level \n of resources held by SAM.";
		SubCostEnergyPassive=2f;
		SubShowOnPanel = true;
		SubStatus=Status.active;

		InitPanel(buttonOnMaterial,"Oxygen");
		InitPanel(buttonPressMaterial,"Metal");
		InitPanel(buttonOnMaterial,"Battery");
		InitPanel(buttonPressMaterial,"Charge");

	}

	private void InitPanel(Material mat,string prefix)
	{
		Transform panel = this.transform.FindChild("InfoPanel");
		foreach(Transform button in panel)
		{

			//Debug.Log(button.name + ">>" + prefix);
			if(button.name.StartsWith(prefix))
			{
				if(prefix == "Health")
					healthBars.Add(button.gameObject);
				if(prefix == "Battery")
					energyBars.Add(button.gameObject);
				if(prefix == "Charge")
					chargeBars.Add(button.gameObject);
				if(prefix == "Metal")
					metalBars.Add(button.gameObject);
				if(prefix == "Oxygen")
					oxygenBars.Add(button.gameObject);

				button.GetComponent<Renderer>().material = mat;
			}
		}
		
	}

	// Update is called once per frame
	protected override void Think () 
	{
		if(system["Nexus"] == null || system ["Body"] == null)
			return;
		NexusExternalSubsystem nex = system["Nexus"].GetComponent<NexusExternalSubsystem>();
		ShipSubsystem hull = system ["Body"].GetComponent < ShipHullSubsystem>() as ShipSubsystem;
		
		float percentHealth = nex.NexusHealth / nex.NexusMaxHealth;
		//float percentHull = hull.SubHealth / hull.SubMaxHealth;
		int index = (int)(percentHealth* (float)(healthBars.Count - 1));
		
		for(int i=0; i<healthBars.Count;i++)
		{
			Renderer rend = healthBars[i].GetComponent<Renderer>();
			if(i < index)
				rend.material = buttonRedMaterial;
			else
				rend.material = buttonOffMaterial;
		}
		
		float percentEnergy = nex.NexusEnergy / nex.NexusMaxEnergy;
		index = (int)(percentEnergy* (float)(energyBars.Count - 1));
		
		for(int i=0; i<energyBars.Count;i++)
		{
			Renderer rend = energyBars[i].GetComponent<Renderer>();
			if(i < index)
				rend.material = buttonOnMaterial;
			else
				rend.material = buttonOffMaterial;
		}

		float chargeLevel = nex.NexusCharge / 50f;
		int indexGoal = 0;
		int indexFrom = 1;
		int indexCenter = chargeBars.Count / 2;
		Material chargeMat;
		if( chargeLevel < 0)
		{
			chargeMat = buttonRedMaterial;
			indexFrom = (int)(indexCenter + chargeLevel);
			if(indexFrom < 0)
				indexFrom = 0;
			indexGoal = indexCenter;
		}
		else
		{
			chargeMat = buttonPressMaterial;
			indexFrom = indexCenter;
			indexGoal = (int)(indexCenter+chargeLevel);
			if(indexGoal > chargeBars.Count-1)
				indexGoal = chargeBars.Count-1;

		}

		for(int i=0; i<chargeBars.Count;i++)
		{
			Renderer rend = chargeBars[i].GetComponent<Renderer>();
			rend.material = buttonOffMaterial;
			if(i <= indexGoal && i >= indexFrom)
				rend.material = chargeMat;
		}

	
		float percentMetal = nex.NexusMetal / nex.NexusMaxMetal;
		index = (int)(percentMetal* (float)(metalBars.Count - 1));
		for(int i=0; i<metalBars.Count;i++)
		{
			Renderer rend = metalBars[i].GetComponent<Renderer>();
			if(i < index)
				rend.material = buttonPressMaterial;
			else
				rend.material = buttonOffMaterial;
		}


		float percentOxygen = nex.NexusOxygen / nex.NexusMaxOxygen;
		index = (int)(percentOxygen* (float)(oxygenBars.Count - 1));
		for(int i=0; i<oxygenBars.Count;i++)
		{
			Renderer rend = oxygenBars[i].GetComponent<Renderer>();
			if(i < index)
				rend.material = buttonPressMaterial;
			else
				rend.material = buttonOffMaterial;
		}



	}
	
	protected override void ThinkFast () 
	{
	}
	
}
