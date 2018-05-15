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

	private Transform primitiveTest;

	private void Awake(){
		//7G/ameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		//g.transform.localScale = new Vector3 (0.1f, 0.1f, 0.1f);
		//primitiveTest = g.transform;
	}

    // Update is called once per frame
    void Update () {

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
            if (power.ActiveQuery) {
                float y_rot = graph.transform.rotation.eulerAngles.y;
				Vector3 dir = transform.position - wim.returnWIMWorldOrigin ();
				Camera.main.transform.parent.position = (dir)*15;
				//primitiveTest.position = (dir)*15;
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
