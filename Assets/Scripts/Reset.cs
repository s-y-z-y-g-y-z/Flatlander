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

	fInput inputCtrl;
	public bool resetLevel;
	public bool harderReset;
	SideScrollController pCtrl;

	// Use this for initialization
	void Start () {
		inputCtrl = FindObjectOfType<fInput>();
		pCtrl = FindObjectOfType<SideScrollController>();
	}
	
	// Update is called once per frame
	void Update () {
		//hardReset ();
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "Player") {
            resetScene();
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
	}*/
    
    //JK~~
    //function to reset the scene
    public void resetScene()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        pCtrl.transform.position = pCtrl.initPlayerPos;
        pCtrl.playerRb.velocity = Vector3.zero;
    }

	void hardReset()
	{
		if (harderReset)
		{
			//Application.LoadLevel (Application.loadedLevel);


		}
	}
}
