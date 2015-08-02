using UnityEngine;
using System.Collections;

public class RobotThink : MonoBehaviour 
{
	public bool selected = false; 
	private Color baseColor;
	float fracJourney;
	Vector3 targetPos;

	// Use this for initialization
	void Start () 
	{
		//targetPos = null;
		fracJourney = 0;
		baseColor = Color.blue;
		GetComponent<Renderer>().material.shader = Shader.Find ("Specular");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(baseColor == Color.blue)
		{
			//Destroy(transform.FindChild("Turret").gameObject);
			// the ABC's of reading

			baseColor = Color.gray;
			string type = GetComponent<PathDraw>().param_robotType;
			if (type == "strong-slow" || type == "weak-slow")
			{

				Destroy(transform.FindChild("WheelL").gameObject);
				Destroy(transform.FindChild("WheelR").gameObject);
			}
			if (type == "weak-fast" || type == "weak-slow")
			{
				Destroy(transform.FindChild("Gripper").gameObject);
			}


		}
		if(selected == true)
		{
			GetComponent<Renderer>().material.shader = Shader.Find ("Specular");
			GetComponent<Renderer>().material.SetColor ("_Color", Color.red);
		}
		else if(selected == false)
		{
			GetComponent<Renderer>().material.shader = Shader.Find ("Specular");
			GetComponent<Renderer>().material.SetColor ("_Color", baseColor);
		}

		//GameObject[] gos;
		//gos = GameObject.FindGameObjectsWithTag("inplanecursor");
		transform.LookAt(GameObject.FindGameObjectsWithTag("inplanecursor")[0].transform);

		Ray rayPoint = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;



		if (Input.GetKey(KeyCode.A))
			targetPos = new Vector3 (transform.position.x-1, transform.position.y,transform.position.z);
		if (Input.GetKey (KeyCode.D))
			targetPos = new Vector3 (transform.position.x+1, transform.position.y,transform.position.z);
		if (Input.GetKey (KeyCode.W))
			targetPos = new Vector3 (transform.position.x, transform.position.y,transform.position.z+1);
		if (Input.GetKey (KeyCode.S))
			targetPos = new Vector3 (transform.position.x, transform.position.y,transform.position.z-1);

		fracJourney = Vector3.Distance (transform.position, targetPos); 
		fracJourney = 0.05f/fracJourney;

		transform.position= Vector3.Lerp(transform.position, targetPos, fracJourney);
		targetPos = transform.position;
	}
}
