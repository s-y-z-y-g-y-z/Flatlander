using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * USED
 * BEN SPURR
 * 
 * looks at a target and offsets from that target based on distance, position, and angle
*/

public class CameraController : MonoBehaviour
{
    //PUBLICS
    [Header ("Target")]
    public Transform target;

    [Header ("Modifiers")]
    public Vector3 positionOffset = new Vector3(0f, 0f, -1f);   //position of camera relative to player (should be normalized
    public Vector3 lookOffset;                                  //the direction the camera is looking relative to the target
    public float movementDamp = 7f;                             //speed of movement
    public float lookDamp = .5f;                                //speed of dolly
    public float distance = 2.8f;                               //z distance from the player to the camera
    public float zMax;                                          //max z distance
    public float zMin;                                          //min z distance
    public bool zoomOut;                                        //for cinematic effects
    public float zoomOutDist;
    public bool startZoomed;                                    //start camera on player

    //PRIVATES
    private float zTarget;                  //target z position for dynamic dolly
    private Vector2 curZMinMax;             //vector of zMax and zMins
    private SideScrollController pCtrl;     //gets reference to player controller

    //initializes values
    void Start()
    {
        pCtrl = FindObjectOfType<SideScrollController>();
        curZMinMax = new Vector2(zMin, zMax);

        //cinematic starting camera
        //(good for staging starting shots)
        if (startZoomed)
        {
            transform.position = target.position + positionOffset * (zTarget * distance);
        }
    }

    // Late Uptate called after all for no render artifacts/stuttering
    void LateUpdate()
    {
        //for cinematic zooming triggered by collider or other state
        if (zoomOut)
        {
            curZMinMax = Vector3.one * zoomOutDist;
        }
        else
        {
            curZMinMax = new Vector2(zMin, zMax);
        }

        //offsets for perspectives
        if (!pCtrl.isSideScrolling)
        {
            positionOffset.y = 1.6f;
            positionOffset.z = -.5f;
            positionOffset.x = 0f;
        }
        else
        {
            positionOffset.y = 0f;
            positionOffset.z = -1f;
            positionOffset.x = 0f;
        }

        //dist multiplier based on player speed
        zTarget = Mathf.Lerp(zTarget, Mathf.Clamp((pCtrl.currentVelocity / pCtrl.maxSpeed) * zoomOutDist, curZMinMax.x, curZMinMax.y), Time.deltaTime * lookDamp);

        //move and look
        transform.position = Vector3.Lerp(transform.position, target.position + positionOffset * (zTarget * distance), Time.deltaTime * movementDamp);
        transform.LookAt((target.position + lookOffset));
    }
}
