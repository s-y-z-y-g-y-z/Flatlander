using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * USED
 * JOSH KARMEL
 * 
 * ATTACHED TO THE DEATHSCREEN
*/

public class DeathScreen : MonoBehaviour {

    //PUBLIC SCRIPT REFERENCES
    public Reset res;
    public GM gm;

    //public buttons
    public Button reset;
    public Button menu;

	// Use this for initialization
	void Start () {
        reset.onClick.AddListener(buttonReset);
        menu.onClick.AddListener(buttonMenu);
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void buttonReset()
    {
        gm.resetScene();
        gm.isDead = false;

    }

    public void buttonMenu()
    {

    }
}
