using UnityEngine;
using System.Collections;

public class BulletDecay : MonoBehaviourThink 
{
	int delay = 0;
	public int maxDelay = 100;
	LineRenderer lineToUpdateSource;
	LineRenderer lineToUpdateDestination;
	private static GameObject st_explosionParticle = Resources.Load("BlastExplosion") as GameObject;
	private static GameObject st_explosionParticleRed = Resources.Load("BlastExplosionRed") as GameObject;

	public string type = "blue";
	float damagePower = 0;

	override protected void Initalize () 
	{
		damagePower = 500f;
	}


	override protected void Think () 
	{
		delay ++;
		//delay = delay % maxDelay;
		if (delay < maxDelay)
			return;
		Destroy (this.gameObject);

	}
	override protected void ThinkFast ()
	{
		if(lineToUpdateSource != null)
		{
			lineToUpdateSource.SetPosition(0,this.transform.position);
			float width = 0.3f*((float)((maxDelay-delay)/maxDelay));
		//	lineToUpdateSource.SetWidth(0,width);

		}
		if(lineToUpdateDestination != null)
			lineToUpdateDestination.SetPosition(1,this.transform.position);
	} 
	public void SetLineForward(LineRenderer lineForward)
	{
		lineToUpdateSource = lineForward;
	}

	public void SetLineBackward(LineRenderer lineBackward)
	{
		lineToUpdateDestination = lineBackward;
	}

	public void OnCollisionEnter(Collision collision) 
	{
		this.GetComponent<Rigidbody>().AddExplosionForce (10f, collision.contacts [0].point, 3f);
		GameObject part;// = GameObject.Instantiate (st_explosionParticle) as GameObject;
		if(type == "blue")
			part = GameObject.Instantiate (st_explosionParticle) as GameObject;
		else
			part = GameObject.Instantiate (st_explosionParticleRed) as GameObject;


		if(collision.gameObject.GetComponent<Damagable>() != null)
		{
			collision.gameObject.GetComponent<Damagable>().DoDamage(damagePower,collision);
		}
		if(collision.gameObject.GetComponent<ShipSystem>() != null)
		{
//			Debug.Log(damagePower);
			collision.gameObject.GetComponent<ShipSystem>().DoDamage(damagePower,collision);
		}		
		part.transform.position = collision.contacts [0].point;
		Vector3 orientation = Vector3.Cross (collision.contacts [0].normal, this.transform.up);
		part.transform.rotation = Quaternion.LookRotation(orientation,this.transform.up);
		GameObject.Destroy (part, 1f);
		GameObject.Destroy (this.gameObject);
		//	Debug.Log("Particle Hit");
		
	}


}
