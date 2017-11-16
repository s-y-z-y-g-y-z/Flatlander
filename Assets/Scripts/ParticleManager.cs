using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * USED
 * JOSHUA KARMEL
 * 
*/

public class ParticleManager : MonoBehaviour {

    //PUBLIC REFERENCES
    public fInput inputCtrl;
    public SideScrollController pCtrl;

    //Particle References
    public GameObject jumpSmoke;

	// Use this for initialization
	void Start () {
        jumpSmoke.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void checkJump()
    {
        if (Input.GetButtonDown("Jump") && pCtrl.isGrounded){
            jumpSmoke.SetActive(true);
        }
        else
        {
            jumpSmoke.SetActive(false);
        }
    }
}
