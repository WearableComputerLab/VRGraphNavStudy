using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Range_Label : MonoBehaviour {

    public Transform leverA, leverB;
    public Vector3 targetVector;
    public float distA, distB;
    public int A, B;
    public int multiplyer;
    private Text label;
	// Use this for initialization
	void Start () {
        label = GetComponent<Text>();	
	}
	
	// Update is called once per frame
	void Update () {
        distA = Vector3.Distance(leverA.localPosition, targetVector);
        distB = Vector3.Distance(leverB.localPosition, targetVector);
        A = Mathf.CeilToInt(distA*100);
        B = Mathf.CeilToInt(distB*100);
        if (A != B) {
            label.text = "" + A + " - " + B;
        } else {
            label.text = "" + A;
        }
    }
}
