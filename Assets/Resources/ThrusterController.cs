using UnityEngine;
using System.Collections;

public class ThrusterController : ShipSubsystem
{
	protected float maxPower = 5;
	protected float goalPower;
	protected float currentPower;
	protected GameObject jet;
	//protected GameObject ship;

	// Use this for initialization
	protected override void Initalize () 
	{
		SubDisplayName= "Thruster Jet";
		SubDescription= "Converts Energy into force.";
		SubCostEnergyPassive=2f;
		SubCostEnergyActive=10f;

		SubShowOnPanel = true;
		SubStatus=Status.active;

		currentPower = 0;
		maxPower =5;
		goalPower = 0;
		jet = (this.transform.FindChild ("Jet").gameObject as GameObject);
		///ship = this.transform.parent.parent.parent.gameObject;
		/// 
		g_name = "Thruster";
	}
	protected override void DoDisable()
	{
		(jet.GetComponent<ParticleSystemRenderer>() as 
		 ParticleSystemRenderer)
			.enabled = false;
		goalPower = 0;
	}

	// Update is called once per frame
	protected override void Think ()
	{

		currentPower = goalPower;

		if (currentPower != 0)
		{
			(jet.GetComponent<ParticleSystemRenderer>() as 
			ParticleSystemRenderer)
			.enabled = true;
		}
		else
		{
			(jet.GetComponent<ParticleSystemRenderer>() as 
			ParticleSystemRenderer)
			.enabled = false;
		}


		if(currentPower != 0)
		{

			nexus.AddEnergyCharge(-this.SubCostEnergyActive*currentPower);
			//ship.rigidbody.AddForceAtPosition(jet.transform.forward * currentPower*(-1),jet.transform.position,ForceMode.Force);
			goalPower = 0; // after a pulse, lay off
		}
	}

	public float GetMaxPower()
	{
		return maxPower;
	}
	public void SetPower(float power)
	{
		this.goalPower = power;
	}

	public void SetPowerFraction(float fraction)
	{
		this.goalPower = this.maxPower*fraction;
	}

	// Update is called once per frame
	protected override void ThinkFast ()
	{
		
	}
}
