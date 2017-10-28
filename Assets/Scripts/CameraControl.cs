using UnityEngine;
using System.Collections;

//Felipe Lai
/*
 * UNUSED
 * MODIFIED INTO CameraController.cs
 */

public class CameraControl : MonoBehaviour
{

    public bool topDown;
    public Vector3 topTarget, sideTarget;
    public Transform player;
    public float speed;

    // Use this for initialization
    void Start()
    {
        topDown = false;
    }

    // Update is called once per frame
    void Update()
    {
        //----Camera Rotation----
        Vector3 playerDir = player.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = rot;


        float step = speed * Time.deltaTime;

        //---Camera Movement----
        if (Input.GetKeyDown("space"))
            topDown = !topDown;

        //Camera Shifting
        if (topDown)
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, topTarget, step);

        if (!topDown)
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, sideTarget, step);
    }
}