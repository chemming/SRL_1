using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatusSubsystem : ShipSubsystem {
	
	// Use this for initialization
	List<GameObject> healthBars;
	List<GameObject> energyBars;

	protected override void Initalize () 
	{
		healthBars = new List<GameObject> ();
		energyBars = new List<GameObject> ();
		SubDisplayName= "Status Panel";
		SubDescription= "Reports the current status \n of SAM subsystems.";
		SubCostEnergyPassive=2f;
		SubShowOnPanel = true;
		SubStatus=Status.active;
		InitPanel(buttonRedMaterial,"Health");
		InitPanel(buttonOnMaterial,"Oxygen");
		InitPanel(buttonOnMaterial,"Shield");
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
				//if(prefix == "Oxygen")
				//	oxygenBars.Add(button.gameObject);
				button.GetComponent<Renderer>().material = mat;
			}

		}

	}

	// Update is called once per frame
	protected override void Think () 
	{
		if (system ["Nexus"] == null || system ["Body"] == null)
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
	}
	
	protected override void ThinkFast () 
	{
	}
	
}
