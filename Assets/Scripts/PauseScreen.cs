using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour {

    //PUBLIC SCRIPT REFERENCES
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
        gm.ResetScene();
        gameObject.SetActive(false);
    }

    public void buttonMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
