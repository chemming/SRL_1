using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AIController : ShipSubsystem , IGeneralShipController
{
	Hashtable commandStatus;
	Hashtable actionMapping;

	Vector3 shipUp;

	Vector3 targetVelocity;
	Vector3 targetOrientation;
	bool shoot = false; 
	float thinkTime;
	static protected List<AIController> aiList = new List<AIController>();

	Vector3 initalPos;

	override protected void Initalize () 
	{
		g_displayName = "AI  Interface";
		this.g_displayDescription = "Controls the Robot";
		this.g_activeEnergyCost = 0;
		this.g_activeMetalCost = 0;
		this.g_activeOxygenCost = 0;
		this.g_criticalSystem = true;
		this.g_status = Status.active;
		this.g_showOnPanel = false;
		this.g_maxEnergy = 0;
		this.g_maxMetal = 0;
		this.g_maxOxygen = 0;
		this.g_maxHealth = 1;
		this.g_health = 1;

		targetOrientation = Vector3.zero;
		targetVelocity = Vector3.zero;
		initalPos = this.gameObject.transform.position;
		aiList.Add (this);
	}

	
	public Vector3 GetTargetDisplacement()
	{
		return targetVelocity;
	}
	
	public Vector3 GetTargetOrientation()
	{
		return targetOrientation;
	}
	
	public bool GetAction(ActionCode code)
	{
		if(code == ActionCode.fire1 || code == ActionCode.fire2)
		{
			if(shoot == true)
				return true;
		}
		return false;
	}

	public Vector3 GetUpVector()
	{
		return shipUp;
	}
	
	override protected void Think () 
	{
		for (int i=0; i <aiList.Count; i++)
		{
			if(aiList[i] == null|| aiList[i].ship == null)
			{
				aiList.RemoveAt(i);
				i--;
			}
		}

		float speedFactorShoot = 1f;

		GameObject player = GameObject.Find ("Robot");
		Vector3 closest = Vector3.one * 200f;
		if (player == null)
			return;

		foreach(AIController buddy in aiList)
		{

			if((buddy.ship.transform.position - ship.transform.position).magnitude < closest.magnitude
			   && buddy!= this)
				closest = buddy.ship.transform.position - ship.transform.position;
		}
		

		if (player == null)
			return;
		if (player.transform == null)
			return;

		Vector3 playerPosShoot = player.transform.position 
			+ ((ship.GetComponent<Rigidbody>().velocity - player.GetComponent<Rigidbody>().velocity)) * speedFactorShoot;
		Vector3 playerPosTrack = player.transform.position
			+ player.GetComponent<Rigidbody>().velocity.normalized*65f;

		Debug.DrawLine (player.transform.position, playerPosTrack, Color.red, 0.1f);

		targetOrientation = playerPosShoot - ship.transform.position;

		shoot = false;
		if((ship.transform.forward - targetOrientation.normalized).magnitude < 0.3f
		   && (ship.transform.position - playerPosShoot).magnitude < 100f)
		{
			shoot = true;
		}

		shipUp = new Vector3 (0, 1, 0);
		Vector3 direction = playerPosTrack -  ship.transform.position; //- ship.rigidbody.velocity;
		Vector3 goal = direction - (ship.GetComponent<Rigidbody>().velocity - player.GetComponent<Rigidbody>().velocity)*9f;// - direction.normalized * 15f;// + player.rigidbody.velocity;

		if (closest.magnitude < 35f)
			goal += -closest.normalized * 35f; 
		targetVelocity =  goal ;

	}
	
	override protected void ThinkFast () 
	{


	}
	
	
	public bool IsLocalPlayer()
	{
		return false;
	}
	public bool IsRemotePlayer()
	{
		return false;
	}

	
}
