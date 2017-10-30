using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeponRecoil : MonoBehaviour {

    public bool isShooting;
    public float recoilSpeed=10f;
    public float recoilLength=.3f;
    public Vector3 rotationalRecoil;
    public bool moveToInitPosition;
    public float dist;
    Vector3 targetPos;
    Quaternion targetRotation;
    Transform ShoulderPos;//parent
	
    // Use this for initialization
	void Start ()
    {
        moveToInitPosition = true;
        ShoulderPos = transform.parent;
	}
	
	// Update is called once per frame
	void Update ()
    {
        dist = Vector3.Distance(transform.position, ShoulderPos.position);

        if (isShooting)
        {
            if (dist >= recoilLength - 0.01f)
            {
                moveToInitPosition = true;
                isShooting = false;
            }
            else
            {
                moveToInitPosition = false;
            }
        }

        float speed;
        if (moveToInitPosition)
        {
            targetRotation = Quaternion.Euler(Vector3.zero);
            targetPos = ShoulderPos.position;
            speed = recoilSpeed;
        }
        else
        {
            targetRotation =Quaternion.Euler(rotationalRecoil);
            targetPos = ShoulderPos.position - transform.forward * recoilLength;
            speed = recoilSpeed * 2f;
        }

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * speed);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);
    }
}
