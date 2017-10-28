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

		lr = GetComponent<LineRenderer>();
		gun = GameObject.FindGameObjectWithTag ("barrel");
		// Since we are instantiating nodes,
		// The last node will have this script.
		// Essentially "this" object
		lastNode = transform.gameObject;

		Nodes.Add (lastNode);
	}
	
	// Update is called once per frame
	void Update () {

		transform.position = Vector3.MoveTowards (transform.position, destination, speed);

		if (transform.position == destination) {


			/*if (Vector3.Distance (gun.transform.position, lastNode.transform.position) > distance) {
				//reateNode();
			}*/
				
		}/*else if (!done) {
			done = true;

			while (Vector3.Distance (gun.transform.position, lastNode.transform.position) > distance) {
				CreateNode();
			}

			lastNode.GetComponent<HingeJoint> ().connectedBody = gun.GetComponent<Rigidbody> ();
		}*/

	}

	void OnCollisionEnter(Collision col)
	{
		
	}


}
