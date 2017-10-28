using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDepletion : MonoBehaviour {

	public float healthVal = 100;
	public float baseDmg = 1;
	public float baseHeal = 10;


	private Slider health;
	private bool touchFloor = false;
	// Use this for initialization
	void Start () 
	{
		health = GameObject.FindObjectOfType<Slider>();


	}
	
	// Update is called once per frame
	void Update () 
	{
		health.value = healthVal;	
		if (touchFloor == true) {
			RemoveHealth (1);
		}
	}

	//algorithm for health depletion
	private void Touching()
	{

	}

	void RemoveHealth(float dmg)
	{
		if (dmg.Equals(null) || dmg == null) 
		{
			dmg = baseDmg;
		}

		healthVal = healthVal - dmg;

	}

	void AddHealth(float heal)
	{
		if (heal.Equals (null) || heal == null) 
		{
			heal = baseHeal;
		}

		healthVal = healthVal + heal;
	}
		


	void OnCollisionEnter(Collision col)
	{
		// needs to be changed to check for floor
		touchFloor = true;
	}

	void OnCollisionExit(Collision col)
	{
		touchFloor = false;
	}
}
