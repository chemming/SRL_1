using UnityEngine;
using System.Collections;

public class SparksTiny : MonoBehaviour {

	float time = 0; 
	float dieTime = 1.8f; 
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		time += Time.deltaTime;
//		Debug.Log (time);
		if (time > dieTime)
			Destroy (this.gameObject);
	}
}
