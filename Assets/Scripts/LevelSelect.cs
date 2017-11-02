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
	public Button test;
	public Button back;

	public Canvas menu;
	public Canvas lvl;
	// Use this for initialization
	void Start () {
		Button btn = level1.GetComponent<Button> ();
		btn.onClick.AddListener(Load1);

		btn = test.GetComponent<Button> ();
		btn.onClick.AddListener (LoadTest);

		btn = back.GetComponent<Button> ();
		btn.onClick.AddListener (GoBack);
	}

	// loads level 1 aka proto
	void Load1()
	{
		SceneManager.LoadScene ("proto");
	}

	// loads test scene
	void LoadTest()
	{
		SceneManager.LoadScene ("Playtest level");
	}

	//go back to main menu screen
	void GoBack()
	{
		menu.enabled = true;
		lvl.enabled = false;
	}
}
