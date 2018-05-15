using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_SaveButton : MonoBehaviour {

    public Cardboard_Box_Behaviour box;
    private MeshRenderer model;
    public bool highlight_button = false;
    public bool push_button = false;
    public float time_push = 1;
    private bool outputCSV = false;

	// Use this for initialization
	void Awake () {
        // retrieve the box component from the parent object
		//box = transform.parent.parent.GetComponent<Cardboard_Box_Behaviour>();
        model = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        if(push_button) {
            pushButton();
        }
	}

    public void Highlight() { 
        model.sharedMaterials[0].SetColor("_EmissionColor",Color.gray);       
    }

    public void pushButton() {
        time_push -= Time.deltaTime;
        if(time_push > 0.8f) {
            transform.parent.localPosition = new Vector3(0.01f,-0.58f,-0.378f);
        } else {
            if(time_push > 0.3f) { 
                transform.parent.localPosition = new Vector3(0.01f,-0.25f,-0.378f);
                model.sharedMaterials[0].SetColor("_EmissionColor",new Color(0.2322962f,0.4310344f,0.1711461f));
            } else {
                transform.parent.localPosition = new Vector3(0.01f,-0.58f,-0.378f);
                model.sharedMaterials[0].SetColor("_EmissionColor",Color.black);
            }
        }
        if(!outputCSV) {
          box.OutputToCSV();
          outputCSV = true;
        }
    }

    public void ExitHighlight(){
        model.sharedMaterials[0].SetColor("_EmissionColor",Color.black);
        time_push = 1;
        outputCSV = false;
        transform.parent.localPosition = new Vector3(0.01f,-0.58f,-0.378f);
    }
}
