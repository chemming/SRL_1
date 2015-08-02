using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerController : ShipSubsystem , IGeneralShipController
{
	Hashtable commandStatus;
	Hashtable actionMapping;
	Hashtable actionMappingJoy;

	Vector3 shipCursorPos;
	Vector3 worldCursorPos;
	Vector3 worldDisplayCursorPos;

	Vector3 shipUp;
	GameObject DebugCursorConsole; 
	GameObject targetedSub;
	ShipSubsystem focusedPanel;
	Vector3 targetVelocity;
	Vector3 targetOrientation;
	Vector3 targetFwdVelocity;
	Vector3 avatarPoint;
	Vector3 cursorPos;

	Vector3 forwardVector;
	Vector3 sideVector;

	int rotateSign; 

	GameObject spacePath;

	public enum controlMode {sensor,manual,menu};
	
	controlMode mode;

	public bool IsLocalPlayer()
	{
		return true;
	}
	public bool IsRemotePlayer()
	{
		return false;
	}

	override protected void Initalize () 
	{
		rotateSign = 1;
		avatarPoint = Vector3.zero;
		targetFwdVelocity = Vector3.zero;
		g_displayName = "Human Player Interface";
		this.g_displayDescription = "Retrieves information from the player object.";
		this.g_activeEnergyCost = 0;
		this.g_activeMetalCost = 0;
		this.g_activeOxygenCost = 0;
		this.g_criticalSystem = true;
		this.g_status = Status.active;
		this.g_showOnPanel = false;
		this.g_maxEnergy = 0;
		this.g_maxMetal = 0;
		this.g_maxOxygen = 0;
		this.g_maxHealth = 1;
		this.g_health = 1;
		InitCommandStatus ();

		cursorPos = Vector3.zero;

		focusedPanel = null;
		//ap = this.transform.parent.parent.parent.GetComponent<ShipAutopilot> ();
		//ship = this.transform.parent.parent.parent;
		DebugCursorConsole = GameObject.Find("DebugCursorConsole");
		
		DebugCursorConsole.GetComponent<Renderer>().enabled = true;
		mode = controlMode.sensor;

		targetOrientation = Vector3.zero;
		targetVelocity = Vector3.zero;

	}
	private void InitCommandStatus()
	{
		commandStatus = new Hashtable ();
		commandStatus [ActionCode.up] = 0;
		commandStatus [ActionCode.left] = 0;
		commandStatus [ActionCode.down] = 0;
		commandStatus [ActionCode.right] = 0;
		commandStatus [ActionCode.aimup] = 0;
		commandStatus [ActionCode.aimleft] = 0;
		commandStatus [ActionCode.aimdown] = 0;
		commandStatus [ActionCode.aimright] = 0;

		commandStatus [ActionCode.forward] = 0;
		commandStatus [ActionCode.backward] = 0;
		commandStatus [ActionCode.fire1] = 0;
		commandStatus [ActionCode.fire2] = 0;
		commandStatus [ActionCode.mainMenu] = 0;
		commandStatus [ActionCode.shipMenu] = 0;
		commandStatus [ActionCode.viewToggle] = 0;
		commandStatus [ActionCode.weaponToggleSlot1] = 0;
		commandStatus [ActionCode.weaponToggleSlot2] = 0;


		actionMapping = new Hashtable ();
		actionMapping [KeyCode.W] = ActionCode.forward;
		actionMapping [KeyCode.S] = ActionCode.backward;
		actionMapping [KeyCode.A] = ActionCode.left;
		actionMapping [KeyCode.D] = ActionCode.right;
		actionMapping [KeyCode.Q] = ActionCode.up;
		actionMapping [KeyCode.E] = ActionCode.down;

		actionMapping [KeyCode.V] = ActionCode.viewToggle;
		actionMapping [KeyCode.X] = ActionCode.mainMenu;
		actionMapping [KeyCode.P] = ActionCode.shipMenu;
		actionMapping [KeyCode.Space] = ActionCode.fire1;
		actionMapping [KeyCode.B] = ActionCode.fire2;
		actionMapping [KeyCode.R] = ActionCode.weaponToggleSlot1;
		actionMapping [KeyCode.T] = ActionCode.weaponToggleSlot2;

		actionMapping [KeyCode.U] = ActionCode.aimup;
		actionMapping [KeyCode.J] = ActionCode.aimdown;
		actionMapping [KeyCode.K] = ActionCode.aimright;
		actionMapping [KeyCode.H] = ActionCode.aimleft;

		actionMappingJoy = new Hashtable ();
		actionMappingJoy ["Axis0-"] = ActionCode.left;
		actionMappingJoy ["Axis0+"] = ActionCode.right;
		actionMappingJoy ["Axis1-"] = ActionCode.forward;
		actionMappingJoy ["Axis1+"] = ActionCode.backward;

		actionMappingJoy ["Axis3+"] = ActionCode.aimleft;
		actionMappingJoy ["Axis3-"] = ActionCode.aimright;
		actionMappingJoy ["Axis4+"] = ActionCode.aimup;
		actionMappingJoy ["Axis4-"] = ActionCode.aimdown;
	
		actionMappingJoy ["Axis5-"] = ActionCode.left;
		actionMappingJoy ["Axis5+"] = ActionCode.right;
		actionMappingJoy ["Axis6+"] = ActionCode.forward;
		actionMappingJoy ["Axis6-"] = ActionCode.backward;

		/*actionMappingJoy ["Joy0"] = ActionCode.fire1;
		actionMappingJoy ["Joy1"] = ActionCode.fire1;
		actionMappingJoy ["Joy2"] = ActionCode.fire1;
		actionMappingJoy ["Joy3"] = ActionCode.fire1;
		actionMappingJoy ["Joy4"] = ActionCode.fire1;*/
		actionMappingJoy ["Joy5"] = ActionCode.fire1;
		/*actionMappingJoy ["Joy6"] = ActionCode.fire1;
		actionMappingJoy ["Joy7"] = ActionCode.fire1;
		actionMappingJoy ["Joy8"] = ActionCode.fire1;
		actionMappingJoy ["Joy9"] = ActionCode.fire1;
		actionMappingJoy ["Joy10"] = ActionCode.fire1;*/

	}

	public Vector3 GetTargetDisplacement()
	{
		return targetVelocity;
	}

	public Vector3 GetTargetOrientation()
	{
		return targetOrientation;
	}


	public Vector3 GetCursorPosition()
	{
		if (mode == controlMode.sensor)
		{
			Camera c =GameObject.Find("OverheadCamera").GetComponent<Camera>();
			if((int)commandStatus[ActionCode.aimup] == 0 && (int)commandStatus[ActionCode.aimdown] == 0)
			{	
				if((forwardVector.normalized +
				   Vector3.Project(ship.transform.forward.normalized,forwardVector.normalized)).magnitude > 1 )
				{
					rotateSign = 1;
				}
				else
				{
					rotateSign = -1;
				}
			}
			return c.WorldToScreenPoint(ship.transform.position
	            +30f*forwardVector.normalized*Mathf.Cos(cursorPos.y*Mathf.PI/180f)
	            +30f*   sideVector.normalized*Mathf.Sin(cursorPos.y*Mathf.PI/180f));


		}
		cursorPos.x = Screen.width/2;
		cursorPos.y = Screen.height/2;


		return cursorPos;

//		return Input.mousePosition;
	}

	public Vector3 GetWorldCursorPosition()
	{
		return worldCursorPos;
	}
	public Vector3 GetWorldDisplayCursorPosition()
	{
		return worldDisplayCursorPos;
	}

	public Vector3 GetUpVector()
	{
		return shipUp;
	}

	public bool GetAction(ActionCode code)
	{
		if (commandStatus == null) // called before initalization.
			return false;
		return (bool)(System.Convert.ToBoolean(commandStatus [code]));
	}

	override protected void Think () 
	{
		ProcessGameInput ();
	}

	override protected void ThinkFast () 
	{

		UpdateCursors ();

		if(CurrentMode() == controlMode.sensor)
		{
			DebugCursorConsole.transform.position = worldDisplayCursorPos;
			DebugCursorConsole.transform.position = worldCursorPos;
			DebugCursorConsole.transform.localScale = Vector3.one*2f;
		}
		else if(CurrentMode() == controlMode.manual)
		{
			DebugCursorConsole.transform.position = shipCursorPos;
			DebugCursorConsole.transform.localScale = Vector3.one*0.05f;
		}
		else if(CurrentMode() == controlMode.menu)
		{
			DebugCursorConsole.transform.position = shipCursorPos;
			DebugCursorConsole.transform.localScale = Vector3.one*0.005f;
		}
		CalculateOrientationTarget ();
		foreach(KeyCode key in actionMapping.Keys)
		{
			commandStatus[actionMapping[key]] = 0;
			if(Input.GetKey(key))
				commandStatus[actionMapping[key]] = 1;
		}
		foreach(string key in actionMappingJoy.Keys)
		{
			if(key.StartsWith("Axis") && key.EndsWith("+"))
			{
				if(Input.GetAxis (key.ToString().TrimEnd('+'))> 0)
				{
					commandStatus[actionMappingJoy[key]] = 1;
				}
			}
			else if(key.StartsWith("Axis") && key.EndsWith("-"))
			{
				if(Input.GetAxis (key.ToString().TrimEnd('-'))< 0)
				{
					commandStatus[actionMappingJoy[key]] = 1;
				}
			}
			else
			{
				if(Input.GetAxis (key.ToString())>0)
				{
					commandStatus[actionMappingJoy[key]] = 1;
					//Debug.Log(key+ " :>"+ actionMappingJoy[key]);
				}
			}
		}


		//Debug.Log (cursorPos);
	}

	
	private void UpdateCursors()
	{
		GameObject avatar = ship;
		Camera c =GameObject.Find("OverheadCamera").GetComponent<Camera>();
		
		if(mode == controlMode.sensor)
		{
			DebugCursorConsole.gameObject.GetComponent<Renderer>().enabled = true;
				//cursorPos.x = 10;
			if (((int)commandStatus [ActionCode.aimright]) > 0)
			{
				//cursorPos.x += 3f;
			}
			if (((int)commandStatus [ActionCode.aimleft]) > 0)
			{
				//cursorPos.x -= 3f;
			}
			if (((int)commandStatus [ActionCode.aimup]) > 0)
			{
				cursorPos.y += rotateSign*3f;
			}
			if (((int)commandStatus [ActionCode.aimdown]) > 0)
			{
				cursorPos.y -= rotateSign*3f;
			}

			if(cursorPos.y < 0)
				cursorPos.y += 360f;
			if(cursorPos.y > 360f)
				cursorPos.y -= 360f;
			//Debug.Log(cursorPos);



			c =GameObject.Find("OverheadCamera").GetComponent<Camera>();
			DebugCursorConsole.gameObject.transform.parent = c.transform;
			Ray rayMouse =	c.ScreenPointToRay (GetCursorPosition());
			Vector3 cursorPoint = c.ScreenToViewportPoint(GetCursorPosition());
			avatarPoint = c.WorldToViewportPoint(avatar.transform.position);
			Vector3 consoleCursor =  cursorPoint;
			worldDisplayCursorPos = c.ScreenToViewportPoint(GetCursorPosition());
			
			consoleCursor.z = avatarPoint.z;
			
			avatarPoint.z = 0;
			Vector3 avatarCursor = cursorPoint- avatarPoint;
			avatarCursor.x = -avatarCursor.x;
			
			Vector3 worldConsoleCursor = c.ViewportToWorldPoint(consoleCursor);
			Vector3 worldAvatar = avatar.transform.position;
			Vector3 worldAvatar2ConsoleCursor = worldConsoleCursor - worldAvatar;
			//Vector3 sensorAvatar2ConsoleCursor= sensor.transform.InverseTransformDirection(worldAvatar2ConsoleCursor);
			
			shipCursorPos = c.ViewportToWorldPoint(consoleCursor);
			//worldCursorPos = transform.parent.parent.position + sensorAvatar2ConsoleCursor*(1/ scale);
			worldCursorPos = worldConsoleCursor;
			shipUp = Vector3.up;
		}
		else
		{
			DebugCursorConsole.gameObject.transform.parent = ship.transform;
			DebugCursorConsole.gameObject.GetComponent<Renderer>().enabled = false;

			c =GameObject.Find("ShipCamera").GetComponent<Camera>();
			Vector3 cursorPointManual = c.ScreenToViewportPoint(GetCursorPosition());
			Vector3 consoleCursorManual =  cursorPointManual;
			consoleCursorManual.z = consoleCursorManual.z + 3; // Move the cursor forward
			shipCursorPos = c.ViewportToWorldPoint(consoleCursorManual);
			consoleCursorManual.z = consoleCursorManual.z + 12; // Move the worldpoint forward
			worldCursorPos = c.ViewportToWorldPoint(consoleCursorManual);
			if (((int)commandStatus [ActionCode.aimright]) > 0)
				worldCursorPos += ship.transform.right*(-3f); 
			if (((int)commandStatus [ActionCode.aimleft]) > 0)
				worldCursorPos += ship.transform.right*(3f); 
			if (((int)commandStatus [ActionCode.aimup]) > 0)
				worldCursorPos += ship.transform.up*(-3f); 
			if (((int)commandStatus [ActionCode.aimdown]) > 0)
				worldCursorPos += ship.transform.up*(3f); 

			shipUp = ship.transform.up;
			
			if(mode == controlMode.menu)
			{
				Ray ray = c.ScreenPointToRay (GetCursorPosition());
				
				RaycastHit hit;
				if (Physics.Raycast( ray,  out hit,30.0f) )
				{
					shipCursorPos = c.transform.position + ray.direction*(hit.distance + c.nearClipPlane);
					
					ShipSubsystem sub =  hit.collider.transform.parent.gameObject.GetComponent<ShipSubsystem>();
					
					if(targetedSub != null && targetedSub != hit.collider.transform.parent.gameObject)
					{
						targetedSub.GetComponent<ShipSubsystem>().CursorLeave();
						targetedSub = null;
					}
					
					if(sub != null)
					{
						targetedSub = sub.gameObject;
						sub.CursorHover(c.transform.position + ray.direction*(hit.distance+ c.nearClipPlane),focusedPanel != null);
						if(GetAction(ActionCode.fire1))
							FocusCameraOnSubsystem(c,  sub);
					}
				}
			}
		}
		
		
	}
	
	private void FocusCameraOnSubsystem(Camera c, ShipSubsystem sub)
	{
		Vector3 pos = sub.transform.InverseTransformPoint(sub.transform.position);
		pos.z = pos.z + 0.35f;
		pos =  sub.transform.TransformPoint(pos);
		
		c.transform.position = pos;
		c.transform.LookAt(sub.transform.position,sub.transform.up);
		
		focusedPanel = sub;
		
	}

	public controlMode CurrentMode()
	{
		return mode;
	}


	
	void ProcessGameInput()
	{

		targetVelocity = Vector3.zero;

		if (GetAction(ActionCode.shipMenu))
		{
			//Debug.Log("Escape");
			mode = controlMode.menu;
			focusedPanel = null;
			Camera c =GameObject.Find("ShipCamera").GetComponent<Camera>();
			c.transform.position = system["Body"].transform.FindChild("MountPilot").position;
			c.transform.rotation = system["Body"].transform.FindChild("MountPilot").rotation;
		}
		
		if (GetAction(ActionCode.viewToggle))
		{
			focusedPanel = null;
			Camera c =GameObject.Find("ShipCamera").GetComponent<Camera>();
			c.transform.position = system["Body"].transform.FindChild("MountPilot").position;
			c.transform.rotation = system["Body"].transform.FindChild("MountPilot").rotation;
			
			if(mode == controlMode.sensor)
			{
				mode = controlMode.manual;
			}
			else
			{
				mode = controlMode.sensor;
			}
		}

		if (mode == controlMode.menu)
			return;
		
		if(	mode == controlMode.sensor)
		{
			targetVelocity = CalculateTargetVelocity2d();
			//targetPos.y = 4; // Here is where we adjust to keep on the path's plane
			//targetPos = targetPos - ship.transform.position;
			if ((GetAction(ActionCode.left)||
			     GetAction(ActionCode.right)||
			     GetAction(ActionCode.forward)||
			     GetAction(ActionCode.backward)) 
			    || targetVelocity.magnitude > 1f)
			{
				targetVelocity = targetVelocity;
			}
			else
			{
				targetVelocity = Vector3.zero ;
			}
		}
		if(	mode == controlMode.manual)
		{

			Vector3 theForward = ship.transform.forward;
			Vector3 theUp = ship.transform.up;
			Vector3 theRight = Vector3.Cross(theUp,theForward);
			Vector3 theLeft = (-1)*theRight;
			
			if (GetAction(ActionCode.left))
				targetVelocity = targetVelocity + SRLConfiguration.P_mvdist*theLeft;
			if (GetAction(ActionCode.right))
				targetVelocity = targetVelocity + SRLConfiguration.P_mvdist*theRight;
			if (GetAction(ActionCode.forward))
				targetVelocity = targetVelocity + SRLConfiguration.P_mvdist* theForward;
			if (GetAction(ActionCode.backward))
				targetVelocity = targetVelocity + SRLConfiguration.P_mvdist*(-1)* theForward;
			if (GetAction(ActionCode.up))
				targetVelocity = targetVelocity + SRLConfiguration.P_mvdist* theUp;
			if (GetAction(ActionCode.down))
				targetVelocity = targetVelocity + SRLConfiguration.P_mvdist*(-1)* theUp;



			if(targetVelocity == Vector3.zero)
			{
				Vector3 currVeloOff = ship.GetComponent<Rigidbody>().velocity - 
					Vector3.Project(ship.GetComponent<Rigidbody>().velocity,theForward);
				Vector3 currVeloOn = Vector3.Project(ship.GetComponent<Rigidbody>().velocity,theForward);

				targetVelocity = -currVeloOff + 
					targetFwdVelocity.magnitude*theForward - currVeloOn;
			}
			else
			{
				targetFwdVelocity = Vector3.Project(ship.GetComponent<Rigidbody>().velocity,theForward);
			}

			targetVelocity = targetVelocity;
		}
	}

	
	private Vector3 CalculateTargetVelocity2d()
	{
		// Chooses velocity such that the player's ship is locked inside of a 2d plane, 
		// The pane is extracted from the SpacePath.
		// This function is fucking great. Simple and it just works.
		
		Vector3 targetVelocity = Vector3.zero;
		if (GetAction(ActionCode.forward))
			targetVelocity = targetVelocity + new Vector3 (0, 0, SRLConfiguration.P_mvdist);
		if (GetAction(ActionCode.left))
			targetVelocity = targetVelocity + new Vector3 (-SRLConfiguration.P_mvdist, 0, 0);
		if (GetAction(ActionCode.right))
			targetVelocity = targetVelocity + new Vector3 (SRLConfiguration.P_mvdist, 0, 0);
		if (GetAction(ActionCode.backward))
			targetVelocity = targetVelocity + new Vector3 (0, 0, -SRLConfiguration.P_mvdist);
		
		//Now we consider the closest point on the space path
		
		if (spacePath == null)
		{
			spacePath = GameObject.Find ("SpacePath");
			if (spacePath == null)
				return targetVelocity;
		}
		//Vector3 theUp = posBeacon.transform.up;
		Vector3 upVector;
		Vector3 beaconPos = (spacePath.GetComponent<SpacePath> ()).GetNearestPoint (ship.transform.position,out upVector, out forwardVector);
		sideVector = Vector3.Cross(upVector,forwardVector);
		
		Vector3 shipPos = ship.transform.position;
		
		float timeFactor = 1f;
		float maxSideDistance = 37f;
		
		
		Vector3 vec_shipBeacon = shipPos - beaconPos;
		
		Vector3 upDisplacement_shipBeacon = Vector3.Project (vec_shipBeacon, upVector.normalized);
		Vector3 upVeclocity_shipBeacon = Vector3.Project (ship.GetComponent<Rigidbody>().velocity, upVector.normalized);
		Vector3 upAddedDisplacement_shipBeacon = upVeclocity_shipBeacon * timeFactor;
		Vector3 upTarget_shipBeacon = upDisplacement_shipBeacon +  upAddedDisplacement_shipBeacon;
		
		Vector3 sideDisplacement_shipBeacon = Vector3.Project (vec_shipBeacon, sideVector.normalized);
		Vector3 sideVeclocity_shipBeacon = Vector3.Project (ship.GetComponent<Rigidbody>().velocity, sideVector.normalized);
		Vector3 sideAddedDisplacement_shipBeacon = sideVeclocity_shipBeacon * timeFactor;
		Vector3 sideTarget_shipBeacon = sideDisplacement_shipBeacon +  sideAddedDisplacement_shipBeacon;
		
		
		
		//First, we correct for the ship displacement:
		//1 - figure out what component of the force contributes to a "bad" direction
		//Debug.DrawLine (shipPos, shipPos + targetVelocity * 1, Color.red, 0.1f);
		Vector3 vec_upBadDirection = Vector3.Project (targetVelocity, upDisplacement_shipBeacon.normalized);
		targetVelocity = targetVelocity - vec_upBadDirection; // Take away that component of the force
		targetVelocity = targetVelocity - upTarget_shipBeacon; // seek normal
		
		if(sideTarget_shipBeacon.magnitude > maxSideDistance*1.1f)
		{
		//	targetVelocity = Vector3.zero;
			Vector3 targetSideDisplacement = Vector3.zero;
			if( (vec_shipBeacon - sideVector.normalized).magnitude < (vec_shipBeacon + sideVector.normalized).magnitude )
				targetSideDisplacement = maxSideDistance* sideVector.normalized;
			else
				targetSideDisplacement = maxSideDistance* (-1f)*sideVector.normalized;
			
			vec_shipBeacon -= targetSideDisplacement;
			
			sideDisplacement_shipBeacon = Vector3.Project (vec_shipBeacon, sideVector.normalized);
			sideVeclocity_shipBeacon = Vector3.Project (ship.GetComponent<Rigidbody>().velocity, sideVector.normalized);
			
			sideAddedDisplacement_shipBeacon = sideVeclocity_shipBeacon * timeFactor;
			sideTarget_shipBeacon = sideDisplacement_shipBeacon +  sideAddedDisplacement_shipBeacon;
			Vector3 vec_sideBadDirection = Vector3.Project (targetVelocity, sideDisplacement_shipBeacon.normalized);
			targetVelocity = targetVelocity - vec_sideBadDirection; // Take away that component of the force
			targetVelocity = targetVelocity - sideTarget_shipBeacon; // seek normal

		//	Debug.DrawLine (shipPos, shipPos + targetVelocity * 1, Color.red, 0.1f);

		}
		
		//Debug.DrawLine (shipPos, shipPos - upDisplacement_shipBeacon * 1, Color.blue, 0.1f);
		//Debug.DrawLine (shipPos, shipPos + targetVelocity * 1, Color.yellow, 0.1f);
		
		return targetVelocity;
	}


	private void CalculateOrientationTarget()
	{
		if(CurrentMode() == PlayerController.controlMode.sensor)
			targetOrientation = GetWorldCursorPosition() - ship.transform.position;
		
		
		if(CurrentMode() == PlayerController.controlMode.manual)
		{
			Vector3 normalizedWorldPos = GetWorldCursorPosition() - this.transform.parent.parent.parent.position;
			normalizedWorldPos.Normalize();
			Vector3 posDiff = normalizedWorldPos - this.transform.parent.parent.parent.forward;
			float easing = ((1- Mathf.Exp(posDiff.magnitude))/ Mathf.Exp(posDiff.magnitude)); 
			easing = Mathf.Abs( easing);
			Vector3 targetAim = normalizedWorldPos + this.transform.parent.parent.parent.position;
			Vector3 currentAim = this.transform.parent.parent.parent.forward + this.transform.parent.parent.parent.position;
			
			
			if(easing > 0.05f)
				targetOrientation = GetWorldCursorPosition() - ship.transform.position;//Vector3.Lerp(currentAim,targetAim,easing));
			//else
			//	ap.SetOrientationTarget (Vector3.zero);//Vector3.Lerp(currentAim,targetAim,easing));
		}
		if(CurrentMode() == PlayerController.controlMode.manual 
		   || CurrentMode() == PlayerController.controlMode.sensor)
			shipUp = pController.GetUpVector();//Vector3.Lerp(currentAim,targetAim,easing));
	}


}
