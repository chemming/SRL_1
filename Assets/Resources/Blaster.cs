using UnityEngine;
using System.Collections;

public class Blaster : ShipSubsystem 
{
	protected int delay = 0;
	protected int maxDelay = 10;
	protected bool pressingFire = false;
	protected bool fireSignal = false;

	protected GameObject emitter;
	override protected void Initalize () 
	{
		SubDisplayName= "Energy Blaster";
		SubDescription= "Converts Energy into Energy \n Noodles.";
		SubCostEnergyPassive=10f;
		SubShowOnPanel = true;
		SubStatus=Status.active;
		SubCostEnergyActive = 80f;
//		SubCostOxygenActive = 800f;
//		SubCostMetalActive = 800f;

		emitter = this.transform.FindChild("Emitter").gameObject;
		g_name = "Orb Blaster";

	}
	protected virtual   void  startBlaster()
	{
	}
	protected virtual  void  stopBlaster()
	{
	}

	protected  virtual void fireBlaster()
	{
//		Debug.Log(this.GetType().Name);
		float fwdAmt = 3f;
		float fwdVelo = 80f;

		//Debug.Log("SHOOTING");
		GameObject obj = Instantiate(Resources.Load("Blast")) as GameObject;
		obj.GetComponent<BulletDecay>().type = "blue";

		Physics.IgnoreCollision(obj.GetComponent<Collider>(), ship.GetComponent<Collider>());
		
		obj.transform.position = emitter.transform.position + ship.transform.forward*fwdAmt;
		//Debug.DrawLine(obj.transform.position,obj.transform.position + ship.transform.forward*fwdAmt,Color.green,0.1f);
		obj.GetComponent<Rigidbody>().velocity = ship.transform.forward*fwdVelo + 
			ship.GetComponent<Rigidbody>().GetPointVelocity(obj.transform.position);
		
		
		obj.transform.forward = ship.transform.forward;
		obj.transform.up = emitter.transform.up;
		
	}

	public void Fire()
	{
		fireSignal = true;
	}

	override protected void Think () 
	{
		if(fireSignal)
		{
			fireSignal = false;
			if (pressingFire == false)
				startBlaster();
			pressingFire = true;
		}
		else
		{
			if (pressingFire == true)
				stopBlaster();
			pressingFire = false;
		}

		if ((pressingFire == true) && (delay == 1))
		{
			nexus.AddEnergyCharge(-this.SubCostEnergyActive);
			//nexus.AddMetalCharge(-this.SubCostMetalActive);
			//nexus.AddOxygenCharge(-this.SubCostOxygenActive);
			delay ++;
			fireBlaster();
		}
		//else
		//	delay = 0;
		if(delay != 1)
		{
			delay ++;
			delay = delay % maxDelay;
		}

	}
	override protected void ThinkFast ()
	{


	} 
}
