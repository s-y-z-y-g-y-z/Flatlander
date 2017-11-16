using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * N/A
 * USED
 * 
 * ATTACHED TO KILLFLOOR
*/

public class Reset : MonoBehaviour {

    //PUBLIC SCRIPT REFERENCES
	public SideScrollController pCtrl;
	public fInput inputCtrl;
    public HealthDepletion hd;
    public GM gm;

    //PUBLIC vals
    public bool resetLevel;
	public bool harderReset;
    public bool hitGround;

	// Use this for initialization
	void Start () {
		inputCtrl = FindObjectOfType<fInput>();
		pCtrl = FindObjectOfType<SideScrollController>();
        hd = FindObjectOfType<HealthDepletion>();
        gm = FindObjectOfType<GM>();
        //hitGround = false;
	}
	
	// Update is called once per frame
	void Update () {
		//hardReset();
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "Player") {
            gm.touchHazard = true;                                               // hitGround = true;
		}
	}

/*
	void startReset()
	{
		if (inputCtrl.reset)
		{
			resetLevel = true;
			pCtrl.transform.position = pCtrl.initPlayerPos;
			pCtrl.playerRb.velocity = Vector3.zero;
		}
		else
		{
			resetLevel = false;
		}
	}
*/
    
/*
    //JK~~ function moved to the GM
    //function to reset the scene
    public void resetScene()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        pCtrl.transform.position = pCtrl.initPlayerPos;
        pCtrl.playerRb.velocity = Vector3.zero;
        hd.healthVal = 100;
    }
*/

	void hardReset()
	{
		if (harderReset)
		{
			//Application.LoadLevel (Application.loadedLevel);
		}
	}
}
