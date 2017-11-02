using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock_Fall : MonoBehaviour {

	public GameObject rock;
	private Rigidbody rb;
	private float time;
	// Use this for initialization
	void Start () {
		rb = rock.GetComponent<Rigidbody>();
	}

	void Update()
	{
		time = Time.deltaTime;
	}
	void OnCollisionEnter(Collision col)
	{
		//time = Time.deltaTime;
		//float callTime = time + 3f;
		if (col.gameObject.CompareTag("Hook")) {
			StartCoroutine (Fall());
		}
	} 

	IEnumerator Fall()
	{
		yield return new WaitForSeconds (2.0f);
		rb.isKinematic = false;
	}


}
