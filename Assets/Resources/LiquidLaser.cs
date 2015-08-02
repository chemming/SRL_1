using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LiquidLaser : Blaster 
{
	GameObject lastNode;

	protected override void Initalize()
	{
		base.Initalize ();

		SubDisplayName= "Liquid Energy Blaster";
		SubDescription= "Shoots Beams of Energy.";
		SubCostEnergyPassive=10f;
		SubShowOnPanel = true;
		SubStatus=Status.active;
	}


	protected override void  startBlaster()
	{
		maxDelay = 2;
	}
	protected  override void stopBlaster()
	{
		lastNode = null;
	}

	protected override void fireBlaster()
	{
		float fwdAmt = 3f;
		float fwdVelo = 120f;
		
		//Debug.Log("SHOOTING");
		GameObject obj = Instantiate(Resources.Load("LiquidNode")) as GameObject;
		obj.GetComponent<BulletDecay>().type = "red";

		Physics.IgnoreCollision(obj.GetComponent<Collider>(), ship.GetComponent<Collider>());
		obj.transform.position = emitter.transform.position + ship.transform.forward*fwdAmt;
		//Debug.DrawLine(obj.transform.position,obj.transform.position + ship.transform.forward*fwdAmt,Color.green,0.1f);
		obj.GetComponent<Rigidbody>().velocity = ship.transform.forward*fwdVelo + 
			ship.GetComponent<Rigidbody>().GetPointVelocity(obj.transform.position);
		
		
		obj.transform.forward = ship.transform.forward;
		obj.transform.up = emitter.transform.up;
		if(lastNode != null)
		{

			LineRenderer rendLn = obj.AddComponent<LineRenderer>() as LineRenderer;
			Material mat = Resources.Load("LaserLiquid") as Material;
			rendLn.material = mat;
			//rendLn.renderer.material.SetColor ("_SpecColor", Color.red);
			rendLn.castShadows = false;
			rendLn.SetWidth(1.2f,1.2f);
			rendLn.SetVertexCount(2);
			rendLn.SetPosition(0, obj.transform.position);
			rendLn.SetPosition(1, lastNode.transform.position);
			BulletDecay bdCompnentForward = obj.GetComponent<BulletDecay>();
			BulletDecay bdCompnentBackward = lastNode.GetComponent<BulletDecay>();

			bdCompnentForward.SetLineForward(rendLn);
			bdCompnentBackward.SetLineBackward(rendLn);
					

		}
		lastNode = obj;

	}
	protected override void ThinkFast ()
	{
		base.ThinkFast ();
	}


		
}
