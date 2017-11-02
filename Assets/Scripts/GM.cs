using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * JOSH KARMEL
 * BEN SPURR
 * USED
*/

public class GM : MonoBehaviour
{
    //PUBLIC REFERENCES
    private SideScrollController pCtrl;
    private fInput inputCtrl;
    public ParameterScreen ps;
    public HealthDepletion hd;
    public Reset res;
    public GameObject ks;
    public GrappleController gCtrl;

    //PUBLIC ATTRIBUTES
    public bool resetLevel;
    public float score;
    public int healthVal;
    public bool isDead;

	// Use this for initialization
	void Start ()
    {
        isDead = false;
        inputCtrl = FindObjectOfType<fInput>();
        pCtrl = FindObjectOfType<SideScrollController>();
        healthVal = hd.healthVal;
        ks.SetActive(false);
	}

    // Update is called once per frame
    void Update()
    {
        healthVal = hd.healthVal;
        checkDead();

        if (isDead)
        {
            ks.SetActive(true);
            pCtrl.isDead = true;
            gCtrl.Retract();
            if (Input.GetButtonDown("Jump"))
            {
                resetScene();
            }
        }
        else
        {
            ks.SetActive(false);
            pCtrl.isDead = false;
        }
    }

    //adds value to score
    public void HandleScore(float value)
    {
        score += value;
    }

    //checks if the player is dead
    public void checkDead()
    {
        if(healthVal <= 0 || inputCtrl.reset)
        {
            isDead = true;
        }
        else
        {
            isDead = false;
        }
    }
    
    //function to reset the scene
    public void resetScene()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        pCtrl.transform.position = pCtrl.initPlayerPos;
        pCtrl.playerRb.velocity = Vector3.zero;
        hd.healthVal = 100;
    }
}
