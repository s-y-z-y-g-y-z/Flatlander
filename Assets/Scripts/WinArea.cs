using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * JOSH KARMEL
 * USED
 * 
 * ATTACHED TO THE WIN AREA OBJECT
*/

public class WinArea : MonoBehaviour {

    //PUBLIC REFERENCED SCRIPTS
    public ParameterScreen ps;      //to pause the game
    public GameObject winScreen;    //the win text UI
    public Reset reset;             //resets the scene after win

    [HideInInspector]
    public bool win;

	// Use this for initialization
	void Start () {
        winScreen.SetActive(false);
        win = false;
	}
	
	// Update is called once per frame
	void Update () {
        checkWin();
	}
    
    //checks if the player won to pause the game and activate the win text
    public void checkWin()
    {
        if (win)
        {
            winScreen.SetActive(true);

            if (Input.GetButtonDown("Reset"))
            {
                win = false;
                ps.isPaused = false;
                reset.resetScene();
            }
            if (Input.GetButtonDown("Cancel"))
            {
                Application.Quit();
            }
        }
        else
        {
            winScreen.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            ps.isPaused = true;
            win = true;
        }
    }
}
