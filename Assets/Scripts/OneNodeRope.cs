using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneNodeRope : MonoBehaviour {

	public Vector3 destination;
	public float speed = 1;
	private GameObject gun;
	private GameObject thisNode;
	public GameObject hook_node;

	// Use this for initialization
	void Start () {
		gun = GameObject.FindGameObjectWithTag ("barrel");
		thisNode = transform.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector3.MoveTowards (transform.position, destination, speed);
		if (transform.position != destination) 
		{
			CreateNode ();
		}

	}

	void CreateNode() {
		Vector3 position = gun.transform.position - thisNode.transform.position;
		position.Normalize ();
		//position = thisNode;

		Instantiate(hook_node, position, Quaternion.identity);
		//thisNode.GetComponent<HingeJoint> ().connectedBody = thisNode.GetComponent<Rigidbody> ();
	}
}
