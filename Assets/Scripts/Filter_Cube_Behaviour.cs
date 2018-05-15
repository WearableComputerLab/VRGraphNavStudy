using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Filter_Cube_Behaviour : MonoBehaviour {

    public string Query;
    public Mesh def_mesh, filtered_mesh;
    public Color filterColor;
    private GameObject queryObject;
    private ArrayList filterNodes = new ArrayList();
    private ArrayList filterParents = new ArrayList();
    ArrayList queryStrings = new ArrayList();
    ArrayList ageStrings = new ArrayList();
    public GraphVisualizer graph;
    public Label_Switcher[] labels;
    public Power_Button_Behaviour[] pwr_btns;
    public bool sendQuery = false;
    public SteamVR_TrackedObject tracker;
    private Power_Button_Behaviour queryButton;
    public Range_Label Ranged_Attribute;
    public Power_Button_Behaviour Ranged_Power_Button;
    public bool ranged_On = false;
    public SteamVR_TrackedController cont;

    public bool a, b, c, d, e;

	// Use this for initialization
	void Start () {
        queryObject = new GameObject();
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.Lerp(transform.rotation, tracker.transform.rotation, 20 * Time.deltaTime);
        transform.position = tracker.transform.position + tracker.transform.forward * 0.2f;
        // Check if a ranged attribute is powered
        if (Ranged_Power_Button != null) {
            ranged_On = Ranged_Power_Button.ActiveQuery;
        }

        if(queryButton.ActiveQuery == true && sendQuery == false) {
            Query = "";
            queryStrings = new ArrayList();
            ageStrings = new ArrayList();

            for(int i = 0; i < labels.Length; i++) {
                if (pwr_btns[i].ActiveQuery) {
                    // maybe should be arrayList? instead of string
                    Query += labels[i].ToString() +" ";
                    queryStrings.Add(labels[i].ToString());
                }
            }
            if (ranged_On) {
                int min = Ranged_Attribute.A;
                int max = Ranged_Attribute.B;
                for(int i = min; i <= max; i++) {
                    ageStrings.Add("age-" + i);
                }
            }
            SendQuery();
            sendQuery = true;
        }
        if (queryButton.ActiveQuery == false) {
            sendQuery = false;
        }
        SteamVR_Controller.Device device = SteamVR_Controller.Input((int)tracker.index);
        cont = tracker.GetComponent<SteamVR_TrackedController>();
        

    }

    public void setQueryButton(Power_Button_Behaviour btn) {
        this.queryButton = btn;
    }

    public void SendQuery() {
        ArrayList graphNodes = graph.returnGraphNodes();
        if (filterNodes.Count > 0) {
            foreach (VisualNode v in filterNodes) {
                graph.removeIgnoreNode(v);
                //v.ColorDef();
                v.gameObject.SetActive(false);
                v.GetComponent<MeshFilter>().mesh = def_mesh;
            }
        }
        filterNodes = new ArrayList();
        foreach (GameObject g in graphNodes) {
            VisualNode v = g.GetComponent<VisualNode>();
            int queryCount = 0;
            if (ageStrings.Count == 0) {
                foreach (string s in queryStrings) {
                    if (s.Equals(string.Empty) == false) {
                        print(v.getAttributeInfo().ToLower().Contains(s.ToLower()));
                        if (v.getAttributeInfo().ToLower().Contains(s.ToLower())) {
                            queryCount++;
                        }
                    }
                }
                if (queryCount == queryStrings.Count && Query.Equals(string.Empty) == false) {
                    print("Found Node");
                    filterNodes.Add(v);
                    graph.addIgnoreNode(v);
                }
            } else {
                bool foundAge = false;
                foreach(string s in ageStrings) {
                    if (s.Equals(string.Empty) == false) {
                        if (v.getAttributeInfo().ToLower().Contains(s.ToLower())) {
                            foundAge = true;
                            Query += "found age";
                        }
                    }
                }
                foreach (string s in queryStrings) {
                    if (s.Equals(string.Empty) == false) {
                        print(v.getAttributeInfo().ToLower().Contains(s.ToLower()));
                        if (v.getAttributeInfo().ToLower().Contains(s.ToLower())) {
                            queryCount++;
                        }
                    }
                }
                if (queryCount == queryStrings.Count && Query.Equals(string.Empty) == false && foundAge == true) {
                    print("Found Node");
                    filterNodes.Add(v);
                    graph.addIgnoreNode(v);
                }
                if(queryStrings.Count == 0 && foundAge) {
                    print("Found Node");
                    filterNodes.Add(v);
                    graph.addIgnoreNode(v);
                }
            }
        }

        ArrayList clusters = graph.returnClusterNodes();
        foreach(ClusterBehaviour c in clusters) {
            if(Query.Equals(string.Empty)) {
                c.Brighten();
            } else {
                c.Dim();
            }
            c.ExitHighlight();
            c.RecreateCluster();
        }
        CreateFilterCluster();
    }

    public void CreateFilterCluster() {
        if(filterNodes.Count > 0)
        foreach(VisualNode v in filterNodes) {
            print(v.node_name);
                //v.transform.parent = null;
                //v.ColorWhite();
            v.GetComponent<MeshFilter>().mesh = filtered_mesh;
            v.gameObject.SetActive(true);
        }
    }
   

}
