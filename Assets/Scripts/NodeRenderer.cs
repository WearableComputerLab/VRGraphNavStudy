using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeRenderer : MonoBehaviour {
    ArrayList nodes = new ArrayList();
    public Material m;
    public Mesh mesh;
    public Vector3[] vec;
    public bool[] normals;
	// Use this for initialization
	void Start () {
		
	}

    // Connect all of the `points` to the `mainPoint`
    void DrawNodes() {
            
            //Vector3 mainPointPos = g.transform.position;
            GL.PushMatrix();
            m.SetPass(0);
        //GL..LoadIdentity();

        GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, false);

        GL.Begin(GL.QUADS);
            GL.MultMatrix(transform.localToWorldMatrix);
            int i = 0;
        // Top face
        GL.Color(new Color(0.0f, 1.0f, 0.0f));  // Green
        GL.Vertex3(1.0f, 1.0f, -1.0f);  // Top-right of top face
        GL.Vertex3(-1.0f, 1.0f, -1.0f);  // Top-left of top face
        GL.Vertex3(-1.0f, 1.0f, 1.0f);  // Bottom-left of top face
        GL.Vertex3(1.0f, 1.0f, 1.0f);  // Bottom-right of top face

        // Bottom face
        GL.Color(new Color(1.0f, 0.5f, 0.0f)); // Orange
        GL.Vertex3(1.0f, -1.0f, -1.0f); // Top-right of bottom face
        GL.Vertex3(-1.0f, -1.0f, -1.0f); // Top-left of bottom face
        GL.Vertex3(-1.0f, -1.0f, 1.0f); // Bottom-left of bottom face
        GL.Vertex3(1.0f, -1.0f, 1.0f); // Bottom-right of bottom face

        // Front face
        GL.Color(new Color(1.0f, 0.0f, 0.0f));  // Red
        GL.Vertex3(1.0f, 1.0f, 1.0f);  // Top-Right of front face
        GL.Vertex3(-1.0f, 1.0f, 1.0f);  // Top-left of front face
        GL.Vertex3(-1.0f, -1.0f, 1.0f);  // Bottom-left of front face
        GL.Vertex3(1.0f, -1.0f, 1.0f);  // Bottom-right of front face

        // Back face
        GL.Color(new Color(1.0f, 1.0f, 0.0f)); // Yellow
        GL.Vertex3(1.0f, -1.0f, -1.0f); // Bottom-Left of back face
        GL.Vertex3(-1.0f, -1.0f, -1.0f); // Bottom-Right of back face
        GL.Vertex3(-1.0f, 1.0f, -1.0f); // Top-Right of back face
        GL.Vertex3(1.0f, 1.0f, -1.0f); // Top-Left of back face

        // Left face
        GL.Color(new Color(0.0f, 0.0f, 1.0f));  // Blue
        GL.Vertex3(-1.0f, 1.0f, 1.0f);  // Top-Right of left face
        GL.Vertex3(-1.0f, 1.0f, -1.0f);  // Top-Left of left face
        GL.Vertex3(-1.0f, -1.0f, -1.0f);  // Bottom-Left of left face
        GL.Vertex3(-1.0f, -1.0f, 1.0f);  // Bottom-Right of left face

        // Right face
        GL.Color(new Color(1.0f, 0.0f, 1.0f));  // Violet
        GL.Vertex3(1.0f, 1.0f, 1.0f);  // Top-Right of left face
        GL.Vertex3(1.0f, 1.0f, -1.0f);  // Top-Left of left face
        GL.Vertex3(1.0f, -1.0f, -1.0f);  // Bottom-Left of left face
        GL.Vertex3(1.0f, -1.0f, 1.0f);  // Bottom-Right of left face
        GL.End();
            GL.PopMatrix();
    }

    // To show the lines in the game window whne it is running

    void OnPostRender() {
        if (nodes.Count > 0) {
            DrawNodes();
        }
    }
    void OnDrawGizmos() {
        DrawNodes();
    }

}
