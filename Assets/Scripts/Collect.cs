using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * JOSH KARMEL
 * USED
 * 
 * ATTACHED TO COLLECTABLES
*/

public class Collect : MonoBehaviour {

    //PUBLIC SCRIPT REFERENCES
    public GM gm;

    private void Start()
    {
        
    }

    private void Update()
    {
        if(tag == "Collectable")
        {
            transform.Rotate(Vector3.up * Time.deltaTime * 60, Space.World);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            gm.HandleScore(1);
            gameObject.SetActive(false);
        }
    }
}
