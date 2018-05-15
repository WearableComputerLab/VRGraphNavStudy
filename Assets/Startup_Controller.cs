using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System;

using System.Runtime.InteropServices;

public class Startup_Controller : MonoBehaviour {

    [DllImport("forms.dll")]
    public static extern void OpenFileDialog();


    public Text file_name, attribute_text;
    public Toggle is_quadrant;
    public bool quadrant_layout_on = false;

    public Text k_cluster_label;
    public int k_clusters = 0;
    private List<string> graph_options = new List<string>();
    public List<TextAsset> text_files = new List<TextAsset>();

    System.Windows.Forms.OpenFileDialog o;
    public string loadType = "";

    public string graph_dataset, attribute_dataset;

    // Use this for initialization
    void Start () {
        GetTextFiles();
        DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		if(o != null) {
            if(o.ShowDialog()== System.Windows.Forms.DialogResult.OK) {
                if(o.OpenFile() != null) {
                    string fileName = o.FileName;
                    if (loadType == "graphData") {
                        graph_dataset = File.ReadAllText(fileName);
                        file_name.text = fileName;
                    }
                    if (loadType == "attributeData") {
                        attribute_dataset = File.ReadAllText(fileName);
                        attribute_text.text = fileName;
                    }

                }

                o = null;
                loadType = "";
                print("close dialog");
            }
        }
        if(is_quadrant != null) {
            quadrant_layout_on = is_quadrant.isOn;
        }
        if(k_cluster_label != null) {
            int output;
            if (Int32.TryParse(k_cluster_label.text, out output)) {
                k_clusters = output;
            } else {
                k_cluster_label.text = "";
            }
        }
	}

    public void LoadVisualizationScene() {
        SceneManager.LoadScene(1);
    }

    public void LoadTextAsset() {
        o = new System.Windows.Forms.OpenFileDialog();
        loadType = "graphData";
    }

    public void LoadAttributeAsset() {
        o = new System.Windows.Forms.OpenFileDialog();
        loadType = "attributeData";
    }

    void GetTextFiles() {

    }

}
