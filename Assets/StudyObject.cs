using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class StudyObject : MonoBehaviour {
    public GraphVisualizer graph;
    private Dual_Controller_Behaviour controllerBehaviour;
    public StudyAssign studyAssign;
    public World_Miniature_Graph wim;
    public Text splashText;
    private float splashTime = 4;
    
    bool stop = false;
    public int studyStage = 1;
    public int stageI = 0;
    public int taskNum = 0;
    public int[] Data_Order;
    public TextAsset[] simpleFind;
    public TextAsset[] complexFind;
    public TextAsset[] circleCount;
    public TextAsset[] AssocCount;
    // Text Assets Chosen
    public TextAsset simpleFindAsset;
    public TextAsset complexFindAsset;
    public TextAsset circleCountAsset;
    public TextAsset assocCountAsset;
    // Study Prompt
    public StudyText studyPrompt;

    private ArrayList simpleFindNodes = new ArrayList();
    private ArrayList complexFindNodes = new ArrayList();
    private ArrayList circleCountNodes = new ArrayList();
    private ArrayList AssocCountNodes = new ArrayList();

    private ArrayList vis_nodes;
    private StudyNode[] studyNodes;
    public StudyNode currentStudyNode;

    public bool simpleStart, complexStart, circleStart, assocStart;
    
    public Document_Close_Button[] closeBtns;
    private ArrayList csv_output = new ArrayList();
    private string currentTask = "Simple Find";
    private string participantID = "1";
    private string navType = "Archery";

    public bool testCSVOutput = false;

	// Use this for initialization
	void Start () {
		studyNodes = transform.GetComponentsInChildren<StudyNode>();
        studyAssign = FindObjectOfType<StudyAssign>();
        controllerBehaviour = FindObjectOfType<Dual_Controller_Behaviour>();
        if(studyAssign.HandOrientArray[0] == "Right Handed")
        {
            controllerBehaviour.leftHanded = false;
        }
        if(studyAssign.HandOrientArray[0] == "Left Handed")
        {
            controllerBehaviour.leftHanded = true;
        }
        csv_output.Add("Task,Node ID,Time");
        studyStage = studyAssign.TaskArray[stageI];
        participantID = studyAssign.ParticipantID;
        navType = studyAssign.currentNavigation;
	}
	
	// Update is called once per frame
	void Update () {
		if(graph.graphIsClustered() && !stop) {
            int pickNumber = studyAssign.DataArray[0];
            vis_nodes = graph.returnGraphNodes();

            // SimpleFind
            simpleFindAsset = simpleFind[pickNumber];
            string s = simpleFindAsset.text.Replace("\r", "\n");
            string[] lines = s.Split("\n"[0]);
            for(int i = 0; i < lines.Length; i++) {
                if(lines[i].Equals(string.Empty)==false)
                simpleFindNodes.Add(search(lines[i]));
            }

            //ComplexFind
            pickNumber = studyAssign.DataArray[1];
            complexFindAsset = complexFind[pickNumber];
            s = complexFindAsset.text.Replace("\r", "\n");
            lines = s.Split("\n"[0]);
            for(int i = 0; i < lines.Length; i++) {
                if(lines[i].Equals(string.Empty)==false)
                complexFindNodes.Add(search(lines[i]));
            }

            //CircleCount
            pickNumber = studyAssign.DataArray[2];
            circleCountAsset = circleCount[pickNumber];
            s = circleCountAsset.text.Replace("\r", "\n");
            lines = s.Split("\n"[0]);
            for(int i = 0; i < lines.Length; i++) {
                if(lines[i].Equals(string.Empty)==false)
                circleCountNodes.Add(search(lines[i]));
            }

            //AssocCount
            pickNumber = studyAssign.DataArray[3];
            assocCountAsset = AssocCount[pickNumber];
            s = assocCountAsset.text.Replace("\r", "\n");
            lines = s.Split("\n"[0]);
            for(int i = 0; i < lines.Length; i++) {
                if(lines[i].Equals(string.Empty)==false)
                AssocCountNodes.Add(search(lines[i]));
            }
            stop = true;
        }
        if (graph.graphIsClustered() && stop == true) {
            // set positions for nodes (simple nodes)
            if (studyStage == 1 && simpleStart == false) {
                currentTask = "Simple Find";
                for (int i = 0; i < simpleFindNodes.Count; i++) {
                    VisualNode v = (VisualNode)simpleFindNodes[i];
                    studyNodes[i].transform.position = v.transform.position;
                    studyNodes[i].setTarget(v);
                    if (i == 0) {
                        currentStudyNode = studyNodes[i];
                        currentStudyNode.gameObject.SetActive(true);
                        closeBtns[0].setStudyNode(currentStudyNode);
                        closeBtns[1].setStudyNode(currentStudyNode);
                        } else {
                        studyNodes[i - 1].setNext(studyNodes[i]);
                        studyNodes[i].gameObject.SetActive(false);
                        }
                    }
                simpleStart = true;
                }
            // set positions for nodes (complex nodes)
            if (studyStage == 2 && complexStart == false) {
                currentTask = "Complex Find";
                for (int i = 0; i < complexFindNodes.Count; i++) {
                    VisualNode v = (VisualNode)complexFindNodes[i];
                    studyNodes[i].transform.position = v.transform.position;
                    studyNodes[i].setTarget(v);
                    if (i == 0) {
                        currentStudyNode = studyNodes[i];
                        currentStudyNode.gameObject.SetActive(true);
                        closeBtns[0].setStudyNode(currentStudyNode);
                        closeBtns[1].setStudyNode(currentStudyNode);
                        } else {
                        studyNodes[i - 1].setNext(studyNodes[i]);
                        studyNodes[i].gameObject.SetActive(false);
                        }
                    }
                complexStart = true;
                }
            // set positions for nodes (circle nodes)
            if (studyStage == 3 && circleStart == false) {
                currentTask = "Circle Find";
                for (int i = 0; i < circleCountNodes.Count; i++) {
                    VisualNode v = (VisualNode)circleCountNodes[i];
                    studyNodes[i].transform.position = v.transform.position;
                    studyNodes[i].setTarget(v);
                    if (i == 0) {
                        currentStudyNode = studyNodes[i];
                        currentStudyNode.gameObject.SetActive(true);
                        closeBtns[0].setStudyNode(currentStudyNode);
                        closeBtns[1].setStudyNode(currentStudyNode);
                        } else {
                        studyNodes[i - 1].setNext(studyNodes[i]);
                        studyNodes[i].gameObject.SetActive(false);
                        }
                    }
                circleStart = true;
                }
            // set positions for nodes (circle nodes)
            if (studyStage == 4 && assocStart == false) {
                currentTask = "Association Find";
                for (int i = 0; i < AssocCountNodes.Count; i++) {
                    VisualNode v = (VisualNode)AssocCountNodes[i];
                    studyNodes[i].transform.position = v.transform.position;
                    studyNodes[i].setTarget(v);
                    if (i == 0) {
                        currentStudyNode = studyNodes[i];
                        currentStudyNode.gameObject.SetActive(true);
                        closeBtns[0].setStudyNode(currentStudyNode);
                        closeBtns[1].setStudyNode(currentStudyNode);
                        } else {
                        studyNodes[i - 1].setNext(studyNodes[i]);
                        studyNodes[i].gameObject.SetActive(false);
                        }
                    }
                assocStart = true;
                }
            }
        if(testCSVOutput) {
            createCSV();
            testCSVOutput = false;
        }
        if(studyAssign != null)
        {
            splashTime -= Time.deltaTime;
            if (splashTime > 0) {
                if (studyAssign.initialLoad > 1)
                {
                    splashText.text = studyAssign.currentNavigation;
                } else
                {
                    splashText.text = "INITIAL LOAD TEST";
                }
            } else {
                splashText.text = "";
            }
            if (studyAssign.currentNavigation.Equals("WM") == false)
            {
                wim.gameObject.SetActive(false);
            }

        }
	}

    VisualNode search(string s) {
        foreach(GameObject g in vis_nodes) {
            VisualNode v = g.GetComponent<VisualNode>(); 
            if(v.node_name.Equals(s)) {
                return v;
            }
        }
        return null;
    }

    public void setCurrentNode(StudyNode s) {
        csv_output.Add(currentTask+","+s.getID()+","+currentStudyNode.getTime_Taken());
        if(stageI < 3) { 
            studyPrompt.moveUI(currentStudyNode.transform.position);
        }
        currentStudyNode.resetTime();
        currentStudyNode = s;
        closeBtns[0].setStudyNode(s);
        closeBtns[1].setStudyNode(s);
        taskNum++;
    }

    public void setStage() {
        stageI++;
        studyStage = studyAssign.TaskArray[stageI];
        taskNum = 0;
        studyPrompt.moveUI(currentStudyNode.transform.position);
        if(stageI == 4) {
            createCSV(); 
        }
    }

    public void createCSV() {
        string filename = @".\" + participantID +"_"+ navType + ".csv";
        if (!File.Exists(filename)) {           
            TextWriter writer = new StreamWriter(filename,true);
            foreach(string s in csv_output) {
                writer.WriteLine(s);
            }
            if (!File.Exists(filename)) {
                File.Create(filename).Close();
            }
            writer.Close();
        }
    }

}
