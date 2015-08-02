using UnityEngine;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour {

	GameObject c;
	GameObject selected;
	List<GameObject> optionList;
	GameObject menuRoot;
	GameObject stateManager;

	GameObject gameCamera;


	// Use this for initialization
	void Start ()
	{
		selected = null;
		optionList = new List<GameObject> ();
	}

	// Update is called once per frame
	void Update () 
	{

		if(optionList.Count == 0)
		{
			gameCamera = GameObject.Find("ShipCamera");
	
			menuRoot = GameObject.Find("MenuRoot");
			stateManager = GameObject.Find("StateManager");
			optionList.Add(GameObject.Find("NewGame"));
			optionList.Add(GameObject.Find("Continue"));
			optionList.Add(GameObject.Find("SaveQuit"));

		}
		//if(gameCamera == null)
		//	gameCamera = GameObject.Find("ShipCamera");
		gameCamera = GameObject.Find("ShipCamera");

		if(gameCamera != null)
		{
			menuRoot.transform.position = gameCamera.transform.position;
			menuRoot.transform.rotation = gameCamera.transform.rotation;
		}

		if(c == null)
			c = GameObject.Find("MenuCamera");

		if (c == null)
			return;

		Ray ray = c.GetComponent<Camera>().ScreenPointToRay (Input.mousePosition);
		//RaycastHit hita;

		Color colorSelect = new Color (1f, 148f/255f, 148f / 255f, 1);
		Color colorHighlight = new Color (1f, 1f, 148f / 255f, 1);
		Color colorNormal = new Color (1f, 1f, 1f, 1f);

		foreach(GameObject obj in optionList)
		{
			if(obj != selected)
				(obj.GetComponent<TextMesh>()).color = colorNormal;
		}

		foreach(RaycastHit hit in Physics.RaycastAll( ray  ,300.0f) )
		{

			if(hit.collider != null && hit.collider.gameObject.tag == "menuOption")
			{
				if(Input.GetMouseButton(0) == false && 
				   selected == hit.collider.transform.gameObject)
					ProcessMenuButton(selected);

				if(Input.GetMouseButton(0))
				{
					selected = hit.collider.transform.gameObject;
					hit.collider.transform.gameObject
						.GetComponent<TextMesh>().color =colorSelect;
				}
				else
					hit.collider.transform.gameObject
						.GetComponent<TextMesh>().color =colorHighlight;
			}
		}

		if(Input.GetMouseButton(0) == false)
		{
			selected = null;
		}
	}

	private void ProcessMenuButton(GameObject buttonClicked)
	{
		if(buttonClicked == GameObject.Find("NewGame"))
			(stateManager.GetComponent<StateManager>()).RequestState("NewGame");
		if(buttonClicked == GameObject.Find("Continue"))
			(stateManager.GetComponent<StateManager>()).RequestState("Continue");
		if(buttonClicked == GameObject.Find("SaveQuit"))
			(stateManager.GetComponent<StateManager>()).RequestState("SaveQuit");
	}
}
