//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using UnityEngine;
using System.Collections.Generic;
public class GameWorldState : IWorldState
{


	bool isShown = false;
	WorldCreate.WorldType wType;
	public GameWorldState (WorldCreate.WorldType type)
	{
		wType = type;
	}

	public void Create()
	{
		Cursor.visible = false;
		WorldCreate worldConstructor = new WorldCreate ();
		worldConstructor.LoadWorld (wType);
		isShown = true;

	}

	public void Teardown()
	{
		List<Transform> rootObjects = new List<Transform>();

		foreach (Transform t in GameObject.FindObjectsOfType<Transform>())
		{
			if(!rootObjects.Contains(t.root))
				rootObjects.Add(t.root);
		}

		int i = 0;
		foreach(Transform dest in rootObjects)
		{
			if(dest.gameObject.name != "StateManager" && dest.gameObject.name != "MenuRoot")
			{
				dest.name = "XXXDestroy" + (i++);
				GameObject.Destroy(dest.gameObject);
			}
		}

		isShown = false;
	}
	
	public void Pause()
	{
		(GameObject.Find ("World").GetComponent<WorldThink>() as WorldThink).SetRunning(false);

	}
	public void Run()
	{
		Cursor.visible = false;
		(GameObject.Find ("World").GetComponent<WorldThink>() as WorldThink).SetRunning(true);
	}
	
	public void Hide()
	{
		isShown = false;

	}

	public void Show(bool isOverlay)
	{
		Cursor.visible = false;
		isShown = true;

	}

	public void Update()
	{

	}
}

