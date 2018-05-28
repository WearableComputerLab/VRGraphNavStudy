using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Recorder : MonoBehaviour {
	
	[Header("Change With Every Build")]
	// Change with every build
	public string NavigationToRecord = "";	

	// Recorded Devices
	private Transform realHMD;
	private Transform simulatedHMD;
	[Header("Left and Right Controller to Record")]
	public Transform leftController;
	public Transform rightController;

	// String arrays for measurements
	private ArrayList realMovement = new ArrayList();
	private ArrayList realRotation = new ArrayList();
	private ArrayList realVelocity = new ArrayList(); 
	private ArrayList realAngularVelocity = new ArrayList();
	private ArrayList simulatedMovement = new ArrayList();
	private ArrayList simulatedVelocity = new ArrayList();
	private ArrayList leftControllerMovement = new ArrayList(); 
	private ArrayList leftControllerRotation = new ArrayList();
	private ArrayList leftControllerVeloctiy = new ArrayList(); 
	private ArrayList leftAngularVelocity = new ArrayList();
	private ArrayList rightControllerMovement = new ArrayList();
	private ArrayList rightControllerRotation = new ArrayList();
	private ArrayList rightControllerVelocity = new ArrayList();
	private ArrayList rightAngularVelocity = new ArrayList ();
	private ArrayList taskTime = new ArrayList();
	private ArrayList visibleCount = new ArrayList();

	// Time
	private float currentTime = 0;
	private float recurringTime = 0;
	private float maxTime = 1;
	private float prevTime = 0;

	// Velocity Variables
	private Vector3 realPrevPos;
	private Vector3 simulatedPrevPos;
	private Vector3 leftPrevPos;
	private Vector3 rightPrevPos;

	// Angular Velocity Variables
	private Vector3 realPrevRotation;
	private Vector3 leftPrevRotation;
	private Vector3 rightPrevRotation;

	// Reference to study object
	private StudyManager studyManager;
	
	// Reference to the current task (SF and CF)
	private int currentTask = 0;

	// Position at the last task
	private Vector3 RprevTaskpos;
	private Vector3 SprevTaskpos;

	// checking for visibility
	private ClusterBehaviour[] renderers;
	private int count = 0;
	private bool[] visibles;
	private Vector3 centerPoint;
	private float distFromCenter;


	// Use this for initialization
	private void Start () {
		realHMD = Camera.main.transform;
		simulatedHMD = realHMD.parent;

		realPrevPos = realHMD.position;
		simulatedPrevPos = simulatedHMD.position;
		leftPrevPos = leftController.position;
		rightPrevPos = rightController.position;

		realPrevRotation = realHMD.eulerAngles;
		leftPrevRotation = leftController.eulerAngles;
		rightPrevRotation = rightController.eulerAngles;
		
		studyManager = FindObjectOfType<StudyManager>();

		RprevTaskpos = realPrevPos;
		SprevTaskpos = simulatedPrevPos;
	}
	
	// Update is called once per frame
	private void Update () {
		currentTime += Time.deltaTime;
		if(recurringTime < maxTime){
			recurringTime += Time.deltaTime;
		} else {
			recurringTime = 0;
			Record();
		}
	}

	private void Record () {
		// check what camera sees and count
		checkCameraFrustrum();

		// Movement and Velocity
		realMovement.Add((currentTime + "," + realHMD.position.x+","+realHMD.position.y+","+realHMD.position.z) as string);
		realRotation.Add((currentTime + "," + realHMD.eulerAngles.x+","+realHMD.eulerAngles.y+","+realHMD.eulerAngles.z) as string);
		Vector3 vel = realHMD.position - realPrevPos;
		realVelocity.Add ((currentTime + "," + vel.x+","+vel.y+","+vel.z) as string);

		simulatedMovement.Add ((currentTime + "," + simulatedHMD.position.x +","+simulatedHMD.position.y+","+simulatedHMD.position.z) as string);
		vel = simulatedHMD.position - simulatedPrevPos;
		simulatedVelocity.Add ((currentTime + "," + vel.x+","+vel.y+","+vel.z) as string);

		leftControllerMovement.Add ((currentTime + "," + leftController.position.x+","+leftController.position.y+","+leftController.position.z) as string);
		leftControllerRotation.Add ((currentTime + "," + leftController.eulerAngles.x+","+leftController.eulerAngles.y+","+leftController.eulerAngles.z) as string);
		vel = leftController.position - leftPrevPos;
		leftControllerVeloctiy.Add ((currentTime + "," + vel.x+","+vel.y+","+vel.z) as string);

		rightControllerMovement.Add ((currentTime + "," + rightController.position.x+","+rightController.position.y+","+rightController.position.z) as string);
		rightControllerRotation.Add ((currentTime + "," + rightController.eulerAngles.x+","+rightController.eulerAngles.y+","+rightController.eulerAngles.z) as string);
		vel = rightController.position - rightPrevPos;
		rightControllerVelocity.Add ((currentTime + "," + vel.x+","+vel.y+","+vel.z) as string);

		// Angular velocity
		vel = realHMD.eulerAngles - realPrevRotation;
		realAngularVelocity.Add((currentTime + "," + vel.x+","+vel.y+","+vel.z) as string);
		vel = leftController.eulerAngles - leftPrevRotation;
		leftAngularVelocity.Add((currentTime + "," + vel.x+","+vel.y+","+vel.z) as string);
		vel = rightController.eulerAngles - rightPrevRotation;
		rightAngularVelocity.Add((currentTime + "," + vel.x+","+vel.y+","+vel.z) as string);	

		// Check how much the camera sees
		visibleCount.Add(currentTime+","+count+","+distFromCenter);

		// set all prev variables
		realPrevPos = realHMD.position;
		simulatedPrevPos = simulatedHMD.position;
		leftPrevPos = leftController.position;
		rightPrevPos = rightController.position;

		realPrevRotation = realHMD.eulerAngles;
		leftPrevRotation = leftController.eulerAngles;
		rightPrevRotation = rightController.eulerAngles;				
	}

	public void addTask(){
		currentTask++;
		float distTravelled = (realHMD.position - RprevTaskpos).sqrMagnitude;
		float simDistTravelled = (simulatedHMD.position - SprevTaskpos).sqrMagnitude;
		float actualTime = currentTime - prevTime;
		taskTime.Add(currentTask+","+actualTime+","+distTravelled+","+simDistTravelled);
		SprevTaskpos = simulatedHMD.position;
		RprevTaskpos = realHMD.position;
		prevTime = currentTime;
	}

	private void checkCameraFrustrum(){
		renderers = FindObjectsOfType<ClusterBehaviour>();
		visibles = new bool[renderers.Length];
		int i = 0;
		float x = 0;
		float y = 0;
		float z = 0;
		foreach(ClusterBehaviour r in renderers){
			Renderer render = r.GetComponent<Renderer>();
			x += render.transform.position.x;
			y += render.transform.position.y;
			z += render.transform.position.z;
			if(isVisible(render)){
				visibles[i] = true;
			} else {
				visibles[i] = false;
			}
			i++;
		}
		centerPoint = new Vector3(x/renderers.Length,y/renderers.Length,z/renderers.Length);
		distFromCenter = (realHMD.position - centerPoint).sqrMagnitude;
		count = 0;
		foreach(bool b in visibles){
			if(b == true){
				count++;
			}
		}	
	}

	private bool isVisible(Renderer renderer){
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
		if(GeometryUtility.TestPlanesAABB(planes, renderer.bounds)){
			return true;
		} else {
			return false;
		}
	}
	
	public void outputToCSV(){
		string filename = @".\" + studyManager.GetHashCode() +"_"+NavigationToRecord+"_"+SceneManager.GetActiveScene().name+".csv";
        if (!File.Exists(filename)) {           
            TextWriter writer = new StreamWriter(filename,true);

			writer.WriteLine("Time,RPx,RPy,RPz");
            foreach(string s in realMovement) {
                writer.WriteLine(s);
            }

			writer.WriteLine("Time,RRx,RRy,RRz");
            foreach(string s in realRotation) {
                writer.WriteLine(s);
            }

			writer.WriteLine("Time,RVx,RVy,RVz");
            foreach(string s in realVelocity) {
                writer.WriteLine(s);
            }

			writer.WriteLine("Time,RAVx,RAVy,RAz");
            foreach(string s in realAngularVelocity) {
                writer.WriteLine(s);
            }

			writer.WriteLine("Time,SMx,SMy,SMz");
            foreach(string s in realAngularVelocity) {
                writer.WriteLine(s);
            }

			writer.WriteLine("Time,SVx,SVy,SVz");
            foreach(string s in simulatedVelocity) {
                writer.WriteLine(s);
            }

			writer.WriteLine("Time,LCPx,LCPy,LCPz");
            foreach(string s in realAngularVelocity) {
                writer.WriteLine(s);
            }

			writer.WriteLine("Time,LCRx,LCRy,LCRz");
            foreach(string s in leftControllerRotation) {
                writer.WriteLine(s);
            }
			
			writer.WriteLine("Time,LCVx,LCVy,LCVz");
            foreach(string s in leftControllerVeloctiy) {
                writer.WriteLine(s);
            }

			writer.WriteLine("Time,LCAVx,LCAVy,LCAVy");
            foreach(string s in leftAngularVelocity) {
                writer.WriteLine(s);
            }

			writer.WriteLine("Time,RCPx,RCPy,RCPz");
            foreach(string s in rightControllerMovement) {
                writer.WriteLine(s);
            }

			writer.WriteLine("Time,RCRx,RCRy,RCRz");
            foreach(string s in rightControllerRotation) {
                writer.WriteLine(s);
            }
			
			writer.WriteLine("Time,RCVx,RCVy,RCVz");
            foreach(string s in rightControllerVelocity) {
                writer.WriteLine(s);
            }

			writer.WriteLine("Time,RCAVx,RCAVy,RCAVz");
            foreach(string s in rightAngularVelocity) {
                writer.WriteLine(s);
            }

			writer.WriteLine("Time,vCount,d");
            foreach(string s in visibleCount) {
                writer.WriteLine(s);
            }
			
			writer.WriteLine("Task,t,rD,simD");
            foreach(string s in taskTime) {
                writer.WriteLine(s);
            }
			
            if (!File.Exists(filename)) {
                File.Create(filename).Close();
            }
            writer.Close();
        }
	}
}
