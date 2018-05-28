using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCheck : MonoBehaviour {
	
	public ClusterBehaviour[] renderers;
	public int count = 0;
	public bool[] visibles;
	
	// Update is called once per frame
	private void Update () {		
		renderers = FindObjectsOfType<ClusterBehaviour>();
		visibles = new bool[renderers.Length];
		int i = 0;
		foreach(ClusterBehaviour r in renderers){
			Renderer render = r.GetComponent<Renderer>();
			if(isVisible(render)){
				visibles[i] = true;
			} else {
				visibles[i] = false;
			}
			i++;
		}
		count = 0;
		foreach(bool b in visibles){
			if(b == true){
				count++;
			}
		}
		//count = 0;
	}

	private bool isVisible(Renderer renderer){
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
		if(GeometryUtility.TestPlanesAABB(planes, renderer.bounds)){
			return true;
		} else {
			return false;
		}
	}
}
