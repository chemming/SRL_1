using UnityEngine;
using System.Collections;

public class GrowMesh : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		this.transform.localScale *= 1.15f; 
	}
}
