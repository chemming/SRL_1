using UnityEngine;
using System.Collections;
/*
     
    ShipAutopilot:

	This autopilot constantly works, using 
	a ships actuators, to obtain a desired
	displacement and orientation.
	

 */
public abstract class MonoBehaviourThink : MonoBehaviour {
	
	// Use this for initialization
	protected int iteration;
	protected int worldIteration;
	protected WorldThink world;
	private bool initalizedIsDone = false; 
	private bool thinkingEnabled = true; 



	protected void Start () 
	{
	//	thinkingEnabled = true;
		iteration = 0;
		worldIteration = 0;
		world = GameObject.Find ("World").GetComponent<WorldThink>();
//		Debug.Log (this.name);
	}

	// Update is called once per frame
	protected void Update () 
	{
		if (thinkingEnabled == false)
			return;

		if(initalizedIsDone == false)
		{
			Initalize ();
			initalizedIsDone = true;
		}
		if(world == null)
		{
			world = GameObject.Find ("World").GetComponent<WorldThink>();
			if(world == null)
			{
				Debug.Log(this.GetType().Name);
				Debug.Log (world);
			}
		}
		worldIteration = world.GetWorldIteration ();
		if (iteration < worldIteration) 
		{
			ThinkAlpha();
			Think();
			iteration++;
		}
		if(world.GetRunning())
			ThinkFast ();

	}


	public void SetWorldIteration(int worldIter)
	{
		worldIteration = worldIter;
	}

	public void  DisableThink()
	{
		DoDisable ();
		thinkingEnabled = false;
	}
	public void EnableThink()
	{
		thinkingEnabled = true;

	}

	virtual protected void DoDisable()
	{


	}

	//Inspired by the old quake-C method
	abstract protected void Initalize ();
	abstract protected void Think ();
	virtual protected void ThinkAlpha(){} // bit of a hack
	abstract protected void ThinkFast ();


}
