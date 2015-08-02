using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipControlPanel : ShipSubsystem {

	ShipAutopilot ap;
	List<Blaster> blasters;

	override protected void Initalize ()
	{
		SubDisplayName= "Control Panel";
		SubDescription= "Manages and filters pilot \n input.";
		SubCostEnergyPassive=10f;
		SubShowOnPanel = true;
		SubStatus=Status.active;

		blasters = new List<Blaster> ();

	}

	public void AddBlaster(Blaster blast)
	{
		if(!blasters.Contains(blast))
			blasters.Add (blast);
	}

	public void RemoveBlaster(Blaster blast)
	{
		if(blasters.Contains(blast))
			blasters.Remove (blast);
	}


	void ProcessHumanInput()
	{
		PlayerController.controlMode cMode = PlayerController.controlMode.manual;
		if(pController as PlayerController != null)
			cMode = (pController as PlayerController).CurrentMode();
			
		
		//Menu
		if (cMode== PlayerController.controlMode.menu 
		    && 
		    (this.transform.parent.parent.FindChild("Screen").gameObject
		 .GetComponent<MeshRenderer>() as MeshRenderer).enabled == true)
		{
			ap.SetControlMode(false);
			(this.transform.parent.parent.FindChild("Screen").gameObject.GetComponent<MeshRenderer>() as MeshRenderer).enabled = false;
			(this.transform.parent.parent.FindChild("ScreenFrame").gameObject.GetComponent<MeshRenderer>() as MeshRenderer).enabled = false;
			//(this.transform.parent.parent.FindChild("Windshield").gameObject.GetComponent<MeshRenderer>() as MeshRenderer).enabled = false;
			(this.transform.parent.parent.FindChild("MountSensor").FindChild("Sensor").GetComponent<ShipDisplay>() as ShipDisplay).Deactivate();
			ap.SetMovementTarget (Vector3.zero );
		}
		

		//Manual
		if (cMode == PlayerController.controlMode.manual 
		    && 
		    (this.transform.parent.parent.FindChild("Screen").gameObject
		 .GetComponent<MeshRenderer>() as MeshRenderer).enabled == true)
		{
			ap.SetControlMode(true);
			(this.transform.parent.parent.FindChild("Screen").gameObject.GetComponent<MeshRenderer>() as MeshRenderer).enabled = false;
			(this.transform.parent.parent.FindChild("ScreenFrame").gameObject.GetComponent<MeshRenderer>() as MeshRenderer).enabled = false;
			(this.transform.parent.parent.FindChild("MountSensor").FindChild("Sensor").GetComponent<ShipDisplay>() as ShipDisplay).Deactivate();
		}
		
		//Sensor
		if (cMode == PlayerController.controlMode.sensor 
		    && 
		    (this.transform.parent.parent.FindChild("Screen").gameObject
		 .GetComponent<MeshRenderer>() as MeshRenderer).enabled == false)
		{
			ap.SetControlMode(false);
			(this.transform.parent.parent.FindChild("Screen").gameObject.GetComponent<MeshRenderer>() as MeshRenderer).enabled = true;
			(this.transform.parent.parent.FindChild("ScreenFrame").gameObject.GetComponent<MeshRenderer>() as MeshRenderer).enabled = true;
			(this.transform.parent.parent.FindChild("MountSensor").FindChild("Sensor").gameObject.GetComponent<ShipDisplay>() as ShipDisplay).Activate();
		}

	}

	void ProcessGameInput()
	{
		if (nexus.NexusHealth <= 0)
			return;

		if (pController.IsLocalPlayer ())
			ProcessHumanInput ();
		
		Vector3 targetVelocity = Vector3.zero;
		ap.SetMovementTarget (pController.GetTargetDisplacement ());

		if (pController.GetAction(ActionCode.fire1)==true)
		{
			foreach(Blaster b in blasters)
				b.Fire();
		}

	}

	override protected void Think ()
	{
		if (system ["ShipAutopilot"] == null)
			return;

		if(ap == null)
		{
			ap = system ["ShipAutopilot"].GetComponent<ShipAutopilot> () as ShipAutopilot; 
		}
		if(ap == null)
		{
			Debug.Log("Could not attach control system to pilot system");
			return;
		}

		ProcessGameInput ();
	}



	override protected void ThinkFast ()
	{
		if (ap == null)
			return;


		ap.SetOrientationUp (pController.GetUpVector());
		ap.SetOrientationTarget (pController.GetTargetOrientation());
	}
}
