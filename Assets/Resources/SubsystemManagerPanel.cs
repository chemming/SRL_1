 using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SubsystemManagerPanel : ShipSubsystem {
	
	// Use this for initialization
	//TODO: ****Wrap the sub objects up as ShipControl objects so they can be (super) easily reused 
	//Putting off until beta. Until then - hackfest

	Transform[] slotTransforms;
	GameObject[] checkSymbols;
	GameObject[] minusSymbols;
	GameObject[] deleteSymbols;
	GameObject[] textLabels;



	GameObject upArrow;
	GameObject downArrow;




	bool clickDown;


	List<ShipSubsystem> subList;// = new List<ShipSubsystem>
	int subIndex = 0;

	GameObject highlightedComponent;// = new List<ShipSubsystem>

	//Selected Object Properties
	int s_index;
	bool s_reload;

	GameObject s_nameText;
	GameObject s_detailText;
	GameObject s_energyText;
	GameObject s_healthText;
	GameObject s_statusText;
	GameObject s_iconObject;

	List<GameObject> allComponents;// = new List<ShipSubsystem>

	GameObject componentNode;
	int numSlotsSubsusyem;
	protected override void Initalize () 
	{
		numSlotsSubsusyem = 6;
		slotTransforms = new Transform[numSlotsSubsusyem];
		componentNode = new GameObject ();
		componentNode.transform.parent = this.transform;
		subList = new List<ShipSubsystem> ();
		allComponents = new List<GameObject> ();
		InitalizeSlots ();
		InitalizeSelectedSubsystem ();
		clickDown = false;


		//	MeshRenderer meshRenderer = theText.AddComponent("MeshRenderer") as MeshRenderer;
		
	}
	
	// Update is called once per frame
	protected override void Think () 
	{
		UpdateListHighlights ();
		UpdateSelectedItem ();

	}


	private void UpdateSelectedItem()
	{

		ShipSubsystem sub = subList[s_index].GetComponent<ShipSubsystem>();
		s_nameText.GetComponent<TextMesh>().text = sub.SubDisplayName;
		s_detailText.GetComponent<TextMesh>().text = sub.SubDescription;
		s_energyText.GetComponent<TextMesh>().text = sub.SubCostEnergyPassive.ToString();
		s_healthText.GetComponent<TextMesh>().text = sub.SubHealth.ToString();
		s_statusText.GetComponent<TextMesh>().text = sub.SubStatus.ToString();

	}


	private void UpdateListHighlights()
	{
		if(subList.Count == 0)
		{
			foreach(string  subName in system.Keys)
			{
				if(system[subName].GetComponent<ShipSubsystem>() != null)
					subList.Add(system[subName].GetComponent<ShipSubsystem>());
			}
		}
		//Here we refresh the labels to match the selected components
		subIndex = subIndex % subList.Count; // keep within the boundry
		if (subIndex < 0)
			subIndex = subList.Count - 1;
		for (int i = 0; i < 6; i++)
		{
			int fromIndex = (subIndex + i)%subList.Count;
			textLabels[i].GetComponent<TextMesh>().text = fromIndex + ":"+subList[fromIndex].name;
			if(highlightedComponent!=checkSymbols[i] )
				checkSymbols[i].gameObject.GetComponent<Renderer>().material = buttonOffMaterial;
			if(highlightedComponent!=deleteSymbols[i] )
				deleteSymbols[i].gameObject.GetComponent<Renderer>().material = buttonOffMaterial;
			if(highlightedComponent!=minusSymbols[i] )
				minusSymbols[i].gameObject.GetComponent<Renderer>().material = buttonOffMaterial;
			if(highlightedComponent!=textLabels[i] )
				(textLabels[i].GetComponent<TextMesh>()).color = new Color(133f/255f,240f/255f,1,1);
			//s_index
			
			if(subList[fromIndex].GetComponent<ShipSubsystem>().SubStatus == Status.active
			   && highlightedComponent!=checkSymbols[i] )
				checkSymbols[i].gameObject.GetComponent<Renderer>().material = buttonPressMaterial;
			if(subList[fromIndex].GetComponent<ShipSubsystem>().SubStatus == Status.minimal
			   && highlightedComponent!=minusSymbols[i])
				minusSymbols[i].gameObject.GetComponent<Renderer>().material = buttonPressMaterial;
			if(subList[fromIndex].GetComponent<ShipSubsystem>().SubStatus == Status.deactivated
			   && highlightedComponent!=deleteSymbols[i])
				deleteSymbols[i].gameObject.GetComponent<Renderer>().material = buttonPressMaterial;
			if(s_index == fromIndex
			   && highlightedComponent!=textLabels[i])
				(textLabels[i].GetComponent<TextMesh>()).color = new Color(255f,255f,148f/255f,1);
			
		}

	}

	protected override void ThinkFast () 
	{
	}


	private void InitalizeSlots()
	{
		checkSymbols = new GameObject[numSlotsSubsusyem];
		minusSymbols = new GameObject[numSlotsSubsusyem];
		deleteSymbols = new GameObject[numSlotsSubsusyem];
		textLabels = new GameObject[numSlotsSubsusyem];


		for(int i = 1; i<numSlotsSubsusyem+1;i++)
		{
			GameObject theText = Instantiate (Resources.Load ("ConsoleText")) as GameObject;
			GameObject theChecks = Instantiate (Resources.Load ("Components/Ship/ControlChecks")) as GameObject;
			
			string slotLabel =  i+"1";
			Transform slotTransform = this.transform.FindChild ("InfoPanel").FindChild (slotLabel);
			slotTransforms[i-1] = slotTransform;
			//slotTransform.rotation = Quaternion.AngleAxis(180f,)
			slotTransform.localScale = Vector3.one;
			//Debug.Log(hullTransform);
			theText.GetComponent<TextMesh>().text = "Subsystem Info "+slotLabel;
			theText.transform.parent = componentNode.transform;
			theText.transform.position = slotTransforms[i-1].position 
				+ slotTransforms[i-1].TransformDirection(new Vector3(-0.035f,0.005f,0));
			theText.transform.rotation = slotTransforms[i-1].rotation;
			theText.transform.localScale = new Vector3(0.0005f,0.0005f,0.0005f);
			theText.name = "text_"+slotLabel;

			theChecks.transform.parent = componentNode.transform;
			theChecks.transform.position = slotTransforms[i-1].position
				+ slotTransforms[i-1].TransformDirection(new Vector3(0.1f,0,0));
			theChecks.transform.rotation =  slotTransforms[i-1].rotation;
			//theChecks.renderer.material = buttonOnMaterial;
			theChecks.name = "checks_"+slotLabel;
			Transform checks = theChecks.transform.FindChild("Checks");

			textLabels[i-1] = theText;

			minusSymbols[i-1] = checks.FindChild("Minus").gameObject;
			deleteSymbols[i-1] = checks.FindChild("Delete").gameObject;
			checkSymbols[i-1] = checks.FindChild("Check").gameObject;

			allComponents.Add(minusSymbols[i-1].gameObject);
			allComponents.Add(deleteSymbols[i-1].gameObject);
			allComponents.Add(checkSymbols[i-1].gameObject);
			allComponents.Add(theText.gameObject);
			foreach(Transform checkMark in checks)
				checkMark.gameObject.GetComponent<Renderer>().material = buttonOffMaterial;
		}
		//Before we finish, we create the navigation arrows
		upArrow = Instantiate (Resources.Load ("Components/Ship/ControlArrow")) as GameObject;
		upArrow.transform.parent = componentNode.transform;
		upArrow.transform.position = slotTransforms[0].position 
			+ slotTransforms[0].TransformDirection(new Vector3(0,0.01f,0));
		upArrow.transform.rotation =  slotTransforms[0].rotation;
		upArrow.transform.FindChild("Arrow").gameObject.GetComponent<Renderer>().material = buttonOffMaterial;

		//Before we finish, we create the navigation arrows
		downArrow = Instantiate (Resources.Load ("Components/Ship/ControlArrowDown")) as GameObject;
		downArrow.transform.parent = componentNode.transform;
		downArrow.transform.position = slotTransforms[numSlotsSubsusyem-1].position 
			+ slotTransforms[numSlotsSubsusyem-1].TransformDirection(new Vector3(0,-0.01f,0));
		downArrow.transform.rotation =  slotTransforms[numSlotsSubsusyem-1].rotation;
		downArrow.transform.FindChild("Arrow").gameObject.GetComponent<Renderer>().material = buttonOffMaterial;
	
		allComponents.Add(upArrow.transform.FindChild("Arrow").gameObject);
		allComponents.Add(downArrow.transform.FindChild("Arrow").gameObject);
	}


	private GameObject SetupTextComponent(string slotLabel,float offset,string name,string text )
	{
		GameObject nameText = Instantiate (Resources.Load ("ConsoleText")) as GameObject;

		Transform slotTransform = this.transform.FindChild ("InfoPanel").FindChild (slotLabel);
		slotTransform.localScale = Vector3.one;
		
		nameText.GetComponent<TextMesh>().text = text;
		nameText.transform.parent = componentNode.transform;
		nameText.transform.position = slotTransform.position 
			+ slotTransform.TransformDirection(new Vector3(offset,0.005f,0));
		nameText.transform.rotation = slotTransform.rotation;
		nameText.transform.localScale = new Vector3(0.0005f,0.0005f,0.0005f);
		nameText.name = name;
		return nameText;
	}

	private void InitalizeSelectedSubsystem()
	{
//		Debug.Log ("Yes I got called");
		s_reload = false;
		s_index = 0;

		s_nameText = SetupTextComponent ("12", 0.02f, "s_nameText", "Selected: <unselected>  ");


		s_detailText = SetupTextComponent ("22", 0.02f, "s_detailText", "This is a little \n detail text in lines! ");;

		s_energyText = SetupTextComponent ("42", 0.04f, "s_energyText", "+10 energy");
		s_healthText = SetupTextComponent ("52", 0.04f, "s_nameText",   "+10 health");
		s_statusText = SetupTextComponent ("62", 0.04f, "s_statusText", "-2 health");


		//s_iconObject;

	}


	protected override void PlayerInteract(Vector3 position,bool focused)
	{
		if(clickDown == false)
			SetSelectedControl (position, focused);

		if (!Input.GetMouseButton (0) && clickDown == true && highlightedComponent != null)
		{
			ProcessClick(highlightedComponent);
		}

		if (Input.GetMouseButton (0))
		{
			if(highlightedComponent != null &&highlightedComponent.GetComponent<TextMesh>() == null )
			{
				highlightedComponent.GetComponent<Renderer>().material = buttonPressMaterial;
			}
			if(highlightedComponent != null && highlightedComponent.GetComponent<TextMesh>() != null )
			{
				highlightedComponent.GetComponent<TextMesh>().color = new Color(1,1,1,1);
			}
			clickDown = true;
		}
		else
			clickDown = false;
	}


	private void ProcessClick(GameObject highlightedComponent)
	{

		if(highlightedComponent == upArrow.transform.FindChild("Arrow").gameObject)
			subIndex --;
		if(highlightedComponent == downArrow.transform.FindChild("Arrow").gameObject)
			subIndex ++;

		for(int i = 0; i<numSlotsSubsusyem;i++)
		{
			int fromIndex = (subIndex + i)%subList.Count;
			if(highlightedComponent == minusSymbols[i].gameObject)
				subList[fromIndex].gameObject.GetComponent<ShipSubsystem>().SubStatus = Status.minimal;
			if(highlightedComponent == deleteSymbols[i].gameObject)
				subList[fromIndex].gameObject.GetComponent<ShipSubsystem>().SubStatus = Status.deactivated;
			if(highlightedComponent == checkSymbols[i].gameObject)
				subList[fromIndex].gameObject.GetComponent<ShipSubsystem>().SubStatus = Status.active;
			if(highlightedComponent == textLabels[i].gameObject)
			{
				s_index = fromIndex;
			}
		}

	}

	private void SetSelectedControl(Vector3 position,bool focused)
	{
		if (focused == false)
			return;
		//Debug.Log("Checking");
		GameObject closestObj = null;
		float closestDist = 100f;
		float dist = closestDist;
		
		foreach(GameObject comp in allComponents)
		{
			Vector3 compPos = comp.transform.position;
			dist = (compPos-position).magnitude;
			if(dist< closestDist)
			{
				closestDist = dist;
				closestObj = comp;
			}
		}
		if (highlightedComponent != null && highlightedComponent.GetComponent<TextMesh>() == null)
			highlightedComponent.gameObject.GetComponent<Renderer>().material = buttonOffMaterial;
		if (highlightedComponent != null && highlightedComponent.GetComponent<TextMesh>() != null)
		{
			(highlightedComponent.GetComponent<TextMesh>()).color = new Color(133f/255f,240f/255f,1,1);
		}
		if(closestObj.GetComponent<TextMesh>() != null)
		{
			TextMesh txt = closestObj.GetComponent<TextMesh>();
			if(dist < 0.2f && txt.color.g != 148f/255f )
			{
				highlightedComponent = closestObj;
				txt.color = new Color(1,1,148f/255f,1);
				//txt.color =  Color.red;
			}
		}
		else
		{

			//Debug.Log (dist);
			if(dist < 0.2f && closestObj.gameObject.GetComponent<Renderer>().material != buttonPressMaterial)
			{
				highlightedComponent = closestObj;
				closestObj.gameObject.GetComponent<Renderer>().material = buttonOnMaterial;
				//Debug.Log(closestObj);
			}
		}
	}
	
}
