using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSight : MonoBehaviour
{

    LineRenderer line;
    GrappleController grapple;
    float range;
    fInput inputCtrl;

    // Use this for initialization
    void Start()
    {
        line = GetComponent<LineRenderer>();
        grapple = FindObjectOfType<GrappleController>();
        inputCtrl = FindObjectOfType<fInput>();

    }

    // Update is called once per frame
    void Update()
    {
        range = Vector3.Distance(inputCtrl.lookPos, grapple.barrel.transform.position);
        range = Mathf.Clamp(range, 1f, grapple.range);
        line.positionCount = 2;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.forward * range + transform.position);
    }
}
