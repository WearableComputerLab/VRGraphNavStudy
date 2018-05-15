using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Label_Viewer : MonoBehaviour {

    public Label_Switcher label_parent;
    public int index_addition;
    private Text textLabel;

	// Use this for initialization
	void Start () {
        textLabel = GetComponent<Text>();
    }
	
	// Update is called once per frame
	void LateUpdate () {
        int currentIndex = label_parent.returnIndex();
        if(currentIndex > 0 && currentIndex < label_parent.values.Length-1) {
            textLabel.text = label_parent.values[currentIndex + index_addition];
        } 
        if(index_addition == -1 && currentIndex == 0) {
            textLabel.text = "";
        }
        if (index_addition == 1 && currentIndex == label_parent.values.Length-1) {
            textLabel.text = "";
        }
    }
}
