using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * UNUSED
 * Zac Lopez
*/

/*
 * UNUSED
 * Zac Lopez
*/

public class Launch_hook : MonoBehaviour {

	public GameObject hook;
	GameObject curHook;
    SideScrollController pCtrl;
	private int ropes = 0;
    // Use this for initialization
    void Start()
    {
        pCtrl = FindObjectOfType<SideScrollController>();
    }
	// Update is called once per frame
	void Update () {

		// get hook point
		// need to check if object the hook gits is hookable 
		// collision check
		if (Input.GetMouseButtonDown (0)) {
            Vector3 dest = pCtrl.lookPos;

			if (ropes == 0) {
				curHook = (GameObject)Instantiate (hook, transform.position, Quaternion.identity);
				//curHook.GetComponent<Rigidbody> ().tra
				curHook.GetComponent<Rope> ().destination = dest;
				ropes++;

			} else {

				Destroy (curHook);
				ropes--;;

			}
		}


	}
}
