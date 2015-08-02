using UnityEngine;
using System.Collections;

public class SpaceCloud : MonoBehaviourThink 

{
	private static int nodeDepthMax;
	protected int nodeDepth =-1;
	private static int nodeChildrenMax;
	private static int nodeChildrenMin;
	protected Vector3 direction = Vector3.zero;


	protected GameObject[] cloudChildren;
	//override protected void ThinkAlpha(){} // bit of a hack

	//Inspired by the old quake-C method
	override protected void Initalize ()
	{
		nodeChildrenMax = SRLConfiguration.GetSettingInt("SpaceCloud_node_children_max",2); 
		nodeChildrenMin = SRLConfiguration.GetSettingInt("SpaceCloud_node_children_min",0);
		nodeDepthMax = SRLConfiguration.GetSettingInt("SpaceCloud_node_depth_max",2);
		if(nodeDepth == -1)
			nodeDepth = nodeDepthMax;
		//direction = Vector3.zero;
		CreateChildren ();
	}


	override protected void Think ()
	{



	}


	override protected void ThinkFast ()
	{



	}


	private void CreateChildren()
	{

		int numChildren = Random.Range (nodeChildrenMin, nodeChildrenMax);
		numChildren -= nodeDepthMax - this.nodeDepth;

		GameObject cloudChild;

		if(this.nodeDepth >0)
		{

			for(int i = 0; i < numChildren; i++)
			{
				if(direction == Vector3.zero)
				{
					direction = new Vector3(Random.Range(-10f,10f),
					                        Random.Range(-10f,10f),
					                        Random.Range(-10f,10f));
				}
				else
				{
					direction = direction+ new Vector3(this.nodeDepth*Random.Range(-3f,3f),
					                        this.nodeDepth*Random.Range(-3f,3f),
					                        this.nodeDepth*Random.Range(-3f,3f));

				}
				cloudChild = GameObject.Instantiate(Resources.Load("Environment/CloudNode")) as GameObject;
				(cloudChild.GetComponent<SpaceCloud>()).nodeDepth = this.nodeDepth - 1;
				(cloudChild.GetComponent<SpaceCloud>()).direction = direction;
				cloudChild.transform.parent = this.transform;
				cloudChild.transform.position = this.transform.position + direction;


			}
		}
	}

}
