using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public GrappleController gCtrl;
    public WinArea winArea;

    //PUBLIC ATTRIBUTES
    public bool resetLevel;
    public float colScore;
    public float scoreColCount;
    public float totalScore;
    public int healthVal;
    public bool touchHazard;
    public bool isDead;
    public Text clock;
    public GameObject ks;
    public GameObject ws;


    private float timer;
    private float roundedTimer;
    private float startTime;

	// Use this for initialization
	void Start ()
    {
        isDead = false;
        touchHazard = false;
        inputCtrl = FindObjectOfType<fInput>();
        pCtrl = FindObjectOfType<SideScrollController>();
        healthVal = hd.healthVal;
        ks.SetActive(false);
        ws.SetActive(false);
        startTime = 6000;
        timer = startTime;
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
        else if (winArea.win)
        {
            ws.SetActive(true);
            gCtrl.Retract();
            if (Input.GetButtonDown("Jump"))
            {
                resetScene();
            }
        }
        else
        {
            ks.SetActive(false);
            ws.SetActive(false);
            pCtrl.isDead = false;
            updateClock();
        }
    }

    //adds value to score
    public void HandleColScore(float value)
    {
        colScore += value;
    }

    public void HandleBonusColScore()
    {
        scoreColCount++;
    }

    //checks if the player is dead
    public void checkDead()
    {
        if(healthVal <= 0 || inputCtrl.reset || touchHazard) 
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
        scoreColCount = 0;
        totalScore = 0;
        timer = startTime;
        touchHazard = false;
        if (ps.isPaused)
        {
            ps.isPaused = false;
        }
        if (res.hitGround)
        {
            
            res.hitGround = false;
        }

        winArea.win = false;
    }

    public void updateClock()
    {
        timer -= Time.deltaTime * 12f;

        roundedTimer = Mathf.RoundToInt(timer);

        clock.text = roundedTimer.ToString();
    }

    public float calculateScore()
    {
        if (isDead)
        {
            roundedTimer = 0;
        }

        totalScore = roundedTimer + (colScore * 1500) + (scoreColCount * 3000);
        return totalScore;

    }

    public void kill()
    {
        isDead = true;
    }
}
