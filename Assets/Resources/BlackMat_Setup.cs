﻿using UnityEngine;
using System.Collections;

public class BlackMat_Setup : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		gameObject.GetComponent<Renderer>().material.color = Color.black;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
