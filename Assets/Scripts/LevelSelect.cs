using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/* USED
 * ===============
 * Zac Lopez
 * 
 * Script attached to LevelSelect canvas and is used for 
 * choosing a level, player can click back button
 * to go back to Main menu
 */

public class LevelSelect : MonoBehaviour {

	public Button level1;
	public Button level2;
    public Button level3;
	public Button back;

	public Canvas menu;
	public Canvas lvl;
	// Use this for initialization
	void Start () {
		Button btn = level1.GetComponent<Button> ();
		btn.onClick.AddListener(Load1);

		btn = level2.GetComponent<Button> ();
		btn.onClick.AddListener (Load2);

        btn = level3.GetComponent<Button>();
        btn.onClick.AddListener(Load3);

        btn = back.GetComponent<Button> ();
		btn.onClick.AddListener (GoBack);
	}
	void Load1()
	{
		SceneManager.LoadScene ("Tutorial");
	}

	// loads test scene
	void Load2()
	{
		SceneManager.LoadScene ("Proto2");
	}

    void Load3()
    {
        SceneManager.LoadScene("World3");
    }

	//go back to main menu screen
	void GoBack()
	{
		menu.enabled = true;
		lvl.enabled = false;
	}
}
