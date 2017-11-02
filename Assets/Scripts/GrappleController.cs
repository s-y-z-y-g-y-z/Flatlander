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
//to work with configurable joint, using angular drive for force
public class GrappleController : MonoBehaviour
{
    //DEPENDENCIES
    [Header("Dependencies")]
    public GameObject HookPrefab;
    public GameObject HookStaticModel;
    public GameObject barrel;
    public GameObject gunshotParticlePrefab;
    public GameObject dashParticlePrefab;
    public ParameterScreen ps;
    public GM gm;

    [Header("Modifiers")]
    public float climbSpeed;
    public float swingPower;
    public float fastClimbDamper;
    public float ropeJumpPower = 15f;
    public float jumpThreshold = 2f; //distance from anchor before player can jump up from rope;
    public float range;
    public float minRange = 0.3f;
    public float curRange;
    public float power;
    public float recoilForce = 0.3f;
    public float massInfluence = 1f;
    [Header("Rope Attributes")]
    public int ropeSmoothness = 7;
    public float ropeDroop = -1f;

    [Header("Conditionals")]
    public bool hookIsSet;

    //INTERNALS
    private ConfigurableJoint joint;
    private GameObject staticHook;
    public SideScrollController pCtrl;
    private WeponRecoil recoil;
    private LineRenderer line;
    private fInput inputCtrl;
    private SoftJointLimit jointLimits;
    [HideInInspector]
    public GameObject curHook;
    private Rigidbody curHookRb;


    private Rigidbody curAnchorRb;
    private Vector3 lineMid;
    public Vector3 lineEnd;
    private float lineCoef;
    public GameObject curParticle;
    public float tightenSpeed;
    public float targetDist;

    // Use this for initialization
    void Start()
    {
        recoil = FindObjectOfType<WeponRecoil>();
        line = gameObject.GetComponent<LineRenderer>();
        inputCtrl = FindObjectOfType<fInput>();
        joint = gameObject.GetComponent<ConfigurableJoint>();
        pCtrl = FindObjectOfType<SideScrollController>();
        staticHook = Instantiate(HookStaticModel, barrel.transform);
        jointLimits = joint.linearLimit;
        Retract();
    }

    // Update is called once per frame
    void Update()
    {

        HandleLine();
        Mathf.Clamp(massInfluence, 0f, 3f);
        lineCoef = ropeDroop / 10f;

        //checks if shooting
        if ((inputCtrl.isShooting || inputCtrl.reset && curHook != null) && (!pCtrl.isDead || ps.isPaused))
        {
            Shoot();
        }

        if (pCtrl.isSwinging)
        {
            HandleLength();
            HandleSwinging(pCtrl.horizontal);
        }
        else
        {
            jointLimits.limit = range;
        }
        joint.linearLimit = jointLimits;
    }

    void HandleLength()
    {
        curRange = Vector3.Distance(transform.TransformPoint(pCtrl.playerRb.centerOfMass), curHook.transform.position);

        if (Input.GetButtonDown("Jump") && !pCtrl.isGrounded)
        {
            Retract();
            if (curRange - minRange < jumpThreshold)
            {
                pCtrl.SpecialJump(ropeJumpPower, Vector3.up, true);
            }
        }
        else if (inputCtrl.vertical > 0f)//climb
        {
            if (inputCtrl.vertical > 0f && Mathf.Abs(pCtrl.playerRb.velocity.x) < 3f && transform.position.y < curHook.transform.position.y)
            {
                targetDist = Mathf.Lerp(targetDist, minRange, Time.deltaTime * climbSpeed);
            }
            else
            {
                targetDist = Mathf.Lerp(targetDist, minRange, Time.deltaTime * climbSpeed * .8f);
            }
        }
        else if (inputCtrl.vertical < 0f)//drop
        {
            targetDist = Mathf.Lerp(targetDist, range, Time.deltaTime * climbSpeed);
        }
        else
        {
            if (curRange < joint.linearLimit.limit)
            {
                targetDist = Mathf.Clamp(Vector3.Distance(pCtrl.transform.position, curHook.transform.position), 1f, range);
            }
        }

        targetDist = Mathf.Clamp(targetDist, .3f, range);

        jointLimits.limit = targetDist;
    }

    void HandleSwinging(float horizontal)
    {
        Vector3 dir = curHook.transform.position - pCtrl.transform.position; 
        Vector3 angle;
        
        angle.x = -dir.y;
        angle.y = dir.x;
        angle.z = 0f;

        angle = -angle.normalized;

        Debug.DrawRay(pCtrl.transform.position, angle, Color.red);

        if(pCtrl.transform.position.y<curHook.transform.position.y&&pCtrl.playerRb.velocity.magnitude>3f)
        {
            pCtrl.playerRb.AddForce(angle * horizontal * swingPower, ForceMode.Force);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (pCtrl.isSwinging)
        {
            Gizmos.DrawSphere(Vector3.Lerp(curHook.transform.position, transform.TransformPoint(pCtrl.playerRb.centerOfMass), jumpThreshold / curRange), .4f);
        }
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
            //rope collision
            //if(Physics.Raycast())
        }
    }
    
    //handles reloading and shooting
    public void Shoot()
    {
        //checks if current hook exists, destroys if so and instantiates new one
        //"Click to shoot, click to remove, click to shoot again"
        if (curHook == null)//shoot
        {
            staticHook.SetActive(false);
            recoil.isShooting = true;

            //pCtrl.playerRb.AddForceAtPosition(-transform.forward * recoilForce, transform.position, ForceMode.VelocityChange);

            pCtrl.playerRb.AddForce(-transform.forward * recoilForce, ForceMode.VelocityChange);

            curHook = Instantiate(HookPrefab, barrel.transform.position, barrel.transform.rotation);
            curHookRb = curHook.GetComponent<Rigidbody>();
            curHookRb.AddForce(barrel.transform.forward * power, ForceMode.Impulse);
            curParticle = Instantiate(gunshotParticlePrefab, barrel.transform.position, barrel.transform.rotation);

            curParticle.transform.parent = barrel.transform;

            Destroy(curParticle, 5f);

            joint.connectedBody = curHookRb;
            joint.connectedAnchor = Vector3.zero;

            targetDist = range;

            joint.massScale = massInfluence;
            joint.enableCollision = false;

        }
        else//if hook exists destroy
        {
            Retract();
        }
    }

    public void Retract()
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

    //handles the actual hooking mechanic of the hook hooking
    //takes in the hit rigidbody as the anchor point
    //called from HookController.cs
    public void SetHook(Rigidbody anchor)
    {
        //checks to see whether the anchor is dynamic or static for pulling/swinging
        if (anchor == null)//look for rigidbody on object
        {
            pCtrl.isSwinging = true;
            //need to make move with objects;
            curHookRb.isKinematic = true;
            //joint.maxDistance = Vector3.Distance(transform.position, curHook.transform.position);
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
            joint.enableCollision = true;
            curHook.SetActive(false);
        }
    }
}
/*
public class GrappleController : MonoBehaviour
{

    //DEPENDENCIES
    [Header("Dependencies")]
    public GameObject HookPrefab;
    public GameObject HookStaticModel;
    public GameObject barrel;
    public GameObject gunshotParticlePrefab;

    [Header("Modifiers")]
    public float climbSpeed;
    public float fastClimbDamper;
    public float ropeJumpPower = 15f;
    public float jumpThreshold=2f; //distance from anchor before player can jump up from rope;
    public float range;
    public float minRange = 0.3f;
    public float curRange;
    public float power;
    public float recoilForce=0.3f;
    public float massInfluence = 1f;
    [Header("Rope Attributes")]
    public int ropeSmoothness = 7;
    public float ropeDroop = -1f;

    [Header("Conditionals")]
    public bool hookIsSet;

    //INTERNALS
    private SpringJoint joint;
    private GameObject staticHook;
    private SideScrollController pCtrl;
    private WeponRecoil recoil;
    private LineRenderer line;
    private fInput inputCtrl;
    [HideInInspector]
    public GameObject curHook;
    private Rigidbody curHookRb;

    private Rigidbody curAnchorRb;
    private Vector3 lineMid;
    public Vector3 lineEnd;
    private float lineCoef;
    public GameObject curParticle;
    public float tightenSpeed;
    float targetDist;

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
        if (inputCtrl.isShooting || inputCtrl.reset && curHook != null) 
        {
            Shoot();
        }


        HandleLine();
        Mathf.Clamp(massInfluence,0f,3f);
        lineCoef = ropeDroop / 10f;

        if(pCtrl.isSwinging)
        {
            joint.minDistance = joint.maxDistance; // clamps distance to minimize bounciness 
            curRange = Vector3.Distance(pCtrl.transform.TransformPoint(pCtrl.playerRb.centerOfMass), curHook.transform.position);

            if (Input.GetButtonUp("Jump") && !pCtrl.isGrounded)
            {
                Retract();
                if(curRange-minRange<jumpThreshold)
                {
                    pCtrl.SpecialJump(ropeJumpPower, Vector3.up, true);
                }
            }
            else if (inputCtrl.vertical > 0f||Input.GetButton("Jump"))//climb
            {
                if (Input.GetButton("Jump") && Mathf.Abs(pCtrl.playerRb.velocity.x) < 3f && transform.position.y < curHook.transform.position.y)
                {
                    joint.damper = fastClimbDamper;
                    targetDist -= climbSpeed* Time.deltaTime;
                }
                else
                {
                    joint.damper = 150f;
                    targetDist -= climbSpeed * Time.deltaTime;
                }
            }
            else if (inputCtrl.vertical < 0f)//drop
            {
                targetDist += climbSpeed * Time.deltaTime;
            }
            else
            {
                joint.damper = 150f;
                if (curRange < joint.maxDistance)
                {
                    targetDist = Mathf.Clamp(Vector3.Distance(pCtrl.transform.position, curHook.transform.position), 1f, range);
                }
            }

            //joint.maxDistance = Mathf.Lerp(joint.maxDistance, targetDist, Time.deltaTime * tightenSpeed);
            targetDist = Mathf.Clamp(targetDist, .3f, range);
            joint.maxDistance = targetDist;
        }
        //joint.anchor = transform.InverseTransformPoint(barrel.transform.position);
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (pCtrl.isSwinging)
        {
            Gizmos.DrawSphere((transform.position - curHook.transform.position).normalized * jumpThreshold, .4f);
        }
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
        if (curHook == null)//shoot
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
            targetDist = range;
            joint.massScale = massInfluence;
            joint.enableCollision = false;

        }
        else//if hook exists destroy
        {
            Retract();
        }
    }

    public void Retract()
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

    //handles the actual hooking mechanic of the hook hooking
    //takes in the hit rigidbody as the anchor point
    //called from HookController.cs
    public void SetHook(Rigidbody anchor)
    {
        //checks to see whether the anchor is dynamic or static for pulling/swinging
        if (anchor == null)//look for rigidbody on object
        {
            pCtrl.isSwinging = true;
            //need to make move with objects;
            curHookRb.isKinematic = true;
            //joint.maxDistance = Vector3.Distance(transform.position, curHook.transform.position);
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
            joint.enableCollision = true;
            curHook.SetActive(false);
        }
    }
}
*/
//to work with spring joint
//to work with configurable joint
/*
public class GrappleController : MonoBehaviour 
{

    //DEPENDENCIES
    [Header("Dependencies")]
    public GameObject HookPrefab;
    public GameObject HookStaticModel;
    public GameObject barrel;
    public GameObject gunshotParticlePrefab;
    public GameObject dashParticlePrefab;

    [Header("Modifiers")]
    public float climbSpeed;
    public float fastClimbDamper;
    public float ropeJumpPower = 15f;
    public float jumpThreshold=2f; //distance from anchor before player can jump up from rope;
    public float range;
    public float minRange = 0.3f;
    public float curRange;
    public float power;
    public float recoilForce=0.3f;
    public float massInfluence = 1f;
    [Header("Rope Attributes")]
    public int ropeSmoothness = 7;
    public float ropeDroop = -1f;

    [Header("Conditionals")]
    public bool hookIsSet;

    //INTERNALS
    private ConfigurableJoint joint;
    private GameObject staticHook;
    public SideScrollController pCtrl;
    private WeponRecoil recoil;
    private LineRenderer line;
    private fInput inputCtrl;
    private SoftJointLimit jointLimits;
    [HideInInspector]
    public GameObject curHook;
    private Rigidbody curHookRb;

    private Rigidbody curAnchorRb;
    private Vector3 lineMid;
    public Vector3 lineEnd;
    private float lineCoef;
    public GameObject curParticle;
    public float tightenSpeed;
    public float targetDist;

    // Use this for initialization
    void Start()
    {
        recoil = FindObjectOfType<WeponRecoil>();
        line = gameObject.GetComponent<LineRenderer>();
        inputCtrl = FindObjectOfType<fInput>();
        joint = gameObject.GetComponent<ConfigurableJoint>();
        pCtrl = FindObjectOfType<SideScrollController>();
        staticHook = Instantiate(HookStaticModel, barrel.transform);
        jointLimits = joint.linearLimit;
        Retract();
    }

    // Update is called once per frame
    void Update()
    {

        HandleLine();
        Mathf.Clamp(massInfluence, 0f, 3f);
        lineCoef = ropeDroop / 10f;

        //checks if shooting
        if (inputCtrl.isShooting || inputCtrl.reset && curHook != null) 
        {
            Shoot();
        }

        if (pCtrl.isSwinging)
        {
            HandleSwinging();
        }
        else
        {
            jointLimits.limit = range;
        }
        joint.linearLimit = jointLimits;
    }

    void HandleSwinging()
    {
        curRange = Vector3.Distance(transform.TransformPoint(pCtrl.playerRb.centerOfMass), curHook.transform.position);

        if (Input.GetButtonUp("Jump") && !pCtrl.isGrounded)
        {
            Retract();
            if (curRange - minRange < jumpThreshold)
            {
                pCtrl.SpecialJump(ropeJumpPower, Vector3.up, true);
            }
        }
        else if (inputCtrl.vertical > 0f || Input.GetButton("Jump"))//climb
        {
            if (Input.GetButton("Jump") && Mathf.Abs(pCtrl.playerRb.velocity.x) < 3f && transform.position.y < curHook.transform.position.y)
            {
                //targetDist -= climbSpeed * Time.deltaTime;
                targetDist = Mathf.Lerp(targetDist, minRange, Time.deltaTime * climbSpeed);
            }
            else
            {
                //targetDist -= climbSpeed * Time.deltaTime;
                targetDist = Mathf.Lerp(targetDist, minRange, Time.deltaTime * climbSpeed*.8f);
            }
        }
        else if (inputCtrl.vertical < 0f)//drop
        {
            //targetDist += climbSpeed * Time.deltaTime;
            targetDist = Mathf.Lerp(targetDist, range, Time.deltaTime * climbSpeed);
        }
        else
        {
            if (curRange < joint.linearLimit.limit)
            {
                targetDist = Mathf.Clamp(Vector3.Distance(pCtrl.transform.position, curHook.transform.position), 1f, range);
            }
        }

        //joint.maxDistance = Mathf.Lerp(joint.maxDistance, targetDist, Time.deltaTime * tightenSpeed);
        targetDist = Mathf.Clamp(targetDist, .3f, range);

        //if (jointLimits.limit > targetDist)
        //{
        //    GameObject dust;
        //    dust = Instantiate(dashParticlePrefab);
        //    dust.transform.position = transform.position;
        //    Destroy(dust, 5f);
        //}
        jointLimits.limit = targetDist;
    }
    //joint.anchor = transform.InverseTransformPoint(barrel.transform.position);
    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (pCtrl.isSwinging)
        {
            Gizmos.DrawSphere(Vector3.Lerp(curHook.transform.position, transform.TransformPoint(pCtrl.playerRb.centerOfMass),jumpThreshold/curRange), .4f);
        }
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
            //rope collision
            //if(Physics.Raycast())
        }
    }

    //handles reloading and shooting
    public void Shoot()
    {
        //checks if current hook exists, destroys if so and instantiates new one
        //"Click to shoot, click to remove, click to shoot again"
        if (curHook == null)//shoot
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
            targetDist = range;
            joint.massScale = massInfluence;
            joint.enableCollision = false;

        }
        else//if hook exists destroy
        {
            Retract();
        }
    }

    public void Retract()
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

    //handles the actual hooking mechanic of the hook hooking
    //takes in the hit rigidbody as the anchor point
    //called from HookController.cs
    public void SetHook(Rigidbody anchor)
    {
        //checks to see whether the anchor is dynamic or static for pulling/swinging
        if (anchor == null)//look for rigidbody on object
        {
            pCtrl.isSwinging = true;
            //need to make move with objects;
            curHookRb.isKinematic = true;
            //joint.maxDistance = Vector3.Distance(transform.position, curHook.transform.position);
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
            joint.enableCollision = true;
            curHook.SetActive(false);
        }
    }
    */

