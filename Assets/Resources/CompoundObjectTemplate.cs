using System;
using System.Collections.Generic;

/*
	We form a definition of Compound objects, which are created at runtime by a factory given a template. This
	is a general template;
 */
public class CompoundObjectTemplate
{
	public string name; //Unity Name to be used
	public int resetRotation;
	public string resourceName; //Unity Resource to load
	public string mountLocation; //the joint/object name on the parent to attach to.
	public string tag; //the unity tag to be used

	private List<CompoundObjectTemplate> children;
	public CompoundObjectTemplate (string nameIn, int resetRotationIn, string resourceNameIn, string mountLocationIn, string tagIn)
	{
		children = new List<CompoundObjectTemplate> ();
		name = nameIn; 
		resetRotation = resetRotationIn;
		resourceName = resourceNameIn; 
		mountLocation = mountLocationIn; 
		tag = tagIn; 

	}
	public CompoundObjectTemplate AddChild(CompoundObjectTemplate child)
	{
		children.Add (child);
		return child;
	}
	public List<CompoundObjectTemplate> GetChildren()
	{
		return children;
	}


}


