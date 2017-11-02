using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDepletion : MonoBehaviour {

    //player has set number of touches he can make to the ground (instead of constant damage)
    //player can collect to fill up ground touches up to an alloted amount
	public int healthVal;
    public int healthChunks; //divisions of health value (ie. hearts)
    public int baseDmg; //number of health chunks to remove upon ground touch
	public int baseHeal; //number of units to fill per pickup/action


    private int maxHealth;
	public Slider healthBar;
    private SideScrollController pCtrl;
    private fInput inputCtrl;
    private bool playerHasLanded;

	// Use this for initialization
	void Start ()
    {
        healthVal = 100;
        healthChunks = 10;
        baseDmg = -1;
        baseHeal = 10;
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
    
    //to be merged with addhealth. If health needs to be added, paramater is +, if removed, parameter is -. ~~JK
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

    //takes in a bool for whether or not is healing or hurting.
    //if healing, heals the player. if hurting, it hurts them.
    public void handleHealth(bool isHealing)
    {
        if (isHealing)
        {
            healthVal += maxHealth / healthChunks;
        }
        else
        {
            healthVal -= maxHealth / healthChunks;
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
        else if(col.gameObject.tag == "CollectableBox")
        {
            AddHealth(baseHeal, true);
        }

	}

	void OnCollisionExit(Collision col)
	{
		playerHasLanded = false;
	}
}
