﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * USED
 * ================================
 * Zac Lopez
 * 
 * Rock_fall can be attached to any game object.
 * After the object detects a collision with the players hook, it waits two seconds
 * and then falls by setting kinematic on objects rb(rigidbody) to be true.
 * Seconds is a public variable that can be changed in the game scene
 */ 
public class Rock_Fall : MonoBehaviour {

    public GameObject rock;
    public float seconds = 2.0f;

    private Rigidbody rb;
    private float time;
	// Use this for initialization
	void Start () {
		rb = rock.GetComponent<Rigidbody>();
	}

    //updates time
	void Update()
	{
		time = Time.deltaTime;
	}

    // checks for collision with hook/grapple
	void OnCollisionEnter(Collision col)
	{
        Vector3 velocity = col.relativeVelocity;
        if (col.gameObject.CompareTag("Hook")) {
			StartCoroutine (Fall());
		}
	} 

    // sets kinematic to be true after alloted seconds
	IEnumerator Fall()
	{
		yield return new WaitForSeconds (seconds);
		rb.isKinematic = false;
	}


}
