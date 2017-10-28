using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * UNUSED
 * ZAC LOPEZ
 * 
 * currently attached to environment manager, stores the objects in the environment 
 */

public class EnvAttributes : MonoBehaviour {

    public List<Transform> ogTrans;
	public List<float> ogYTrans;
    public GameObject[] levelObjects;
    // Use this for initialization


    void Start ()
    {
        levelObjects = GameObject.FindGameObjectsWithTag("Environment");
		setTrans ();
		setY ();
    }

	void setTrans()
	{
		for (int i = 0; i < levelObjects.Length; i++)
		{
			ogTrans.Add(levelObjects[i].transform);
		}
	}

	void setY()
	{
		for (int i = 0; i < ogTrans.Count; i++) 
		{
			ogYTrans.Add (ogTrans [i].position.y);
		}
	}
}
