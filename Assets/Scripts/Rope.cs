using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * UNUSED
 * Zac Lopez
*/

public class Rope : MonoBehaviour {


	public Vector3 destination;
	public float speed = 1;
	public float distance = .5f;
	public GameObject hook_node;
	private GameObject gun;
	private GameObject rope;
	private bool done = false;
	public List<GameObject> Nodes = new List<GameObject> ();
	public int vertices = 2;

	// Use this for initialization
	void Start () {

		gun = GameObject.FindGameObjectWithTag ("barrel");
		// Since we are instantiating nodes,
		// The last node will have this script.
		// Essentially "this" object
		rope = transform.gameObject;
		//GetComponent<CharacterJoint> () - transform.parent.GetComponent<Rigidbody> ();
		Nodes.Add (rope);
	}
	
	// Update is called once per frame
	void Update () {

		transform.position = Vector3.MoveTowards (transform.position, destination, speed);


	}

	void CreateNode()
	{
		

	}

	void OnCollisionEnter(Collision col)
	{
			
	}


}
