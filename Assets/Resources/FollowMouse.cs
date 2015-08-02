using UnityEngine;
using System.Collections;

public class FollowMouse : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
/*		Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),);
		// raycast
		RaycastHit rayHit;
		if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), rayHit))
		{
			// where did the raycast hit in the world - position of rayhit
			Vector3 rayHitWorldPosition = rayHit.point;
			//print ("rayHit.point : " + rayHit.point + " (rayHitWorldPosition)");

			//transform.position = Vector2.Lerp(transform.position, mousePosition, moveSpeed);
			
		}
*/
		if(Camera.main != null)
		{
			Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
			Vector3 p = r.GetPoint (10);
			transform.position = p;
			//Debug.Log (transform.position.x);
			transform.position = new Vector3(transform.position.x,SRLConfiguration.W_height/2,transform.position.z);
		}
		//transform.position = new Vector3(10,0,10);
		//Debug.Log (Input.mousePosition.x);
		//transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}
}
