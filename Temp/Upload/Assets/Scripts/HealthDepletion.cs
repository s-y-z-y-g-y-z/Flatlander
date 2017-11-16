﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDepletion : MonoBehaviour {

    //PUBLIC ATTRIBUTES
    //player has set number of touches he can make to the ground (instead of constant damage)
    //player can collect to fill up ground touches up to an alloted amount
	public int healthVal;       //current player health
    public int healthChunks;    //divisions of health value (ie. hearts)
    public int baseDmg;         //number of health chunks to remove upon ground touch
	public int baseHeal;        //number of units to fill per pickup/action
    public Slider healthBar;

    //PRIVATE ATTRIBUTES
    private int maxHealth;
    private SideScrollController pCtrl;
    private fInput inputCtrl;
    private bool playerHasLanded;

	// Use this for initialization
	void Start ()
    {
        healthVal = 100;
        healthChunks = 10;
        baseDmg = 1;
        baseHeal = 10;
        maxHealth = healthVal;
        pCtrl = FindObjectOfType<SideScrollController>();
        inputCtrl = FindObjectOfType<fInput>();
	}
	
	// Update is called once per frame
	void Update () 
	{
        healthVal = Mathf.Clamp(healthVal, 0, maxHealth);
		healthBar.value = healthVal;

        //keeps 
        if(healthVal > maxHealth)
        {
            healthVal = maxHealth;
        }
	}

	//algorithm for health depletion
	private void Touching()
	{

	}

/* THESE FUNCTIONS HAVE BEEN REPLACED BY handleHealth()
	public void RemoveHealth(int dmg, bool removeChunk)
    { 
        //  removeChunk = dmg removes a chunk or percentage of max health from current health
        //  !removeChunk = dmg removes individual health units
        if (removeChunk && healthVal > 0)
        {
            healthVal -= maxHealth / healthChunks;
        }
        else
        {
            //bleed
            healthVal -= dmg;
        }
	}

    public void AddHealth(int heal, bool healChunk)
    {
        if (healChunk && healthVal < 100)
        {
            healthVal += maxHealth / healthChunks;
        }
        else
        {
            healthVal += heal;
        }
    }
*/

    //JK~~
    //handles all health, removing or adding
    //healthMod: + for healing, - for damage. If useChunks, healthmod = # of chunks
    public void handleHealth(int healthMod, bool useChunks)
    {
        if (useChunks && healthVal < maxHealth)
        {
            healthVal += (maxHealth / healthChunks) * healthMod;
        }
        else
        {
            healthVal += healthMod;
        }
    }
		
    //JK~~
    //resets values
    public void resetHealth()
    {
        healthVal = maxHealth;
    }

	void OnCollisionEnter(Collision col)
	{
		// needs to be changed to check for floor
        if(pCtrl.isGrounded && col.gameObject.tag != "safe")
        {
            handleHealth(-1, true);
        }

	}

	void OnCollisionExit(Collision col)
	{
		playerHasLanded = false;
	}
}
