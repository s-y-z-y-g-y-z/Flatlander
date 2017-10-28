using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * USED
 * BEN SPURR
 * 
 * shoots the hook projectile and controls the joints
 * 
 * NEEDS REVISION TO WORK WITH PHYSICS SO THAT THE JOINT IS ON THE PLAYER NOT THE BARREL TRANSFORM
*/

/*public class GrappleController : MonoBehaviour {

    //DEPENDENCIES
    [Header ("Projectile Prefab")]
    public GameObject HookPrefab;

    [Header ("Projectile Attributes")]
    public float range;
    public float power;
    public int ropeSmoothness=7;
    public float ropeDroop= -0.1f;
    
    //INTERNALS
    private SideScrollController pCtrl;
    private LineRenderer line;
    private fInput inputCtrl;
    private GameObject curHook;
    private Rigidbody curHookRb;
    private SpringJoint joint;
    private Rigidbody curAnchorRb;
    private Vector3 lineMid;
    private Vector3 lineEnd;

    [Header ("Conditionals")]
    public bool hookIsSet;

	// Use this for initialization
	void Start ()
    {
        line = gameObject.GetComponent<LineRenderer>();
        inputCtrl = FindObjectOfType<fInput>();
        joint = gameObject.GetComponent<SpringJoint>();
        pCtrl = FindObjectOfType<SideScrollController>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        //checks if shooting
		if(inputCtrl.isShooting)
        {
            Shoot();
        }

        HandleLine();
    }

    //handles line renderer
    public void HandleLine()
    {
        //line.positionCount = 3;

        //line.SetPosition(0, transform.position);

        //float centerHeight = Mathf.Clamp(range - Vector3.Distance(transform.position, lineEnd),0f,.4f);
        //float yPos = Mathf.Lerp(transform.position.y, lineEnd.y, 0.5f) - (centerHeight * (Mathf.Lerp(transform.position.y, lineEnd.y, 0.5f) - pCtrl.transform.position.y));
        //Vector3 centerPos = new Vector3(Mathf.Lerp(transform.position.x, lineEnd.x, 0.5f), yPos, Mathf.Lerp(transform.position.z, lineEnd.z, 0.5f));
        //line.SetPosition(1, centerPos);
        //line.SetPosition(2, lineEnd);
        ///////////////////////

        //checks current end position for line
        if (curHook == null)
        {
            lineEnd = transform.position+(transform.forward*.2f);
        }
        else if (hookIsSet)
        {
            if (curAnchorRb != null)
            {
                lineEnd = curAnchorRb.transform.TransformPoint(joint.connectedAnchor);
            }
            else
            {
                lineEnd = curHookRb.transform.position;
            }
        }
        else
        {
            lineEnd = curHook.transform.position;
        }

        line.positionCount = ropeSmoothness;

        for (int i = 0; i < line.positionCount; i++)
        {
            if(i==0)//start position
            {
                line.SetPosition(i, transform.position);
            }
            else if (i == line.positionCount - 1)//end position
            {
                line.SetPosition(i, lineEnd);
            }
            else//intermediate nodes
            {
                Vector3 iStart = line.GetPosition(i - 1);
                Vector3 iEnd = line.GetPosition(i + 1);
                float distRatio= Mathf.Clamp01(range - Vector3.Distance(transform.position, lineEnd));//normalizes based on dist
                Vector3 iMid = Vector3.Lerp(iStart, iEnd, 0.5f) + new Vector3(0f, (ropeDroop*distRatio)* Mathf.Sqrt(Vector3.Distance(transform.position, Vector3.Lerp(iStart, iEnd, 0.5f))), 0f);//parabolas! highschool did pay off!
                line.SetPosition(i, iMid);
            }
        }
    }

    //handles reloading and shooting
    public void Shoot()
    {
        //checks if current hook exists, destroys if so and instantiates new one
        //"Click to shoot, click to remove, click to shoot again"
        if(curHook==null)
        {
            curHook = Instantiate(HookPrefab, transform.position, transform.rotation);
            curHookRb = curHook.GetComponent<Rigidbody>();
            curHookRb.AddForce(transform.forward * power, ForceMode.Impulse);
            joint.connectedBody = curHookRb;
            joint.connectedAnchor = Vector3.zero;
            joint.maxDistance = range;
        }
        else
        {
            curAnchorRb = null;
            joint.connectedBody = null;
            Destroy(curHook);
            curHook = null;
        }
    }

    //"The Hooker Function"
    //handles the actual hooking mechanic of the hook hooking
    //takes in the hit rigidbody as the anchor point
    //called from HookController.cs
    public void SetHook(Rigidbody anchor)
    {
        //checks to see whether the anchor is dynamic or static for pulling/swinging
        if(anchor==null)
        {
            //BETTER SWING MECHANIC TO BE IMPLEMENTED
            curHookRb.isKinematic = true;
        }
        else
        {
            //basic dragging mechanics
            hookIsSet = true;
            curAnchorRb = anchor;
            joint.connectedBody = anchor;
            Vector3 target= curHook.transform.position - anchor.transform.position; ;
            joint.connectedAnchor = target;

            curHook.SetActive(false);
        }
    }

    
}*/
public class GrappleController : MonoBehaviour
{

    //DEPENDENCIES
    [Header("Dependencies")]
    public GameObject HookPrefab;
    public GameObject HookStaticModel;
    public GameObject barrel;
    public GameObject gunshotParticlePrefab;

    [Header("Modifiers")]
    public float range;
    public float power;
    public float recoilForce=0.3f;
    public float massInfluence = 1f;
    [Header("Rope Attributes")]
    public int ropeSmoothness = 7;
    public float ropeDroop = -1f;

    [Header("Conditionals")]
    public bool hookIsSet;

    //INTERNALS
    private GameObject staticHook;
    private SideScrollController pCtrl;
    private WeponRecoil recoil;
    private LineRenderer line;
    private fInput inputCtrl;
    private GameObject curHook;
    private Rigidbody curHookRb;
    private SpringJoint joint;
    private Rigidbody curAnchorRb;
    private Vector3 lineMid;
    public Vector3 lineEnd;
    private float lineCoef;
    public GameObject curParticle;

    // Use this for initialization
    void Start()
    {
        recoil = FindObjectOfType<WeponRecoil>();
        line = gameObject.GetComponent<LineRenderer>();
        inputCtrl = FindObjectOfType<fInput>();
        joint = gameObject.GetComponent<SpringJoint>();
        pCtrl = FindObjectOfType<SideScrollController>();
        staticHook = Instantiate(HookStaticModel, barrel.transform);
    }

    // Update is called once per frame
    void Update()
    {
        //checks if shooting
        if (inputCtrl.isShooting)
        {
            Shoot();
        }

        HandleLine();
        Mathf.Clamp(massInfluence,0f,3f);
        lineCoef = ropeDroop / 10f;
        joint.anchor = transform.InverseTransformPoint(barrel.transform.position);
    }

    //handles line renderer
    public void HandleLine()
    {
        //checks current end position for line
        if (curHook == null)
        {
            lineEnd = barrel.transform.position + (barrel.transform.forward * .2f);
        }
        else if (hookIsSet)
        {
            if (curAnchorRb != null)
            {
                lineEnd = curAnchorRb.transform.TransformPoint(joint.connectedAnchor);
            }
            else
            {
                lineEnd = curHookRb.transform.position;
            }
        }
        else
        {
            lineEnd = curHook.transform.position;
        }

        line.positionCount = ropeSmoothness;
        //sets position and count of nodes;
        for (int i = 0; i < line.positionCount; i++)
        {
            if (i == 0)//start position
            {
                line.SetPosition(i, barrel.transform.position);
            }
            else if (i == line.positionCount - 1)//end position
            {
                line.SetPosition(i, lineEnd);
            }
            else//intermediate nodes
            {
                Vector3 iStart = line.GetPosition(i - 1);
                Vector3 iEnd = line.GetPosition(i + 1);
                float distRatio = Mathf.Clamp01(range - Vector3.Distance(barrel.transform.position, lineEnd));//normalizes based on dist
                Vector3 iMid = Vector3.Lerp(iStart, iEnd, 0.5f) + new Vector3(0f, (lineCoef * distRatio) * Mathf.Sqrt(Vector3.Distance(barrel.transform.position, Vector3.Lerp(iStart, iEnd, 0.5f))), 0f);//parabolas! highschool did pay off!
                line.SetPosition(i, iMid);
            }
        }
    }

    //handles reloading and shooting
    public void Shoot()
    {
        //checks if current hook exists, destroys if so and instantiates new one
        //"Click to shoot, click to remove, click to shoot again"
        if (curHook == null)
        {
            staticHook.SetActive(false);
            recoil.isShooting = true;
            //pCtrl.playerRb.AddForceAtPosition(-transform.forward * recoilForce, transform.position, ForceMode.VelocityChange);
            pCtrl.playerRb.AddForce(-transform.forward * recoilForce,ForceMode.VelocityChange);
            curHook = Instantiate(HookPrefab, barrel.transform.position, barrel.transform.rotation);
            curHookRb = curHook.GetComponent<Rigidbody>();
            curHookRb.AddForce(barrel.transform.forward * power, ForceMode.Impulse);
            curParticle = Instantiate(gunshotParticlePrefab, barrel.transform.position, barrel.transform.rotation);
            curParticle.transform.parent = barrel.transform;
            Destroy(curParticle, 5f);
            joint.connectedBody = curHookRb;
            joint.connectedAnchor = Vector3.zero;
            joint.maxDistance = range;
            joint.massScale = massInfluence;
        }
        else
        {
            Destroy(curParticle);
            staticHook.SetActive(true);
            pCtrl.isSwinging = false;
            curAnchorRb = null;
            joint.connectedBody = null;
            joint.massScale = 0f;
            Destroy(curHook);
            curHook = null;
        }
    }

    //"The Hooker Function"
    //handles the actual hooking mechanic of the hook hooking
    //takes in the hit rigidbody as the anchor point
    //called from HookController.cs
    public void SetHook(Rigidbody anchor)
    {
        //checks to see whether the anchor is dynamic or static for pulling/swinging
        if (anchor == null)
        {
            pCtrl.isSwinging = true;
            //BETTER SWING MECHANIC TO BE IMPLEMENTED
            curHookRb.isKinematic = true;
        }
        else
        {
            //basic dragging mechanics
            hookIsSet = true;
            curAnchorRb = anchor;
            joint.connectedBody = anchor;
            //Vector3 target = curHook.transform.position - anchor.transform.position;
            Vector3 target = anchor.transform.InverseTransformPoint(curHook.transform.position);
            joint.connectedAnchor = target;

            curHook.SetActive(false);
        }
    }


}
