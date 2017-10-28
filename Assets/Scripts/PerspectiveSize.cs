using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * USED
 * FELIPE LAI
 * 
 * scales the objects
*/

/*public class PerspectiveSize : MonoBehaviour {

    //checks the camera persp.
    public SideScrollController pCtrl; 

    //the value that the objects are scaled by
    float posDiff;                           //difference between startPos and currentPos
    public float growRatio, shrinkRatio;
    public float scaleSpeed;    //the ratio it physically grows by; the speed of growth
    Vector3 Vscaler;                        //the V3 which contains updated and new scale of object
    public float maxScale, minScale;        //Max and Min scale values of the object. Set to 0 for no max or min

    //the scaled object values
    Rigidbody rigidBody;
    Vector3 startPos;

    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        startPos = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        //actual speed the object scales
        float step = scaleSpeed * Time.deltaTime;

        //checks how far the object is from startPos
        posDiff = startPos.z - transform.position.z;

        //checks whether posDiff is shrinking or growing and then grows or shrinks at their respective rates
        if (posDiff < 0)
        {
            posDiff = Mathf.Pow(2, posDiff / shrinkRatio);
        }
        else
        {
            posDiff = Mathf.Pow(2, (posDiff / growRatio));
        }

        //checks if scale is beyond min and max values and if so sets them to max and min
        if (posDiff < minScale && minScale != 0)
        {
            posDiff = minScale;
        }
        else if (posDiff > maxScale && maxScale != 0)
        {
            posDiff = maxScale;
        }

        //gets the new values and puts them into a V3
        Vscaler = new Vector3(posDiff, posDiff, posDiff);

        //checks the persp.
        if (pCtrl.isSideScrolling)
        {
            //if sidescrolling, scale it to Vscaler
           // transform.localScale = Vector3.MoveTowards(transform.localScale, Vscaler, step);
            rigidBody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY;
   
        }
        else if (!pCtrl.isSideScrolling)
        {
            //if top-down, return to original scale
            transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(1, 1, 1), step);
            rigidBody.constraints = RigidbodyConstraints.None;
        }
    }
}*/
public class PerspectiveSize : MonoBehaviour
{
    public float normalizedRatio;
    public float maxDistance;
    public Vector2 minMaxScales;
    public bool scalePositive;
    float dist;
    public float curScale;
    public float targetScale;
    public float scaleSpeed;

    fInput fInput;
    
    public Transform scaleTransform;


    private void Start()
    {
        fInput = FindObjectOfType<fInput>();
    }

    private void Update()
    {
        dist = Vector3.Distance(transform.position, scaleTransform.position);
        normalizedRatio = dist/maxDistance;

        if (scalePositive)
        {
            targetScale = minMaxScales.y * normalizedRatio;
        }
        else
        {
            targetScale = minMaxScales.x * normalizedRatio;
        }
        targetScale = Mathf.Clamp(targetScale, minMaxScales.x, minMaxScales.y);

        curScale = Mathf.Lerp(curScale, targetScale, Time.deltaTime * scaleSpeed);

        transform.localScale = Vector3.one * targetScale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(scaleTransform.position, maxDistance);
    }
}

