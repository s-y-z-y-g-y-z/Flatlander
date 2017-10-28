using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScaler : MonoBehaviour
{
    //all objects need frozen transforms (ie. transform.scale = vector3.one)
    public bool useScalingAxis;
    public bool showDebug;
    public Vector2 MinMaxScale = new Vector2(.2f, 3f);
    public Vector3 scalingAxis = new Vector3(0f, 0f, 1f);

    private float curDistance;
    private float scaleSpeedRatio;  // ie. 1 unit of distance uniformy scales the object up by 1
    private float scaleSpeed;
    private Vector3 initPos;
    private Rigidbody objectRb;
    private Vector3 targetScale;


	// Use this for initialization
	void Start ()
    {
        transform.position = initPos;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(useScalingAxis)
        {
            ScaleOnAxis();
        }
        else
        {
            curDistance=Vector3.Distance(transform.position,initPos);
            ScaleWithDistance();
        }
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale*scaleSpeedRatio, Time.deltaTime * scaleSpeed);
    }

    void ScaleOnAxis()
    {
        targetScale = new Vector3(curDistance, curDistance, curDistance);
    }

    void ScaleWithDistance()
    {

    }
}
