using System;
using UnityEngine;
public class CompoundObjectFactory
{
	public enum COType //Compound Object Type, classes of objects that can be created from a few templates
	{
		Player,Enemy,Asteroid,Avatar,Cloud
	}


	public CompoundObjectFactory ()
	{
	}

	public static GameObject Create(string modelName,COType type )
	{
		GameObject obj;
		
		obj = RecursiveCreate(GetTypeTemplate(type));
		obj.name = modelName;
		return obj;
	}

	/// <summary>
	/// Get a template for a coumpound object, given an enum. Used by CompoundObjectFactory.   	
	/// </summary>
	/// <param name="type">The class of object to create.</param>
	public static CompoundObjectTemplate GetTypeTemplate(COType type)
	{
		CompoundObjectTemplate ctObj;
		if(type == COType.Player ||  type == COType.Avatar   ||  type == COType.Enemy )
		{
			ctObj = new CompoundObjectTemplate ("Robot",1,"Components/Robot","","robot");
			CompoundObjectTemplate ctShipBody = new CompoundObjectTemplate("Body",0,"Components/Ship10Body","","robot");
			if(type != COType.Avatar)
			{

				if(type != COType.Enemy)
				{				
					ctShipBody.AddChild(new CompoundObjectTemplate("Sensor",1,"Components/Ship/Sensor","MountSensor","") );

					ctShipBody.AddChild(new CompoundObjectTemplate("SensorPanel",1,"Components/Ship/SensorPanel","MountConsoleLeft1B","") );
					ctShipBody.AddChild(new CompoundObjectTemplate("WaypointHud",1,"Components/Ship/WaypointHud","MountSensor","") );
				}

				ctShipBody.AddChild(new CompoundObjectTemplate("ControlPanel",1,"Components/Ship/ControlPanel","MountConsoleLeft2B","") );
				ctShipBody.AddChild(new CompoundObjectTemplate("ManagerSubsystem",1,"Components/Ship/SubsystemManagerPanel","MountConsoleRight2A","") );

				ctShipBody.AddChild(new CompoundObjectTemplate("CMG",1,"Components/Ship/CMG","MountBackTopB","") );
				ctShipBody.AddChild(new CompoundObjectTemplate("ThrusterSubsystemPanel",1,"Components/Ship/ThrusterSubsystemPanel","MountConsoleRight1A","") );
				ctShipBody.AddChild(new CompoundObjectTemplate("ShipAutopilot",1,"Components/Ship/ShipAutopilot","MountConsoleDashRight","") );
				ctShipBody.AddChild(new CompoundObjectTemplate("InfoSubsystem",1,"Components/Ship/InfoSubsystemPanel","MountConsoleDashLeft","") );

				if(type != COType.Enemy)
				{
					ctShipBody.AddChild(new CompoundObjectTemplate("ShipCamera",1,"Components/Ship/ShipCamera","MountPilot","") );
				}
				ctShipBody.AddChild(new CompoundObjectTemplate("ThrusterBackTop",0,"Components/Ship/Thruster","MountBackTopA",""));
				ctShipBody.AddChild(new CompoundObjectTemplate("ThrusterBackRight",0,"Components/Ship/Thruster","MountBackRightA",""));
				ctShipBody.AddChild(new CompoundObjectTemplate("ThrusterBackLeft",0,"Components/Ship/Thruster","MountBackLeftA",""));
				ctShipBody.AddChild(new CompoundObjectTemplate("ThrusterBackBottom",0,"Components/Ship/Thruster","MountBackBottomA",""));
				ctShipBody.AddChild(new CompoundObjectTemplate("ThrusterBackBackward",0,"Components/Ship/Engine","MountBackBackward",""));

				ctShipBody.AddChild(new CompoundObjectTemplate("Blaster0",0,"Components/Ship/Blaster","MountBackTopRightA",""));
				ctShipBody.AddChild(new CompoundObjectTemplate("Blaster1",0,"Components/Ship/LiquidBlaster","MountBackTopLeftA",""));
				ctShipBody.AddChild(new CompoundObjectTemplate("WeaponSubsystem",1,"Components/Ship/WeaponSubsystemPanel","MountConsoleRight0B","") );

				//ctShipBody.AddChild(new CompoundObjectTemplate("Engine",0,"Components/Ship/Engine","MountBackBackward",""));


				ctShipBody.AddChild(new CompoundObjectTemplate("ThrusterFrontTop",0,"Components/Ship/Thruster","MountFrontTopB",""));
				ctShipBody.AddChild(new CompoundObjectTemplate("ThrusterFrontRight",0,"Components/Ship/Thruster","MountFrontRightB",""));
				ctShipBody.AddChild(new CompoundObjectTemplate("ThrusterFrontLeft",0,"Components/Ship/Thruster","MountFrontLeftB",""));
				ctShipBody.AddChild(new CompoundObjectTemplate("ThrusterFrontBottom",0,"Components/Ship/Thruster","MountFrontBottomB",""));
				ctShipBody.AddChild(new CompoundObjectTemplate("ThrusterFrontForward",0,"Components/Ship/Thruster","MountFrontForward",""));

				ctShipBody.AddChild(new CompoundObjectTemplate("StatusSubsystem",1,"Components/Ship/StatusPanel","MountConsoleLeft0B","") );
				ctShipBody.AddChild(new CompoundObjectTemplate("ResourcesSubsystem",1,"Components/Ship/ResourcesPanel","MountConsoleRight0A","") );


				ctShipBody.AddChild(new CompoundObjectTemplate("Nexus",0,"Components/Ship/NexusExternal","MountBackBottomB",""));
				ctShipBody.AddChild(new CompoundObjectTemplate("SolarPanel0",0,"Components/Ship/SolarExternal","MountBackBottomRightB",""));
				ctShipBody.AddChild(new CompoundObjectTemplate("SolarPanel1",0,"Components/Ship/SolarExternal","MountBackBottomLeftB",""));
			//	ctShipBody.AddChild(new CompoundObjectTemplate("SolarPanel2",0,"Components/Ship/SolarExternal","MountBackTopB",""));
				ctShipBody.AddChild(new CompoundObjectTemplate("Battery",0,"Components/Ship/BatteryExternal","MountBackLeftB",""));
				ctShipBody.AddChild(new CompoundObjectTemplate("Metal",0,"Components/Ship/MetalExternal","MountBackRightB",""));
				ctShipBody.AddChild(new CompoundObjectTemplate("SensorDish",0,"Components/Ship/SensorExternal","MountMidBottomA",""));
				ctShipBody.AddChild(new CompoundObjectTemplate("OxygenTank",0,"Components/Ship/OxygenExternal","MountMidLeftA",""));

				if(type != COType.Enemy)
				{
					ctShipBody.AddChild(new CompoundObjectTemplate("PlayerController",0,"Components/Ship/PlayerController","MountPilot",""));
				}
				else
				{
					ctShipBody.AddChild(new CompoundObjectTemplate("AIController",0,"Components/Ship/AIController","MountPilot",""));
				}
			}
			ctObj.AddChild(ctShipBody);
		}
		else if ( type == COType.Cloud)
		{
			ctObj = new CompoundObjectTemplate ("SpaceCloud",0,"Environment/CloudNode","","");
		}
		else 
		{
			ctObj = new CompoundObjectTemplate ("Asteroid",0,"Environment/Asteroid","","obstacle");
		}
		return ctObj;
	}

	/// <summary>
	/// Adds a component to a parent object. Pretty low level service method.
	/// </summary>
	/// <param name="componentName">(string)The GameObject.name of this new</param>
	/// <param name="componentPath">(string)The path to the resource for this object</param>
	/// <param name="mountName">(string) GameObject.name of the mount point for this object.</param>
	/// <param name="container">(GameObject) who is the parent of this component.</param>
	/// <param name="resetRotation">(int)Should the sub objects rotation be zeroed?</param>
	private static GameObject AddComponentTo(string componentName,string componentPath,string mountName,GameObject container,int resetRotation=1)
	{

		GameObject resource = Resources.Load<GameObject>(componentPath);
		if (resource == null)
						Debug.Log ("Null situation you faggot: for ( " + componentName + " , " + componentPath);
		GameObject component = GameObject.Instantiate(resource) as GameObject;
		Transform mountPoint;
		if (container == null)
			mountPoint = null;
		else if(mountName.Length > 0)
			mountPoint  = container.transform.FindChild(mountName);
		else
			mountPoint  = container.transform;

		if(component != null)
		{
			if(mountPoint != null)
				component.transform.parent = mountPoint;
			
			//component.transform.localScale = Vector3.one;
			component.transform.localPosition = Vector3.zero;
			if(resetRotation > 0)
				component.transform.localRotation = Quaternion.identity;
			else if(mountPoint != null)
			{
				component.transform.rotation = component.transform.parent.rotation*component.transform.rotation;

//				Debug.Log(mountPoint.name);
//				Debug.Log(mountPoint.rotation.eulerAngles);
			}
			component.name = componentName;
		}
		else
			Debug.Log("Could not add Object: "+ componentName);
		return component;
	}
	


	private static GameObject RecursiveCreate(CompoundObjectTemplate t,GameObject parent=null)
	{
		GameObject obj;
		obj = AddComponentTo(t.name,t.resourceName,t.mountLocation,parent,t.resetRotation);

		foreach(CompoundObjectTemplate temChild in t.GetChildren())
		{
			RecursiveCreate(temChild,obj);
		}
		return obj;

	}
}