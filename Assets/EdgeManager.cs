using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeManager : MonoBehaviour {

	public int edgeCount = 0;
	private ArrayList edges = new ArrayList();
	private Material lineMat;

	// Use this for initialization
	void Start () {
		lineMat = Resources.Load("unlit") as Material;
	}
		
	public void addEdges(Edge e) {
		edges.Add(e);
		edgeCount++;
	}

	void OnPostRender() {
	 	DrawConnectingLines();
	}

	void DrawConnectingLines() {
		GL.Begin(GL.LINES);
		foreach (Edge e in edges) {
			lineMat.SetPass (0);
			Color c = e.getColor ();
			GL.Color (new Color (c.r, c.g, c.b, 0.2f));
			GL.Vertex3 (e.getA().x, e.getA().y, e.getA().z);
			GL.Vertex3 (e.getB().x, e.getB().y, e.getB().z);
		}
		GL.End();

	}

	public ArrayList getEdges(){
		return edges;
	}
}
