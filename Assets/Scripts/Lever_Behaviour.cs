using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever_Behaviour : MonoBehaviour {

    Vector3 startPos;
    public Dual_Controller_Behaviour controller;
    public bool isVertical, isHorizontial;
    public Vector3 pos1, pos2;
    public Transform overridePos1, overridePos2;
    Vector3 def, highlight;
    private bool movingto1, movingto2;

    void Start() {
        def = transform.localScale;
        highlight = transform.localScale * 1.1f;
        startPos = transform.localPosition;
    }

    void Update() {
        if (overridePos1) {
            pos1 = overridePos1.localPosition;
        }
        if (overridePos2) {
            pos2 = overridePos2.localPosition;
        }
    }

    public void movetoPos1() {
        transform.localScale = highlight;
        if (overridePos1 || overridePos2) {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, pos1, 0.01f);
        } else {
            transform.localPosition = Vector3.Lerp(transform.localPosition, pos1, 0.1f);
        }
        movingto1 = true;
        movingto2 = false;
    }

    public void movetoPos2() {
        transform.localScale = highlight;
        if (overridePos1 || overridePos2) {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, pos2, 0.01f);
        } else {
            transform.localPosition = Vector3.Lerp(transform.localPosition, pos2, 0.1f);
        }
        movingto1 = false;
        movingto2 = true;
    }

    public void movetoStartPos() {
        transform.localScale = def;
        if (overridePos1 == null && overridePos2 == null) {
            transform.localPosition = startPos;
        }
        movingto1 = false;
        movingto2 = false;
    }

    void OnCollisionEnter(Collision other) {
        if (other.transform.tag == "Controller") {
            transform.localScale = highlight;
        }
    }

    void OnCollisionExit(Collision other) {
        if (other.transform.tag == "Controller") {
            transform.localScale = def;
        }
    }

    public int LeverGetAxis() {
        if (movingto1) {
            return -1;
        }
        if (movingto2) {
            return 1;
        }
        return 0;
    }

}
