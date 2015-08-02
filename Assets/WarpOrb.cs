using UnityEngine;
using System.Collections.Generic;

public class WarpOrb : MonoBehaviour {
	List<GameObject> orbs;
	List<Vector3> rotations;
	GameObject warpingObject;
	// Use this for initialization
	void Start () 
	{
		warpingObject = null;
		orbs = new List<GameObject>();
		rotations = new List<Vector3>();
		foreach (Transform child in this.transform)
		{
			orbs.Add(child.gameObject);
			rotations.Add(new Vector3(
				Random.Range(-1,1),
				Random.Range(-1,1),
				Random.Range(-1,1)
				));
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		for(int i = 0; i < orbs.Count; i++)
		{
			orbs[i].transform.Rotate(rotations[i]);
		}
		if(warpingObject != null)
		{
			Vector3 warpDir = transform.position - warpingObject.transform.position;
			if(warpDir.magnitude > 4)
				warpingObject.GetComponent<Rigidbody>().velocity = warpDir.normalized*4;
			if(warpDir.magnitude < 0.5f)
			{
				StateManager stateManager = GameObject.Find ("StateManager").GetComponent<StateManager>();
				stateManager.RequestState("GameMenu");
			}
		}
	}

	void OnTriggerEnter(Collider other) 
	{
		//Debug.Log("Detected Collision");
		if (other.gameObject.name == "Robot")
		{
			other.gameObject.GetComponent<Rigidbody>().velocity *= 0.02f;
			warpingObject = other.gameObject;
		}

	}
}
