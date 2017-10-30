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
    

    //PUBLIC ATTRIBUTES
    public bool resetLevel;
    public float score;
    public int healthVal;

	// Use this for initialization
	void Start ()
    {
        inputCtrl = FindObjectOfType<fInput>();
        pCtrl = FindObjectOfType<SideScrollController>();
        healthVal = hd.healthVal;
	}

    // Update is called once per frame
    void Update()
    {
        healthVal = hd.healthVal;

        if(healthVal <= 0 || inputCtrl.reset)
        {
            handleReset();
        }
    }

    //adds value to score
    public void HandleScore(float value)
    {
        score += value;
    }

    //Resets the level
    public void handleReset()
    {
        res.resetScene();
    }
}
