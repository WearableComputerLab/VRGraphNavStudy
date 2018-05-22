using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miniature_Camera_Behaviour : MonoBehaviour {

	public bool Moving = false;
    public float WIMmultiplyer = 10;
    public GraphVisualizer graph;
    public Power_Button_Behaviour power;
    private bool parentedToWIM = false;
    public GameObject WIMBlur;
    public World_Miniature_Graph wim;
    public Transform cone;
    public bool stopStudyNode = false;
	private Quaternion camera_Quaternion;

	private Transform worldCamera;
	private Transform miniature;
	public Transform worldGraphCenter;

	private void Awake(){
		worldCamera = Camera.main.transform;
	}

    // Update is called once per frame
    void Update () {

		// get miniature and assign if it is not null
		if (wim.getMiniature () != null) {
			miniature = wim.getMiniature ();
		}

        if (!Moving) {
			camera_Quaternion = transform.rotation;
            gameObject.layer = 0;
            if (power.ActiveQuery == false) {
               WIMBlur.SetActive(false);
                gameObject.layer = 0;
            }
            else
            {
                gameObject.layer = 5;
            }
        } else {
            gameObject.layer = 5;
			Quaternion lookatCamera = Quaternion.LookRotation (transform.position - Camera.main.transform.position);
			transform.rotation = lookatCamera;
			if (power.ActiveQuery && miniature != null) {
				Vector3 delta = (miniature.transform.position - transform.position);
				delta = transform.InverseTransformVector(delta);
				Vector3 wimForward = miniature.transform.TransformDirection(Vector3.forward);
				wimForward = transform.InverseTransformDirection(wimForward);
				graph.transform.position = worldCamera.position + worldCamera.TransformVector(delta);
				graph.transform.forward = wimForward;
                WIMBlur.SetActive(true);
            } else
            {
                WIMBlur.SetActive(false);

            }
        }
        if(parentedToWIM == false)
        {
            if(transform.parent.childCount == 2)
            {
                transform.parent = transform.parent.GetChild(1);
                parentedToWIM = true;
            }
        }
	}

    public void setMoving(bool b) {
        this.Moving = b;
    }

    public void setCone(Transform t)
    {
        cone = t;
    }
}
