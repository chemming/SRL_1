using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponSubsystem : ShipSubsystem {

	ShipControlPanel panel;
	Blaster[] blasters;
	GameObject[] avatars;
	bool[] activeGun;
	int countBlaster;

	// Use this for initialization
	protected override void Initalize ()
	{
		countBlaster = SRLConfiguration.GetSettingInt ("weaponSubsystem_countBlaster", 2);

		SubDisplayName= "Weapon Management Console";
		SubDescription= "Manages the power flow to \n each weapon.";
		SubCostEnergyPassive=15f;
		SubShowOnPanel = true;
		SubStatus=Status.active;

		blasters = new Blaster[countBlaster];
		avatars = new GameObject[countBlaster];
		activeGun = new bool[countBlaster];
		panel = system["ControlPanel"].GetComponent< ShipControlPanel>();
		if(panel == null)
			throw new UnityException("Could not attach control panel");
		//Debug.Log("Weapon Init "+countBlaster);
		for(int i = 0; i < countBlaster;i++)
		{
			if(system["Blaster"+i.ToString()] != null)
			{
				blasters[i] = system["Blaster"+i.ToString()].GetComponent<Blaster>();
			}
			else
			{
				countBlaster = i; 
				break;
			}
			Transform top = this.transform.FindChild("InfoPanel").FindChild("W"+(i+1)+"_Icon");
			//Transform bottom = this.transform.FindChild("InfoPanel").FindChild("2"+(i+1));
			GameObject gunAvatar = Instantiate(blasters[i].transform.FindChild("Gun").gameObject) as GameObject;
			gunAvatar.transform.position = top.transform.position + this.transform.TransformDirection(new Vector3(0.08f,-0.013f,0));
			gunAvatar.transform.parent = this.transform;

			gunAvatar.transform.localScale = this.transform.TransformDirection(new Vector3(0.002f,0.02f,0.00016f));
			gunAvatar.GetComponent<Renderer>().material = buttonOffMaterial;

			if(i == 0)
			{
				avatars[i] = gunAvatar;
				activeGun[i] = true;
				avatars[i].GetComponent<Renderer>().material = buttonOnMaterial;
				panel.AddBlaster(blasters[i]);
			}
		}


		InitPanel(buttonOnMaterial,"W1_");
		InitPanel(buttonOnMaterial,"W2_");
		InitPanel(buttonOnMaterial,"W3_");
		InitPanel(buttonOnMaterial,"W4_");
		InitPanel(buttonOnMaterial,"W5_");
		InitPanel(buttonOnMaterial,"W6_");

	}
	
	private void InitPanel(Material mat,string prefix)
	{
		Transform panel = this.transform.FindChild("InfoPanel");
		foreach(Transform button in panel)
		{
			//Debug.Log(button.name + ">>" + prefix);
			if(button.name.StartsWith(prefix))
				button.GetComponent<Renderer>().material = mat;
		}
		
	}



	// Update is called once per frame
	protected override void Think ()
	{
		if (pController == null)
			return;
		ActionCode[] codes = new ActionCode[2]{ActionCode.weaponToggleSlot1,ActionCode.weaponToggleSlot2};
		for(int i =0;i<countBlaster;i++)
		{
			if(pController.GetAction(codes[i]))
			{
				if(activeGun[i]==false)
				{
					activeGun[i] = true;
					avatars[i].GetComponent<Renderer>().material = buttonOnMaterial;
					panel.AddBlaster(blasters[i]);
				}
				else
				{
					avatars[i].GetComponent<Renderer>().material = buttonOffMaterial;
					activeGun[i] = false;
					panel.RemoveBlaster(blasters[i]);
				}
			}
		}

	}
	// Update is called once per frame
	protected override void ThinkFast ()
	{
	}

}
