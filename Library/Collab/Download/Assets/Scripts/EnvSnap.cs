using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * UNUSED 
 * ZAC LOPEZ
 * snaps all environment children to an equal y-value of 0 on perspective shift
 * 
 * ATTACHED to Env parent
 */

public class EnvSnap : MonoBehaviour {

    //PUBLICS
    public bool snap;
	public List<Transform> ogTrans;
	public List<float> tempTrans;
    public Transform child;

    //PRIVATES
    private fInput inputCtrl;
	private EnvAttributes env;

    // Use this for initialization
    void Start () 
	{
        inputCtrl = FindObjectOfType<fInput>();
		env = FindObjectOfType<EnvAttributes> ();
		ogTrans = env.ogTrans;
		tempTrans = env.ogYTrans;
    }
		
    // Update is called once per frame
    void Update()
    {
        snap = inputCtrl.snap;
        Debug.Log(snap);
        HandleSnap();
    }

    void HandleSnap()
    {
        for (int i = 0; i < ogTrans.Count; i++)
        {
            child = ogTrans[i];
			if (snap == true) 
			{
				child.localScale = new Vector3 (child.localScale.x, 0.9f, child.localScale.z);
				child.SetPositionAndRotation (new Vector3 (child.position.x, 0, child.position.z), Quaternion.identity);
			} 
			else if (snap == false) 
			{
				child.SetPositionAndRotation(new Vector3(child.position.x, tempTrans[i], child.position.z), Quaternion.identity);
			}

        }
    }
}
