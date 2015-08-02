using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathDraw : MonoBehaviour
{
	public Transform DotPrefab;
	Vector3 lastDotPosition;
	bool lastPointExists;
	List<GameObject> path;
	bool drawingLine;
	bool updatePosition;
	int iteration;
	int worldIteration;
	Vector3 posBefore;
	Quaternion orientBefore;


	public string param_robotType = "weak-fast"; 
	float param_robotStep = 3f;
	float param_followRange = 0.5f;
	float returnDistance = 2f;

	//Item Variables
	GameObject selectedItem = null;
	GameObject worldEntity = null;
	bool itemCarried = false;


	void Start()
	{
		posBefore = new Vector3(0,0,0);
		orientBefore = new Quaternion(0,0,0,0);
		path = new List<GameObject>();
		drawingLine = false;
		lastPointExists = false;
		iteration = 0;
		worldIteration = 0;
		if(GameObject.FindGameObjectsWithTag ("MainCamera").Length > 0)
			worldEntity = GameObject.FindGameObjectsWithTag ("MainCamera")[0];

	}

	public void SetWorldIteration(int worldIter)
	{
		worldIteration = worldIter;
	}


	void OnCollisionEnter( Collision collision) 
	{
		if(selectedItem != null && selectedItem == collision.collider.gameObject)
		{
			ItemThink item = collision.collider.gameObject.GetComponent<ItemThink>();
			itemCarried = true;
			if ((this.param_robotType == "weak-fast") || (this.param_robotType == "weak-slow"))
			{
				if (item.param_weight == "heavy") 
				{
					// NO SECRET FOR YOU. FUCKER.
					itemCarried = false;
				}

			}

		}

		if(selectedItem != null && itemCarried == true && collision.collider.gameObject.tag == "goal")
		{
			float distance = Vector3.Distance(collision.collider.gameObject.transform.position,transform.position);
			if(distance < returnDistance)
			{
				//selectedItem.tag = "Untagged";
				Vector3 pos = collision.collider.gameObject.transform.position;
				//selectedItem.transform.position = new Vector3(pos.x,pos.y+2,pos.z);
				//selectedItem.renderer.material.shader = Shader.Find("Specular");
				//selectedItem.renderer.material.SetColor("_Color",Color.grey );
				GameObject item = selectedItem;
				DeSelectBox(item);
				itemCarried = false;
				Destroy(item);
				ClearPath();

				// SECRET COLLECT!
				UsePower(-200); 
				ReturnItem();

			}

		}
	}

	void UsePower(int p)
	{
		WorldThink worldMind = null;
		if(worldEntity != null && (worldMind = worldEntity.GetComponent<WorldThink>()) != null)
		{ //use one power for being active
		//	worldMind.totalPower = worldMind.totalPower -p;
		}

	}
	void ReturnItem()
	{
		WorldThink worldMind = null;
		if(worldEntity != null && (worldMind = worldEntity.GetComponent<WorldThink>()) != null)
		{ //use one power for being active
		//	worldMind.totalCollect = worldMind.totalCollect +1;
		}
		
	}

	void ActuateRobot()
	{
		UsePower(1); // use a power for being on.

		if(updatePosition == true &&  path.Count >0) 
		{
			//use a power for moving
			UsePower(1);

			Vector3 node = ((GameObject)path[ 0]).transform.position;
			node.y = transform.position.y; // We don't change height 
			if(Vector3.Distance(transform.position, node) > param_followRange )
			{
				transform.LookAt(node);
				GetComponent<Rigidbody>().velocity = transform.forward* param_robotStep;
			}
			else
			{
				while(Vector3.Distance(transform.position, node) < param_followRange)
				{
					GameObject obj = (GameObject)path[ 0];
					path.RemoveAt(0);
					Destroy(obj);
					if( path.Count == 0)
					{
						updatePosition=false;
					}
					if(path.Count == 0)
						break;
					node = ((GameObject)path[ 0]).transform.position;
				}
			}
		}

	}

	void SaveData()
	{

		Vector3 posAfter = transform.position;
		Quaternion orientAfter = transform.rotation;
		if (posBefore == new Vector3(0,0,0))
			posBefore = posAfter;
		if (orientBefore == new Quaternion(0,0,0,0))
			orientBefore = orientAfter;

			string itemType = "none";
		if(this.selectedItem != null)
		{
			itemType = selectedItem.GetComponent<ItemThink>().param_weight;
		}
		ActionData act = new ActionData(param_robotType,itemType,posBefore,posAfter,orientBefore,orientAfter,iteration);
		act.AddPosition("obstacle",FindClosestItem("obstacle").transform.position);
		if(FindClosestItem("robot") != null)
			act.AddPosition("robot",FindClosestItem("robot").transform.position);
		act.AddPosition("goal",FindClosestItem("goal").transform.position);
		if(FindClosestItem("item") != null)
			act.AddPosition("item",FindClosestItem("item").transform.position);
		if(selectedItem != null)
			act.AddPosition("selectedItem",selectedItem.gameObject.transform.position);
		act.Store();
		
	}
	
	void Update()
	{
		if(iteration < worldIteration)
		{

			iteration++;
			ActuateRobot();


			SaveData();
			//Debug.Log("diff-x:"+(posBefore.x - transform.position.x).ToString());
			posBefore = new Vector3(transform.position.x,transform.position.y,transform.position.z);
			orientBefore = new Quaternion(transform.rotation.eulerAngles.x,transform.rotation.y,transform.rotation.z,transform.rotation.w);

		}
		if(itemCarried == true && selectedItem != null)
		{
			Vector3 pos = transform.position;
			selectedItem.transform.position = new Vector3(pos.x,pos.y+1,pos.z);
		}

		if (Input.GetMouseButton(0) && 1==2)// && this.GetComponent<RobotThink>().selected == true)
		{
			this.GetComponent<RobotThink>().selected = true; 
			RaycastHit hit;
			Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit);
			if(hit.collider == this.GetComponent<Collider>())
			{
				drawingLine = true;
			}
		}

		if (drawingLine == true && Input.GetMouseButton(0))
		{
			Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			Vector3 newDotPosition = mouseRay.origin - mouseRay.direction / mouseRay.direction.y * mouseRay.origin.y;
			if (newDotPosition != lastDotPosition)
			{
				MakeADot(newDotPosition);
				updatePosition = true;
			}
		}
		else
			drawingLine = false;

		if (Input.GetKeyDown ("backspace") && this.gameObject.GetComponent<RobotThink>().selected == true)
		{
			ClearPath();
		}

	}

	void ClearPath()
	{
		drawingLine = false;
		lastPointExists = false;
		while(path.Count > 0)
		{
			GameObject obj = (GameObject)path[0];
			path.RemoveAt(0);
			Destroy(obj);
		}
		//If you have selected an item, do not have it carried, and have deleted your path,
		// give up on the selection
		if(selectedItem != null && itemCarried == false)
			DeSelectBox(selectedItem);

	}
	
	void MakeADot(Vector3 newDotPosition)
	{
		newDotPosition.y = newDotPosition.y + 0.5f;

		if (lastPointExists)
		{
			GameObject robot = Instantiate(Resources.Load<GameObject>("LineSection"),lastDotPosition,Quaternion.identity) as GameObject;
			Transform dot = robot.transform;
			GameObject colliderKeeper = new GameObject("LineSection");

			Vector3 lc = robot.transform.localScale;
			lc.z= Vector3.Distance(newDotPosition, lastDotPosition);
			robot.transform.LookAt(newDotPosition);
			robot.transform.localScale = lc;

			path.Add (robot);
			drawingLine = true;

			RaycastHit hit;

			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),  out hit, 500))
			{
				// and it has the right tag...
				if (hit.transform.tag == "item" && selectedItem == null)
				{
					SelectBox(hit.collider.gameObject);
				}
			}
		}
		lastDotPosition = newDotPosition;
		lastPointExists = true;

	}

	void SelectBox(GameObject item)
	{
		//item.renderer.material.shader = Shader.Find("Specular");
		item.GetComponent<ItemThink> ().carried = true;

		selectedItem = item;
	}

	void DeSelectBox(GameObject item)
	{
		//item.renderer.material.shader = Shader.Find("Diffuse");
		item.GetComponent<ItemThink> ().carried = false;
		selectedItem = null;
	}
	
	
	GameObject FindClosestItem(string theTag) 
	{
		GameObject[] gos;
		float distance;
		GameObject closest = null;

		gos = GameObject.FindGameObjectsWithTag(theTag);
		distance = Mathf.Infinity;
		foreach (GameObject go in gos) 
		{
			float curDistance = Vector3.Distance(go.transform.position, transform.position);
			if (curDistance < distance && go != this) 
			{
				closest = go;
				distance = curDistance;
			}
		}
		return closest;
	}
}
