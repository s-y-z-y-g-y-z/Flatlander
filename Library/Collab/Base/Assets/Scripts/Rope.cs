using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour {

	public Vector3 destination;
	public float speed = 1;
	public float distance = 2;
	public GameObject hook_node;
	private GameObject gun;
	private GameObject lastNode;
	private bool done = false;
	public List<GameObject> Nodes = new List<GameObject> ();
	public int vertices = 2;
	public LineRenderer lr;
	// Use this for initialization
	void Start () {
		gun = GameObject.FindGameObjectWithTag ("barrel");
		// Since we are instantiating nodes,
		// The last node will have this script.
		// Essentially "this" object
		lastNode = transform.gameObject;

		lr = GetComponent<LineRenderer>();

		Nodes.Add (lastNode);
	}
	
	// Update is called once per frame
	void Update () {

		transform.position = Vector3.MoveTowards (transform.position, destination, speed);

		if (transform.position != destination) {

			if (Vector3.Distance (gun.transform.position, lastNode.transform.position) > distance) {
				CreateNode ();
			}

		} else if (!done) {
			done = true;

			while (Vector3.Distance (gun.transform.position, lastNode.transform.position) > distance) {
				CreateNode ();
			}

			lastNode.GetComponent<HingeJoint> ().connectedBody = gun.GetComponent<Rigidbody> ();
		}

		LineRender ();
	}

	void LineRender() {
		lr.positionCount = vertices;
		int i;
		for (i = 0; i < Nodes.Count; i++) {
			lr.SetPosition (i, Nodes [i].transform.position);
		}

		lr.SetPosition (i, gun.transform.position);
	}

	void CreateNode() {
		Vector3 position = gun.transform.position - lastNode.transform.position;
		position.Normalize ();
		position *= distance;
		position += lastNode.transform.position;
		// creating a variable to store node, change to array later?
		GameObject node = (GameObject)Instantiate (hook_node, position, Quaternion.identity);
		node.transform.SetParent (transform);
		lastNode.GetComponent<HingeJoint> ().connectedBody = node.GetComponent<Rigidbody> ();
		lastNode = node;
		Nodes.Add (lastNode);
		vertices++;
	}

	void CreatOneNode() {
		

		Vector3 position = gun.transform.position - lastNode.transform.position;
		position.Normalize ();
		position = position / 2;
		Instantiate (hook_node, position, Quaternion.identity);

	}
}
