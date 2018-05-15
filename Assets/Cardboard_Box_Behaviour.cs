using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cardboard_Box_Behaviour : MonoBehaviour {

    // Arraylist saves all nodes that are collected in the box
    private ArrayList savedNodes = new ArrayList();
    private string dataset = "box_data";
    public bool saveToCSV = false;
    public bool debug = false;
    public VisualNode[] debugNodes;
    private bool nodeInBox = false;
    private bool savedAnim = false; 
    private Animator anim;
    private Renderer meshrenderer;
    private Text nodesText;
    private int textCount = 1;
    private string default_text;
    private Animator paper_plane_anim;

    void Awake() {
        GraphVisualizer g = FindObjectOfType<GraphVisualizer>();
        dataset = g.dataset.name;
        anim = GetComponent<Animator>();
        meshrenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        nodesText = transform.GetChild(2).GetComponentInChildren<Text>();
        default_text = nodesText.text;
        paper_plane_anim = transform.GetChild(3).GetComponent<Animator>();
    }

	void LateUpdate () {
        // for debugging (save to csv if boolean is true and count > 0)
        if(saveToCSV) {
            if(savedNodes.Count > 0) { 
                OutputToCSV();
                saveToCSV = false;
            }
        }
        if(debug) {
            savedNodes = new ArrayList();
            savedNodes.AddRange(debugNodes);
            print("added : "+savedNodes.Count+" nodes to savedNodes");
            debug = false;
        }

        if(nodeInBox) {
            anim.SetBool("expand",true);
        } else {
            anim.SetBool("expand",false);
        }

        if(savedAnim) {
            anim.SetBool("add",true);

        } else {

        }

	}

    // Adds a node to the savebox
    public void AddNode(VisualNode v) {
        savedNodes.Add(v);
        if(textCount < 6) { 
            nodesText.text += "\n" + v.node_name;
        } else {
            nodesText.text += "\n" + "...";
        }
        textCount++;
    }

    // Executes after hitting the save button, saves all nodes to csv
    public void OutputToCSV() {
        paper_plane_anim.SetBool("fly",true);
        transform.GetChild(4).gameObject.SetActive(true);
        // set the file name for the csv output
        string filename = @".\" + dataset + "_interests"+ ".csv";
        if (!File.Exists(filename)) {           
            TextWriter writer = new StreamWriter(filename,true);
            writer.WriteLine("Node Name,"+"Edges,"+"Unstructured Data");
            foreach(VisualNode g in savedNodes) {
                writer.WriteLine(g.getNode().NodeID1 +","+ string.Join(" ", g.adjVisNodes)+","+g.attribute_info.Replace("\n"," "));
            }
            if (!File.Exists(filename)) {
                File.Create(filename).Close();
            }
            // close writer
            writer.Close();
        }
        // reset saved nodes after saving
        savedNodes = new ArrayList();
        textCount = 0;
        nodesText.text = default_text;
        //paper_plane_anim.SetBool("fly",false);
    }

    public void inBox() {
        nodeInBox = true;
        meshrenderer.material.SetColor("_EmissionColor",Color.grey);
    }

    public void outBox() {
        nodeInBox = false;
        savedAnim = true;
        meshrenderer.material.SetColor("_EmissionColor",Color.black);
    }



}
