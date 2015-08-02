using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

/*
     
    ShipAutopilot:

	This autopilot constantly works, using 
	a ships actuators, to obtain a desired
	displacement and orientation.

	It uses the sensor system for situational awareness, and the the ThrusterSubsystem to actuate.
	

 */
public class ShipAutopilot : ShipSubsystem 
{

	// Use this for initialization
	Vector3 movementVector;
	Vector3 orientationVector;
	Vector3 upVector;

	int jetIndex1;
	int jetIndex2;
	int jetIndex3;
	Vector3[] jetPositions;// = thrusterSubsystem.GetThrusterValues (ThrusterSubsystemPanel.ThrusterValueType.Position);



	float debugFactor = 1f;

	int count;
	bool enabled;
	bool manualMode = false;
	float maxGimbalPower = 20;
	float futureFac = 1f;
	ThrusterSubsystemPanel thrusterSubsystem;

	Vector3 debugPrediction;


	//Engine Constants
	float cutoffPredictionFactor = 3f; //How close we are allowed to get to the desiered trajectory before quitting.
	bool forceEnginesFull = true; // A less gooder alternative to scaled thrusters.
	float engineFactor = 0.4f; // How much to weigh the projected engine output against the future projected position 
	float futureFactor = 1f;
	float pulseTime = 1f;
	float powerScale = 50f;
	float maxJetAngle = 35f;
	float targetTolerance = 0f;

	override protected void Initalize () 
	{

		SubDisplayName= "Autopilot";
		SubDescription= "Controls the thrusters\n corrisponding to human commands.";
		SubCostEnergyPassive=10f;
		SubShowOnPanel = true;
		SubStatus=Status.active;

		/* a little slow, and off by an angle 
		 */
		forceEnginesFull = false;
		engineFactor = 0.4f;
		powerScale = engineFactor*25f;
		cutoffPredictionFactor = 15f;
		futureFactor = engineFactor*1.5f;
		targetTolerance = 0.5f;
		maxJetAngle = 25f;
		/*
		forceEnginesFull = false;
		engineFactor = 1f;
		powerScale = engineFactor*5f;
		cutoffPredictionFactor = 5000f;
		futureFactor = engineFactor*1.5f;
		targetTolerance = 0.5f;
		*/


		enabled = true;
		//		GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
		//		GameObject rob = GameObject.FindGameObjectWithTag("robot");
		//		cam.transform.position = new Vector3 (rob.transform.position.x, cam.transform.position.y, rob.transform.position.z);
		count = 0; 
		movementVector = new Vector3 (0, 0, 0);
		orientationVector = new Vector3 (0, 0, 1);
		upVector = new Vector3 (0, 1, 0);

		thrusterSubsystem = ship.transform.FindChild("Body").FindChild("MountConsoleRight1A").GetComponentInChildren<ThrusterSubsystemPanel>();
		manualMode = false;

		InitPanel(buttonOnMaterial,"Battery");
	}
	
	private void InitPanel(Material mat,string prefix)
	{
		Transform panel = this.transform.FindChild("InfoPanel");
		foreach(Transform button in panel)
		{
			//Debug.Log(button.name + ">>" + prefix);
			if(button.name.StartsWith(prefix))
				button.GetComponent<Renderer>().material = mat;
			
		}
		
	}

	public void SetControlMode(bool mode)
	{
		manualMode = mode;
	}

	public void SetMovementTarget(Vector3 target)
	{
		movementVector = target;
	}
	public void SetOrientationTarget(Vector3 target)
	{
		orientationVector = target;
	}
	public void SetOrientationUp(Vector3 vUp)
	{
		upVector = vUp;
	}

	// Update is called once per frame
	override protected void Think () 
	{
		if (enabled == false)
			return;

	}
	// Update is called once per frame
	override protected void ThinkFast ()
	{
		if (enabled == false)
						return;
		//Jet Test
		int modeControl = 0;

		
		PulseThrusters (); //Use the thrusters to control displacement.
		PulseGimbals (); //After, use Gimbals to attempt attitude correct		

		/*intrimMovementVector = movementVector;
		intrimOrientationVector = orientationVector;

		this.rigidbody.velocity = new Vector3 (intrimMovementVector.x, intrimMovementVector.y, intrimMovementVector.z);
		
		//this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(intrimOrientationVector), 0.1f);
		//this.transform.rotation = Quaternion.Slerp(this.transform.rotation, 
		//Quaternion.LookRotation(intrimOrientationVector), 0.1f));


		Vector3 av = this.rigidbody.angularVelocity;
		this.rigidbody.angularVelocity = new Vector3 (0 , 0, 0);
		this.transform.LookAt (orientationVector);*/
	}



	void PulseGimbals()
	{
		if (orientationVector == Vector3.zero)
			return; // Just turn of engines when we have no target

		double slope = 45; //Smooth power slope in
		double distance = 1; //Distance from angle to ease off rotors
		float lockMag = 15f; //Distance from target angle to cut engines entirely, and coast.
		Vector3 targetUp = upVector; // Get as close to using the Y as up as possible.
		float maxGimbalInput = 7f;

	
		float futureFactor = 260f;
		//Debug.Log (futureFactor);

		Vector3 futureAngVFactor;
		if(manualMode == false)
			futureAngVFactor = new Vector3(150f,150f,150f);
		else
			futureAngVFactor = new Vector3(150f,150f,50f); // when in cockpit view, the z tracking ossilates too much for cockpit control . . 

		SetJetPower(new int[]{10,11,12} , new Matrix(new float[,]{{0f},{0f},{0f}}),false);	
		Vector3[] momentCurrent = thrusterSubsystem.GetThrusterValues 
			(ThrusterSubsystemPanel.ThrusterValueType.Moment);

		Vector3 zAxis = ship.transform.InverseTransformDirection(orientationVector);
		zAxis = new Vector3 (zAxis.x, zAxis.y, zAxis.z);
		zAxis.Normalize (); // The direction we want to look


		Vector3 yAxis = ship.transform.InverseTransformDirection(targetUp);
		yAxis.Normalize ();
		//Debug.Log (yAxis);

		//Since we are making 
		yAxis =  yAxis - Vector3.Project (yAxis,zAxis);//- Vector3.Project (zAxis,yAxis);
		yAxis.Normalize ();
		Vector3 xAxis = Vector3.Cross (yAxis, zAxis);


		Matrix trans = new Matrix (new float[3, 3]
		   {{xAxis.x,yAxis.x,zAxis.x},
			{xAxis.y,yAxis.y,zAxis.y},
			{xAxis.z,yAxis.z,zAxis.z}});


		Vector3 approach = new Vector3(0,0,10);
		Vector3 upCurrent = new Vector3(0,10,0);
		Vector3 approachNew = (Vector3)(trans*((Matrix)approach));
		Vector3 upNew = (Vector3)(trans*((Matrix)upCurrent));

		float th1; float c2;float th2; float th3;

		th1 = Mathf.Atan2(trans[1,2],trans[2,2]);
		if(th1 > 2f)
			 th1 = Mathf.Atan2(-trans[2,1],trans[1,1]);

		c2 = Mathf.Sqrt(Mathf.Pow(trans[0,0],2)+Mathf.Pow(trans[0,1],2));
		th2 = Mathf.Atan2(-trans[0,2],c2);
		th3 = Mathf.Atan2(trans[0,1],trans[0,0]);



		//X-Y-Z "instant" rotations
		Vector3 rotationPositions = new Vector3 (th1, th2, th3);
		rotationPositions *= 180f / 3.14159f;

		Vector3 mTarg = rotationPositions;//new Vector3 (0, 0, 0);
		Vector3 rotationDistance = rotationPositions;

		mTarg *= -maxGimbalInput; // I don't know why, but I gotta (fukn) do this.

		double mag = (double)rotationPositions.magnitude;

		//Debug.Log (mag);
//		mTarg.x *= (float)(Math.Exp(Math.Abs(rotationPositions.x)/slope - distance )/(1+ Math.Exp(Math.Abs(rotationPositions.x)/slope-distance)));
//		mTarg.y *= (float)(Math.Exp(Math.Abs(rotationPositions.y)/slope - distance )/(1+ Math.Exp(Math.Abs(rotationPositions.y)/slope-distance)));
//		mTarg.z *= (float)(Math.Exp(Math.Abs(rotationPositions.z)/slope - distance )/(1+ Math.Exp(Math.Abs(rotationPositions.z)/slope-distance)));

		Vector3 mFuture = new Vector3 (0, 0, 0);
		Vector3 currentAngVel = ship.transform.InverseTransformDirection(ship.transform.GetComponent<Rigidbody>().angularVelocity);
		foreach(Vector3 momentVector in momentCurrent)
		{
			mFuture += momentVector;
		}
		currentAngVel += mFuture / futureFactor; 
	
		currentAngVel.x *= futureAngVFactor.x;
		currentAngVel.y *= futureAngVFactor.y;
		currentAngVel.z *= futureAngVFactor.z;

		Vector3 deltaAng = mTarg - currentAngVel;
		Vector3 targetAngles = new Vector3(th1,th2,th3);

		if(mag < lockMag) // A tiny hack to avoid the PID ossilations. Just set the exact velocity when we have pretty much converged.
		{//TODO remove little hack here, which seems reasonable in a game.
			if(deltaAng.magnitude > 0)
				ship.GetComponent<Rigidbody>().angularVelocity += ship.transform.TransformDirection(deltaAng/300f); 
			//ship.rigidbody.angularVelocity = Vector3.zero;
			return;
		}
		//Part 2, given the deltaAng, find a closed form solution given our gimbals.

		Vector3[] forceMoments = thrusterSubsystem.GetThrusterValues (ThrusterSubsystemPanel.ThrusterValueType.MomentFull,false);

//		Debug.Log (forceMoments.Length);

		int j;		
		float [,] vectorData = new float[3,3];
		for(int i=0; i < 3;i ++)
		{

			vectorData[i,0] = forceMoments[i].x;
			vectorData[i,1] = forceMoments[i].y;
			vectorData[i,2] = forceMoments[i].z;

	//		Debug.DrawLine(ship.transform.position,ship.transform.position + ship.transform.up*3,Color.red,0.1f);//+  ship.transform.TransformDirection( forceMoments[i]*4),Color.yellow,0.1f);
	//		Debug.DrawLine(ship.transform.position,ship.transform.position +  ship.transform.TransformDirection( forceMoments[i]*4),Color.yellow,0.1f);
		}
		//deltaAng /= 2; 
		Matrix vM = new Matrix(new float [3,1] {{deltaAng.x},{deltaAng.y},{deltaAng.z}});
		Matrix jM = new Matrix(vectorData);
		Matrix powerVectors = Matrix.Inverse(jM)*vM;
//		Debug.Log (jM);
//		Debug.Log (powerVectors);


		//powerVectors[0,0] = powerVectors[0,0]/2f;
		//powerVectors[1,0] = powerVectors[1,0]/2f;
		//powerVectors[2,0] = powerVectors[2,0]/2f;

		//powerVectors[0,0] = 0f;
		//powerVectors[2,0] = 0f;

		Matrix pv = Matrix.Transpose(powerVectors)*jM;
		Vector3 predictedAngVel = (Vector3)pv;
//		Debug.Log ("Power:" + (((Vector3) powerVectors)*100));

	//	Debug.Log ("PredAngVel:" + predictedAngVel);


		//SetJetPower(new int[]{10,11,12} , powerVectors);	
		SetJetPower(new int[]{10,11,12} , powerVectors,false);	

		return;
		//



	}

	// Choose the best basis thrusters for the current target.
	int[] ChooseBasisThrusters(Vector3 vTarg)
	{ 
		vTarg.Normalize ();

		Vector3[] jetDirections = thrusterSubsystem.GetThrusterValues (ThrusterSubsystemPanel.ThrusterValueType.ForceFull);

		List<int> jetList = new List<int> ();
		//First attempt, probably wont work well, choose three closest jets in vector space
		//Get three jets
		while( jetList.Count < 5)
		{
			int closestId = -1;
			//Loop through searching for the closest jets
			for (int i=0; i< jetDirections.Length; i++)
			{
				Vector3 direc = jetDirections[i];
				//direc = -direc;
				direc.Normalize();
				//Dont add jets that point in far directions
				if(closestId == -1 || 
				   Vector3.Angle(vTarg,direc) < Vector3.Angle(vTarg,jetDirections[closestId]))
				{
					//Dont add jets that you have already added
					if(!jetList.Contains(i))
					{
						//Don't add similar jets (such as those with the same direction) 
						int useJet = 1;
						for (int j=0; j< jetList.Count; j++)
						{
							if(Vector3.Angle(jetDirections[i],jetDirections[jetList[j]]) < maxJetAngle)
								useJet = 0;
						}
						if(useJet == 1)
							closestId = i;
					}
				}
			}
			if(closestId != -1)
				jetList.Add(closestId);
		}
		return jetList.ToArray ();
		//jetIndex1 = UnityEngine.Random.Range (0, jetPositions.Length - 1);
		//jetIndex2 = UnityEngine.Random.Range (0, jetPositions.Length - 1);
		//jetIndex3 = UnityEngine.Random.Range (0, jetPositions.Length - 1);

	}

	//ind - the indexes of the jets, powerVectors - the power to use for eac jet
	protected void SetJetPower(int[] ind, Matrix powerVectors, bool scaleUp = true)
	{
		Matrix powerG2 = new Matrix (powerVectors.GetData ().GetLength (0), powerVectors.GetData ().GetLength (1));

		//Debug.Log (powerVectors);
		int j = 0;
		float[,] powerGoal;
		powerGoal = powerVectors.GetData ();
		
		float power = 0; float maxPower = 0;
		for(int i=0; i < 3;i ++)
		{
			if(Mathf.Abs(powerGoal[i,0]) > maxPower)
				maxPower = Mathf.Abs(powerGoal[i,0]);
		}
		if (maxPower == 0)
			maxPower = 1;
		for(int i=0; i < 3;i ++)
		{
			powerG2[i,0] = powerGoal[i,0]/maxPower; 
			if (scaleUp == false && maxPower < 1)
				power = powerGoal[i,0];
			else
				power = powerGoal[i,0]/maxPower;
			j = ind[i];
			if(power < -1)
				power = -1;
			if(power > 1)
				power = 1;
			thrusterSubsystem.SetThrusterPower(j,power);
		}


	}

	public void SetUpVector(Vector3 theUpandUp)
	{
		this.upVector = theUpandUp;

	}




	void PulseThrusters()
	{
		jetPositions = thrusterSubsystem.GetThrusterValues (ThrusterSubsystemPanel.ThrusterValueType.Position);
		if (jetPositions == null)
			return;
		Vector3[] forceDirections = thrusterSubsystem.GetThrusterValues (ThrusterSubsystemPanel.ThrusterValueType.ForceFull);
		if (jetPositions.Length == 0)
			return; //nope nope nope nope nope nope nope nope nope nope 
		for(int j =0;j< jetPositions.Length;j++)
			thrusterSubsystem.SetThrusterPower(j,0f);
		if (movementVector == Vector3.zero)
						return;

	//	movementVector = new Vector3(0,0,100);


		Vector3 vTarg = ship.transform.InverseTransformDirection (movementVector); // This is the target position.
		Vector3 vCurrent = ship.transform.InverseTransformDirection (ship.GetComponent<Rigidbody>().velocity); // This is the current Direction.

		//Vector3 vTarg = vTargIn.normalized;

		float potentialPower = powerScale/10f;
/*		if(vTarg.magnitude - vCurrent.magnitude >  potentialPower)
		{
			vTarg = vTarg.normalized * (vCurrent.magnitude +potentialPower);
		}*/

		Vector3 vChange = (vTarg*pulseTime - vCurrent*futureFactor); // The direction the jets need to fire to correct our velocity
		//vChange.Normalize ();
		//vChange *= 10;
		Vector3 speedPredicted;
		float effectiveFactor;




		//return;

		/*
		Debug.DrawLine (ship.transform.position, 
		                ship.transform.position + ship.transform.TransformDirection((((Vector3)vTarg))),Color.magenta,0.05f );
		Debug.DrawLine (ship.transform.position, 
		                ship.transform.position + ship.transform.TransformDirection((((Vector3)vChange))),Color.green,0.05f );
		*/
		//Debug.Log (movementVector);

		if ((vTarg - ship.GetComponent<Rigidbody>().velocity).magnitude < targetTolerance && vTarg == Vector3.zero)
		{
			//Debug.Log("Target reached");
			return;
		}
		int[] ind = ChooseBasisThrusters (vChange);
		//We have five basis thrusters to choose from:
		int basisSize = 4;
		int[][] inds = new int[basisSize][];
		Matrix[] powerVectorss = new Matrix[basisSize]; 
		float[] choiceFactor = new float[basisSize];

		inds[0]  = new int[]{ ind [0],ind [1],ind [2]};
		inds[1]  = new int[]{ ind [0],ind [1],ind [3]};
		inds[2]  = new int[]{ ind [0],ind [3],ind [2]};
		inds[3]  = new int[]{ ind [3],ind [1],ind [2]};

		List<int> indexes = new List<int> ();
		Vector3[] speedsPredicted = new Vector3[basisSize];
		for(int n=0;n < inds.Length;n++)
		{
			powerVectorss[n] = GetThrusterPower (forceDirections, inds[n], engineFactor, vChange, out speedPredicted);
			speedsPredicted[n] = speedPredicted;
			choiceFactor[n] = 0;
			float minVal = 1000f;
			float maxVal = 0f;
			for(int x=0; x < 3;x ++)
			{
				if(powerVectorss[n][x,0] < 0)
					choiceFactor[n] -= 100f;
				if(Mathf.Abs(powerVectorss[n][x,0]) < minVal)
					minVal = Mathf.Abs(powerVectorss[n][x,0]);
				if(Mathf.Abs(powerVectorss[n][x,0]) > maxVal)
					maxVal = Mathf.Abs(powerVectorss[n][x,0]);
			}
			for(int x=0; x < 3;x ++)
			{
				choiceFactor[n] -= Mathf.Abs(powerVectorss[n][x,0])/minVal;
			}
/*			for(int x=0; x < 3;x ++)
			{
				choiceFactor[n] += Mathf.Abs(powerVectorss[n][x,0])/maxVal;
			}*/

		}

		float maxChoice = -5000f;
		int thrusterN = 0;
		for(int n=0;n < inds.Length;n++)
		{
			if(choiceFactor[n] > maxChoice)
			{
				maxChoice = choiceFactor[n] ;
				thrusterN = n;
			}
		}

		Matrix powerVectors = powerVectorss[thrusterN];
		speedPredicted = speedsPredicted[thrusterN];

		powerVectors = ScaleThrusterPower(powerVectors, vChange, out effectiveFactor);

		Vector3 predictLarger = vCurrent + (((Vector3)speedPredicted)* effectiveFactor)*cutoffPredictionFactor;
		if (Vector3.Angle (predictLarger, vTarg) > Vector3.Angle (vCurrent, vTarg))
		{
			return;
		}


		/////////////                                      ///////////////
		///////////// MAJOR HACK - HARD CODED ENGINE POWER ///////////////
		/// 

		//if (vChange.magnitude > potentialPower)
		//	vChange = vChange.normalized * potentialPower;



		float snappyPowerFactor = 1f; //Allow for the speed to be scaled when

		//Vector3 commonVec = Vector3.Project (speedPredicted.normalized, ship.rigidbody.velocity.normalized);
	//	snappyPowerFactor = (1 - 1 * commonVec.magnitude) + 0.1f*commonVec.magnitude;
	//	if ((commonVec.normalized + ship.rigidbody.velocity.normalized).magnitude < 1)
	//		snappyPowerFactor = 1f;


		ship.GetComponent<Rigidbody>().velocity += ship.transform.TransformDirection(speedPredicted*snappyPowerFactor);
		
		//////////////////////////////////////////////////////////////////
		//////////////////////////////////////////////////////////////////
		SetJetPower(ind, powerVectors,forceEnginesFull);
	}




	private Matrix GetThrusterPower(Vector3[] forceDirections,int[] ind,float engineFactor,Vector3 vChange, out Vector3 speedPredicted )
	{
		int j;		
		float [,] vectorData = new float[3,3];
		
		for(int i=0; i < 3;i ++)
		{
			j = ind[i];
			
			vectorData[i,0] = forceDirections[j].x*engineFactor;
			vectorData[i,1] = forceDirections[j].y*engineFactor;
			vectorData[i,2] = forceDirections[j].z*engineFactor;
		}

		Vector3 directionChange = vChange;
		directionChange.Normalize ();
		
		Matrix vM = new Matrix(new float [3,1] {{directionChange.x},{directionChange.y},{directionChange.z}});
		Matrix jM = new Matrix(vectorData);
		
		Matrix powerVectors = Matrix.Inverse(jM)*vM;
		speedPredicted = (Vector3)(jM*powerVectors);
		return powerVectors;

	}


	private Matrix ScaleThrusterPower(Matrix powerVectors, Vector3 vChange,out float effectiveFactor)
	{
		float maxPower = 0f;
		for(int i=0; i < 3;i ++)
		{
			if(Mathf.Abs(powerVectors[i,0]) > maxPower)
				maxPower = Mathf.Abs(powerVectors[i,0]);
		}
		
		//Matrix speedPredicted = jM*powerVectors;
		Vector3 speedPredicted = vChange;
		
		float wantedFactor = vChange.magnitude / ((Vector3)speedPredicted).magnitude;
		float highestFactor = 1/maxPower;
		effectiveFactor = Mathf.Min (wantedFactor, highestFactor);

		for(int i=0; i < 3;i ++)
		{
			powerVectors[i,0] *= effectiveFactor*powerScale;
			if(powerVectors[i,0] < 0)
				powerVectors[i,0] = 0;
		}
		return powerVectors;
	}


	protected override void PlayerInteract(Vector3 position,bool focused)
	{
		

		//Debug.Log("Hover and shiz");

		//Renderer rend = this.transform.FindChild ("Console").GetComponent<Renderer> ();
		//rend.material.SetColor (0, Color.red);
	}


}
