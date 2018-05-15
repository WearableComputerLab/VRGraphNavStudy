using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Label_Switcher : MonoBehaviour {

    public string[] values;
    public Lever_Behaviour lever;
    private Text textLabel;
    private int index;
    private string currentValue;
    float t = 0.3f;

	// Use this for initialization
	void Start () {
        textLabel = GetComponent<Text>();
        index = values.Length / 2;
    }
	
	// Update is called once per frame
	void LateUpdate () {

        textLabel.text = values[index];

		if(lever.LeverGetAxis() >= 1 && index < values.Length-1) {
            if (t == 0.3f) {
                index++;
            }
        }
        if (lever.LeverGetAxis() <= -1 && index > 0) {
            if (t == 0.3f) {
                index--;
            }
        }

        if(t > 0) {
            t -= Time.deltaTime;
        } else {
            t = 0.3f;
        }

    }

    public int returnIndex() {
        return index;
    }

    public override string ToString() {
        return values[index];
    }
}
