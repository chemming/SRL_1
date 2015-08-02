using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldThink : MonoBehaviour 
{
	GameObject selectedRobot = null;
	float delay = 0.1f; 
	int worldIteration = 0;
	bool gameRunning = false;
	bool gameOver = false;
	int tempId = 0;

	public GameObject currentAvatar;

	private Rect guiRect;


	void Start ()
	{
		tempId = Random.Range (0, 100);
		Time.timeScale = 1;

		InvokeRepeating ("GameThink", delay, delay);
	}

	// Update is called once per 'think'
	void GameThink()
	{
		if(gameRunning == true)
		{
			worldIteration++;
			RunLevel();
		}



	}

	private void RunLevel()
	{
		//return;
		float draw = Random.Range (worldIteration, 1000);
		if(draw  > 990)
		{

			GameObject player = GameObject.Find ("Robot");
			if(player == null)
				return;

			GameObject newObj = CompoundObjectFactory.Create("AI",CompoundObjectFactory.COType.Enemy) as GameObject;

			newObj.transform.position = player.transform.position + player.transform.forward*
				(125f + Random.Range(0,160f));
			newObj.GetComponent<Rigidbody>().velocity = player.GetComponent<Rigidbody>().velocity;


			ShipSystem system = player.GetComponent<ShipSystem>();
			WaypointHud hud = (system["WaypointHud"].GetComponent<WaypointHud>()); 
			hud.TrackObject(newObj);
		}
	}
	public int GetWorldIteration()
	{
		return worldIteration;
	}

	public void SetRunning(bool state)
	{
		if(state == true)
			Time.timeScale = 1;
		else
			Time.timeScale = 0;
		gameRunning = state;
	}

	public bool GetRunning()
	{
		return gameRunning;
	}


	void Update () 
	{

	}
}
