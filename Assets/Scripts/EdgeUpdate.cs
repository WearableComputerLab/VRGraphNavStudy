using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public class EdgeUpdate : MonoBehaviour {
    public Transform a;
    public Transform b;
    LineRenderer renderEdge;
    public Material lineMat;
    public float c_r, c_g, c_b, c_a;
    public EdgeUpdate e;
    int index = 0;
    private bool hidden = false;
    private bool color_set = false;

    // Use this for initialization
    void Start () {
        lineMat = Resources.Load("unlit") as Material;
        c_a = 0.2f;
		DrawDefault();
    }

    // Connect all of the `points` to the `mainPoint`
    void DrawConnectingLines() {
        Vector3 mainPointPos = a.transform.position;
        Vector3 pointPos = b.transform.position;
        /// TODO: Optimise
        /*


        */

        GL.Begin(GL.LINES);
        lineMat.SetPass(0);
        GL.Color(new Color(c_r, c_g, c_b, c_a));
        GL.Vertex3(mainPointPos.x, mainPointPos.y, mainPointPos.z);
        GL.Vertex3(pointPos.x, pointPos.y, pointPos.z);
        GL.End();
    }

    void OnPreRender() {
        //DrawDefault();
    }

    // To show the lines in the game window whne it is running
	/*
    void OnPostRender() {
        if (lineMat != null && !hidden) {
            DrawConnectingLines();
            if(color_set == false && a != null && b != null)
            {
                setCircleColor();
                color_set = true;
            }
        }
    }
	*/

	void DrawDefault(){
		if (lineMat != null && !hidden) {
			DrawConnectingLines();
			if(color_set == false && a != null && b != null)
			{
				setCircleColor();
				color_set = true;
			}
		}
	}
	
    public void setColor() {
        StackTrace stackTrace = new StackTrace();
        MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();
        string typeName = methodBase.DeclaringType.Name;
        string methodName = methodBase.Name;
        print(typeName +" "+methodName);

        Color c = a.parent.GetComponent<MeshRenderer>().material.color;
        c_r = c.r;
        c_g = c.g;
        c_b = c.b;
        VisualNode v1 = a.GetComponent<VisualNode>();
        v1.addAllocatedEdge(index);
        VisualNode v2 = b.GetComponent<VisualNode>();
        v2.addAllocatedEdge(index);

    }

    public void Highlight() {
        c_a = 1f;
    }

    public void ExitHighlight() {
        c_a = 0.2f;
    }

    public void Hide() {
        c_a = 0;
        hidden = true;
    }

    public void UnHide() {
        c_a = 0.2f;
        hidden = false;
    }

    public void setNodePositions(Transform a1, Transform b2) {
        a = a1;
        b = b2;
    }

    public bool isHidden() {
        return hidden;
    }

    public void setMaterial(Material mat, Color col) {
        lineMat = mat;
        
    }

    public void setColor(Color color) {
        StackTrace stackTrace = new StackTrace();
        MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();
        string typeName = methodBase.DeclaringType.Name;
        string methodName = methodBase.Name;
        print(typeName +" "+methodName);
        c_r = color.r;
        c_g = color.g;
        c_b = color.b;
        c_a = color.a;
    }

    public void setCircleColor() {
        Color c = a.parent.parent.GetComponent<MeshRenderer>().material.color;
        c_r = c.r;
        c_g = c.g;
        c_b = c.b;
        VisualNode v1 = a.GetComponent<VisualNode>();
        v1.addAllocatedEdge(index);
        VisualNode v2 = b.GetComponent<VisualNode>();
        v2.addAllocatedEdge(index);
    }

    public void setE(int index) {
        this.index = index;
    }
}
