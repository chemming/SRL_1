using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
This badass script controls the thrusters for the whole system! Cool right?!?!?

 */
public class ThrusterSubsystemPanel : ShipSubsystem 
{

	Dictionary<GameObject,ThrusterController> buttonThruster; // A mapping of buttons -> thruster
	Dictionary<ThrusterController,GameObject> thrusterButton; // A mapping of buttons -> thruster
	Dictionary<GameObject,GameObject> buttonLight; // A mapping of buttons -> lights
	Dictionary<ThrusterController,Vector3> thrusterForce; // A mapping of thrusters to max power vectors;
	Dictionary<ThrusterController,Vector3> thrusterPosition; // A mapping of thrusters to max power vectors;
	Dictionary<ThrusterController,Vector3> thrusterMoment; // A mapping of thrusters to max power vectors;
	Dictionary<ThrusterController,Vector3> thrusterAccLinear; 
	Dictionary<ThrusterController,Vector3> thrusterAccAngular; 
	Dictionary<ThrusterController,float> thrusterPower; 
	List<ThrusterController> thrusterList;

	public enum ThrusterValueType {Force,Position,Moment,AccLinear,AccAngular,Power,ForceFull,MomentFull};

	Transform body;
	//Transform ship;
	//TODO: Simplify using system registration system


	private string[][] GetConsoleMapping()
	{


		int buttonCount = 18;
		string[][] thrusterConsoleMapping = new string[buttonCount][];
		thrusterConsoleMapping[0] = new string[4]{ "MountFrontForward","ThrusterFrontForward","ThrusterPanel","Forward9"};
		thrusterConsoleMapping[0] = new string[4]{ "MountFrontForward","ThrusterFrontForward","ThrusterPanel","Forward8"};
		thrusterConsoleMapping[0] = new string[4]{ "MountFrontForward","ThrusterFrontForward","ThrusterPanel","Forward7"};
		thrusterConsoleMapping[0] = new string[4]{ "MountFrontForward","ThrusterFrontForward","ThrusterPanel","Forward6"};
		thrusterConsoleMapping[0] = new string[4]{ "MountFrontForward","ThrusterFrontForward","ThrusterPanel","Forward5"};
		thrusterConsoleMapping[0] = new string[4]{ "MountFrontForward","ThrusterFrontForward","ThrusterPanel","Forward4"};
		thrusterConsoleMapping[0] = new string[4]{ "MountFrontForward","ThrusterFrontForward","ThrusterPanel","Forward3"};
		thrusterConsoleMapping[0] = new string[4]{ "MountFrontForward","ThrusterFrontForward","ThrusterPanel","Forward2"};
		thrusterConsoleMapping[0] = new string[4]{ "MountFrontForward","ThrusterFrontForward","ThrusterPanel","Forward1"};

		thrusterConsoleMapping[1] = new string[4]{ "MountFrontTopB","ThrusterFrontTop","ThrusterPanel","Front4"};
		thrusterConsoleMapping[2] = new string[4]{ "MountFrontRightB","ThrusterFrontRight","ThrusterPanel","Front3"};
		thrusterConsoleMapping[3] = new string[4]{ "MountFrontLeftB","ThrusterFrontLeft","ThrusterPanel","Front2"};
		thrusterConsoleMapping[4] = new string[4]{ "MountFrontBottomB","ThrusterFrontBottom","ThrusterPanel","Front1"};

		thrusterConsoleMapping[5] = new string[4]{ "MountBackTopA","ThrusterBackTop","ThrusterPanel","Back4"};
		thrusterConsoleMapping[6] = new string[4]{ "MountBackRightA","ThrusterBackRight","ThrusterPanel","Back3"};
		thrusterConsoleMapping[7] = new string[4]{ "MountBackLeftA","ThrusterBackLeft","ThrusterPanel","Back2"};
		thrusterConsoleMapping[8] = new string[4]{ "MountBackBottomA","ThrusterBackBottom","ThrusterPanel","Back1"};

		thrusterConsoleMapping[9] = new string[4]{ "MountBackBackward","ThrusterBackBackward","ThrusterPanel","Backward9"};
		thrusterConsoleMapping[10] = new string[4]{ "MountBackBackward","ThrusterBackBackward","ThrusterPanel","Backward8"};
		thrusterConsoleMapping[11] = new string[4]{ "MountBackBackward","ThrusterBackBackward","ThrusterPanel","Backward7"};
		thrusterConsoleMapping[12] = new string[4]{ "MountBackBackward","ThrusterBackBackward","ThrusterPanel","Backward6"};
		thrusterConsoleMapping[13] = new string[4]{ "MountBackBackward","ThrusterBackBackward","ThrusterPanel","Backward5"};
		thrusterConsoleMapping[14] = new string[4]{ "MountBackBackward","ThrusterBackBackward","ThrusterPanel","Backward4"};
		thrusterConsoleMapping[15] = new string[4]{ "MountBackTopB","Rotorx","ThrusterPanel","Backward3"};
		thrusterConsoleMapping[16] = new string[4]{ "MountBackTopB","Rotory","ThrusterPanel","Backward2"};
		thrusterConsoleMapping[17] = new string[4]{ "MountBackTopB","Rotorz","ThrusterPanel","Backward1"};

		return thrusterConsoleMapping;
	}

	private void SetupThruster(ThrusterController con)
	{
		if (thrusterList.Contains (con))
			return;

		if((con as CMGController) != null)
		{
			thrusterForce[con] = Vector3.zero; // Ignore force and position.
			thrusterPosition[con] = Vector3.zero;
			thrusterMoment[con] = (con as CMGController).GetMaxPower()*ship.transform.InverseTransformDirection(con.transform.up);
		}
		else
		{
			thrusterForce[con] = (ship.transform.InverseTransformDirection(con.transform.FindChild("Jet").forward*(-1)) )*con.GetMaxPower();
			thrusterPosition[con] = ship.transform.InverseTransformPoint(con.transform.FindChild("Jet").position); // Relative position of the force vector
			thrusterMoment[con] = Vector3.Cross(thrusterForce[con],thrusterPosition[con]);
		}
		thrusterAccLinear[con] = thrusterForce[con] / ship.GetComponent<Rigidbody>().mass;
		thrusterAccAngular[con] = thrusterMoment[con] / ship.GetComponent<Rigidbody>().mass;
		thrusterPower[con] = 0f; // power between zero and one
		thrusterList.Add (con);
	}

	protected override void Initalize ()
	{
		SubDisplayName= "Thruster Management Console";
		SubDescription= "Manages the power flow to \n each thruster.";
		SubCostEnergyPassive=5f;
		SubShowOnPanel = true;
		SubStatus=Status.active;


		thrusterForce = new Dictionary<ThrusterController, Vector3> ();
		thrusterPosition = new Dictionary<ThrusterController, Vector3> ();
		thrusterAccLinear = new Dictionary<ThrusterController, Vector3> ();
		thrusterAccAngular = new Dictionary<ThrusterController, Vector3> ();
		thrusterMoment = new Dictionary<ThrusterController, Vector3> ();
		thrusterPower = new Dictionary<ThrusterController, float> ();
		thrusterList = new List<ThrusterController>();

		string[][] thrusterConsoleMapping; 

		thrusterConsoleMapping = GetConsoleMapping();
		int index = 0; 


		body = system["Body"].transform;
		//ship = body.parent;
		buttonThruster = new Dictionary<GameObject,ThrusterController >();
		thrusterButton = new Dictionary<ThrusterController,GameObject > ();


		index = 0; 
		// In this loop, we map all the console buttons to their thrusters
		while (index < thrusterConsoleMapping.Length)
		{
//			Debug.Log(ship.transform.FindChild (thrusterConsoleMapping[index][0]) + "---" + thrusterConsoleMapping[index][0]);

			ThrusterController con =  null;
			Transform obj = body.FindChild (thrusterConsoleMapping[index][0]).FindChild (thrusterConsoleMapping[index][1]);
			if(obj!= null)
				con = obj.gameObject.GetComponent<ThrusterController> ();
			if( con == null)
			{// Try going deeper . . . in a real hackey way.
			//TODO recurse properly, or iterate properly. For now, if it ain't broke . . .
				foreach (Transform par in body.FindChild (thrusterConsoleMapping[index][0]).transform)
				{
			//		Debug.Log("huh1:"+thrusterConsoleMapping[index][1]);
			//		Debug.Log("huh2:"+par);
					con = par.FindChild(thrusterConsoleMapping[index][1]).gameObject.GetComponent<ThrusterController> ();
					if (con != null) break;
					foreach (Transform parT in par.transform)
					{
						con = parT.FindChild(thrusterConsoleMapping[index][1]).gameObject.GetComponent<ThrusterController> ();
						if (con != null) break;
							foreach (Transform parTT in parT.transform)
							{
								con = parTT.FindChild(thrusterConsoleMapping[index][1]).gameObject.GetComponent<ThrusterController> ();
								if (con != null) break;
							}
						if (con != null)break;					
					}
					if (con != null) break;
				}
			}
			if(con == null)
			{
				Debug.Log("Cant find ship component" + thrusterConsoleMapping[index][1] + " Aborting");
				break;

			}
			else
			{
				GameObject button = transform.FindChild (thrusterConsoleMapping[index][2]).FindChild (thrusterConsoleMapping[index][3]).gameObject;
				buttonThruster [button ] = con;
				thrusterButton [con ] = button;
				button.GetComponent<Renderer>().material = buttonOffMaterial;

				SetupThruster(con);

			}
			index++;
		}
	}

	//GetThrusterAcceleration : The Acceleration currently affecting the ship  
	public Vector3[] GetThrusterValues(ThrusterValueType mode, bool thruster=true)
	{
		if(thrusterList == null)
			return new Vector3[0];
		//The first half is 
		int tcount = thrusterList.Count;
		List<Vector3> result = new List<Vector3>();
		for(int i=0; i<thrusterList.Count; i++)
		{
			ThrusterController con = thrusterList[i];
			if((con as CMGController == null && thruster == true) ||
			   (con as CMGController != null && thruster == false))
			{
//				Debug.Log(con);
				if(mode == ThrusterValueType.AccAngular)
					result.Add( thrusterPower[con]*thrusterAccAngular[con]);
				if(mode == ThrusterValueType.AccLinear)
					result.Add( thrusterPower[con]*thrusterAccLinear[con]);
				if(mode == ThrusterValueType.Force)
					result.Add(  thrusterPower[con]*thrusterForce[con]);
				if(mode == ThrusterValueType.Moment)
					result.Add(  thrusterPower[con]*thrusterMoment[con]);
				if(mode == ThrusterValueType.Position)
					result.Add(  thrusterPosition[con]);
				if(mode == ThrusterValueType.ForceFull)
					result.Add( thrusterForce[con]);
				if(mode == ThrusterValueType.MomentFull)
					result.Add(  thrusterMoment[con]);
			}
			//			if(mode == ThrusterValueType.Power) This is a float, brainstorming
//				result[i] = thrusterPower[con];
		//	Debug.Log(thrusterForce[con]);
		}
		return result.ToArray();
	}

	public void SetThrusterPower(int thrusterIndex,float powerFraction)
	{
//		if(powerFraction != 0 && thrusterIndex < 10)
//			Debug.Log (thrusterIndex + "::::" + powerFraction);
//		Debug.Log (thrusterList.Count);
		if(powerFraction > 0.3f)
			thrusterButton [thrusterList[thrusterIndex]] .GetComponent<Renderer>().material = buttonOnMaterial;
		else
			thrusterButton [thrusterList[thrusterIndex] ] .GetComponent<Renderer>().material = buttonOffMaterial;

		thrusterList[thrusterIndex].SetPowerFraction (powerFraction);
		thrusterPower [thrusterList[thrusterIndex]] = powerFraction;
	}

	protected override void Think ()
	{
	
	}

	protected override void ThinkFast ()
	{
	
	}


}
