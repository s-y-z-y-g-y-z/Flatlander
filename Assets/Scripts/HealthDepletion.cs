using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDepletion : MonoBehaviour {

    //player has set number of touches he can make to the ground (instead of constant damage)
    //player can collect to fill up ground touches up to an alloted amount
	public int healthVal = 100;
    public int healthChunks = 3; //divisions of health value (ie. hearts)
    public int baseDmg = 1; //number of health chunks to remove upon ground touch
	public int baseHeal = 10; //number of units to fill per pickup/action


    private int maxHealth;
	public Slider healthBar;
    private SideScrollController pCtrl;
    private fInput inputCtrl;
    private bool playerHasLanded;

	// Use this for initialization
	void Start ()
    {
        maxHealth = healthVal;
        pCtrl = FindObjectOfType<SideScrollController>();
        inputCtrl = FindObjectOfType<fInput>();
	}
	
	// Update is called once per frame
	void Update () 
	{
        if(inputCtrl.reset)
        {
            healthVal = maxHealth;
        }
        healthVal = Mathf.Clamp(healthVal, 0, maxHealth);
		healthBar.value = healthVal;
	}

	//algorithm for health depletion
	private void Touching()
	{

	}

	public void RemoveHealth(int dmg, bool removeChunk) //  removeChunk = dmg removes a chunk or percentage of max health from current health
    {                                                   //  !removeChunk = dmg removes individual health units
        if (removeChunk)
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
        if(healChunk)
        {
            healthVal += maxHealth / healthChunks;
        }
        else
        {
            healthVal += heal;
        }
	}
		
    //JK~~
    //resets values
    public void resetHealth()
    {
        healthVal = 100;
    }

	void OnCollisionEnter(Collision col)
	{
		// needs to be changed to check for floor
        if(pCtrl.isGrounded && col.gameObject.tag != "safe")
        {
            RemoveHealth(baseDmg, true);
        }

	}

	void OnCollisionExit(Collision col)
	{
		playerHasLanded = false;
	}
}
