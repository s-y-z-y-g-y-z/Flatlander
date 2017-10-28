using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * USED
 * BEN SPURR
 * 
 * Handles input from Input Manager
 * 
 * ATTACHED TO InputManager
*/

public class fInput : MonoBehaviour {

    //EXTERNAL
    public ParameterScreen param;

    [Header ("Input Axis Values")]
    public float horizontal;
    public float vertical;
    public float horizontalAim;
    public float verticalAim;

    [Header ("Conditionals")]
    public bool isSideScrolling = true;
    public bool snap = false;
    public bool reset;
    public bool isJumping;
    public bool isUsingController;
    public bool isShooting;
    public Vector3 lookPos;

    //INTERNAL
    private float lastRotate;       //store rotation for controller aiming
    private GameObject player;
    private Quaternion aimRotation;
    
    private bool isPaused;

	// Use this for initialization
	void Start ()
    {
        player = GameObject.Find("Player");
        isPaused = param.isPaused;
	}
	
	// Update is called once per frame
	void Update ()
    {
        isPaused = param.isPaused;
        handlePause();
        toggleSnap();
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        //ENABLE WITH CONTROLLER SUPPORT
        //verticalAim = Input.GetAxisRaw("Vertical Aim");
        //horizontalAim = Input.GetAxisRaw("Horizontal Aim");

        if (!isPaused)
        {
            isShooting = Input.GetButtonDown("Fire1");
            isJumping = Input.GetButtonDown("Jump");
        }

		if (Input.GetButtonDown ("Perspective Shift")) {
			isSideScrolling = !isSideScrolling;
		}
        if(Input.GetButtonDown("Reset"))
        {
            reset = true;
        }
        else
        {
            reset = false;
        }


        if (isUsingController)
        {
            HandleControllerAim();
        }
        else
        {
            HandleMouseAim();
        }
    }

    //handles the aim from the mouse
    void HandleMouseAim()  
    {
        //Shoots ray forward for hit point so transform can rotate towards
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 lookP = hit.point;
            lookP.z = 0f;
            lookPos = lookP;
        }
    }

    //NOT FULLY IMPLEMENTED
    //handles the aim from the controller
    void HandleControllerAim()
    {
        //get vector between camera and player 
        Vector3 difference = Camera.main.transform.position - player.transform.position;

        //why negative difference? idk
        float camRotate = Mathf.Atan2(-difference.x, -difference.z);

        float playerRotate = Mathf.Atan2(horizontalAim, verticalAim);

        //combining the two radians
        playerRotate = playerRotate + camRotate;

        float checkRotation = (Mathf.Abs(Mathf.Atan2(horizontalAim, verticalAim)));

        //store last rotation of player so it doesn't reset when there is no joystick input
        if (checkRotation > 0.2f)
        {
            lastRotate = playerRotate;
        }
        else if (checkRotation < 0.01f && verticalAim > 0)
        {
            lastRotate = 0f;
        }
        else
        {
            playerRotate = lastRotate;
        }

        //convert radian to degrees
        Quaternion eulerRotation = Quaternion.Euler(0f, playerRotate * Mathf.Rad2Deg, 0f);

        //plugin degree conversion into transform
        aimRotation = Quaternion.Slerp(aimRotation, eulerRotation, Time.deltaTime * 10);
    }

    //JOSH KARMEL
    //Handles pausing the game
    void handlePause()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            param.handleParamScreen();
        }
    }

    //ZAC LOPEZ
    //Toggles environment snap
    void toggleSnap()
    {
        if (Input.GetButtonDown("Perspective Shift"))
        {
            if (snap == false)
            {
                snap = true;
            }
            else if (snap == true)
            {
                snap = false;
            }
        }
    }
}
