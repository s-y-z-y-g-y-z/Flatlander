using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public Button start; 
	public Button lvl;
	public Button quit;

	// Use this for initialization
	void Start () {
		Button btn = start.GetComponent<Button> ();
		btn.onClick.AddListener (GameStart);

		btn = quit.GetComponent<Button> ();
		btn.onClick.AddListener (Quit);

		btn = lvl.GetComponent<Button> ();
		btn.onClick.AddListener (LevelSelect);
	}
	
	// Update is called once per frame
	void Update () {
			
	}

	void GameStart()
	{
		SceneManager.LoadScene ("proto", LoadSceneMode.Single);
	}

	void Quit()
	{

	}

	void LevelSelect()
	{

	}
		
}
