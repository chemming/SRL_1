using UnityEngine;
using System.Collections;

public class StateManager : MonoBehaviour 
{

	IWorldState mainMenuState;
	IWorldState gameState;
	string toLoad;

	IWorldState activeState;

	void Start () 
	{
		//Screen.SetResolution(640, 480, true);
		Screen.SetResolution(320, 200, true);

		mainMenuState = new MenuWorldState();
		mainMenuState.Create();
		mainMenuState.Run();
		toLoad = "";

	}
	
	// Update is called once per frame
	void Update () 
	{
		if(toLoad.Length > 1)
			RequestState(toLoad);

		if(mainMenuState != null)
			mainMenuState.Update();

		if(Input.GetKey(KeyCode.X))
		{
			RequestState("GameMenu");
		}
	}

	public void RequestState(string stateIn)
	{

		//return;
		if(stateIn == "Menu")
		{
			if(gameState != null)
			{
				gameState.Pause();
				gameState.Hide();
				gameState.Teardown();
				gameState = null;
			}
			mainMenuState.Show(false);
			mainMenuState.Run();

		}
		if(stateIn == "GameMenu")
		{
			if(gameState != null)
			{
				gameState.Pause();
			}

			mainMenuState.Show(true);
			mainMenuState.Run();
			
		}
		if(stateIn == "Continue")
		{
			if(gameState != null)
			{
				gameState.Run();
				gameState.Show(false);
				mainMenuState.Hide();
				mainMenuState.Pause();
			}

		}

		if(stateIn == "NewGame")
		{
			if(gameState != null)
			{
				gameState.Hide();
				gameState.Teardown();
				gameState = null;
			}
			toLoad = "load_Game";
		}

		if(stateIn == "SaveQuit")
		{
			if(gameState != null)
			{
				gameState.Hide();
				gameState.Teardown();
				gameState = null;
			}
			mainMenuState.Teardown();
			mainMenuState.Hide();
//			mainMenuState = null;
			Application.Quit();
		}

		//TOLOAD
		if(stateIn == "load_Game")
		{
			toLoad = "";
			mainMenuState.Hide();
			gameState = new GameWorldState(WorldCreate.WorldType.debug_game);
			gameState.Create();
			gameState.Run();
		}


	}
}
