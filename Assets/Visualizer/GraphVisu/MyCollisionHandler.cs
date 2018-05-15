using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyCollisionHandler : MonoBehaviour {

	Color startColor = Color.gray;
	Color collisionColor = Color.yellow;

	List<string> colliderNames = new List<string>(){"bone3RI","bone3","Cylinder"};

	public delegate void NodeEntered(int nodeId);
	public static event NodeEntered OnNodeEntered;

	public delegate void NodeEnteredPosition(Vector3 position);
	public static event NodeEnteredPosition OnNodeEnteredPostion;

	public delegate void NodeEditing(int nodeId, Vector3 posIdx);
	public static event  NodeEditing OnNodeEditing;

	public delegate void NodeExited(int nodeId);
	public static event  NodeExited OnNodeExited;

	private int currentSelection = -1;

	Vector3 initScale;

	// Use this for initialization
	void Start () {

		startColor = GetComponent<Renderer>().material.color;
		initScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter (Collision col)
	{
		foreach(string CN in colliderNames)
		{
			if(col.gameObject.name.Equals(CN))
			{
				GetComponent<Renderer>().material.color = collisionColor;
				currentSelection = int.Parse(gameObject.name);

				if(transform.localScale != initScale*2f) 
				transform.localScale*=2f;

				if(OnNodeEntered != null)
				OnNodeEntered(currentSelection);

			}
		}
	}

	void OnCollisionStay(Collision col) {

		foreach(string CN in colliderNames)
		{
			if(col.gameObject.name.Equals(CN))
			{
				if(OnNodeEditing != null)
				OnNodeEditing(currentSelection, col.gameObject.transform.position); // col.collider.transform.position);
			}
		}
	}

	void OnCollisionExit (Collision col)
	{
		//if(!col.gameObject.name.Equals("Cylinder"))
		foreach(string CN in colliderNames)
		{
			if(col.gameObject.name.Equals(CN))
			{
				GetComponent<Renderer>().material.color = startColor;
				transform.localScale = initScale;

				if(OnNodeExited!=null)
				OnNodeExited(int.Parse(gameObject.name));

			}
		}
	}
}
