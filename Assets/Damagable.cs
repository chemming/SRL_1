using UnityEngine;
using System.Collections;

public class Damagable : MonoBehaviour, ITakeDamage 
{
	float health = 100f;
	float maxHealth = 100f;
	private static GameObject st_explosionDefault = Resources.Load("Environment/Explosion") as GameObject;

	// Use this for initialization
	void Start () 
	{
		health = 100f;
		maxHealth = 100f;
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	public void DoDamage(float number)
	{
		health -= number;
		if (health <= 0)
			Die ();
	}

	public void DoDamage(float number, Collision collision)
	{
		health -= number;
		if (health <= 0)
			Die ();
	}
	
	public void DoHeal(float number)
	{

	}
	public void DoHeal(float number, Collision collision)
	{

	}

	public void OnCollisionEnter(Collision collision) 
	{
		if (collision.gameObject.GetComponent<Rigidbody>() == null)
				return;
		if (collision.relativeVelocity.magnitude > 10f)
		{
			DoDamage (collision.gameObject.GetComponent<Rigidbody>().mass*collision.relativeVelocity.magnitude, collision);
		}
	}

	public void Die()
	{
		if(st_explosionDefault != null)
		{
			GameObject explosion = (GameObject.Instantiate(st_explosionDefault) as GameObject);
			explosion.transform.position = this.transform.position;
			explosion.transform.rotation = this.transform.rotation;
			explosion.transform.localScale *= 7;
		}
		GameObject.Destroy (this.gameObject);
	}

	public float GetHealth()
	{
		return health;

	}

}
