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
    public GameObject jumpPEs;
    public GameObject landingPEs;
    public Vector3 playerVel;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        handleJumpPEs();

        if (!pCtrl.isGrounded) {
            playerVel = pCtrl.localVelocity;
        }

        //handleLandingPEs();
    }

    //instantiates the jump particle effects
    public void handleJumpPEs()
    {
        //checks if the player jumps from the ground
        if (Input.GetButtonDown("Jump") && pCtrl.isGrounded){
            Transform clone = Instantiate(jumpPEs.transform, pCtrl.transform.position, pCtrl.transform.rotation);
            jumpPEs.SetActive(true);
            
            //clone gameobject is destroyed after .75s
            Destroy(clone.gameObject,0.75f);
        }
    }
    
    
    //Need to figure out how to find velocity when landing
    public void handleLandingPEs()
    {
        //if player falling faster than 5
        if(playerVel.z < -1.0f && pCtrl.isGrounded)
        {
            Transform clone = Instantiate(landingPEs.transform, pCtrl.transform.position, pCtrl.transform.rotation);
            landingPEs.SetActive(true);

            //destroys after .75s
            Destroy(clone.gameObject, 0.75f);
        }
    }
    
}
