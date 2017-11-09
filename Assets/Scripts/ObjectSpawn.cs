using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawn : MonoBehaviour {
    public Transform obj;
    public float spawnRate;
    public int max;

	// Use this for initialization
	void Start () {
        StartCoroutine(Stuff());
	}
	
    IEnumerator Stuff()
    {
        for (int i = 0; i <= max; i++)
        {
            yield return new WaitForSeconds(spawnRate);
            Instantiate(obj, transform.position, transform.rotation);
        }
    }
}
