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
    public AudioClip shootClip;

    [Header("Modifiers")]
    public float climbSpeed;
    public float swingPower;
    public float fallPower;//while swinging
    public float effectiveSwingAngle = 5f;//hanging straight down = 0 degrees
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
    public LayerMask ropeCollisionMask;

    [Header("Conditionals")]
    public bool hookIsSet;
    public bool drawDebug;

    //INTERNALS
    private ConfigurableJoint joint;
    private GameObject staticHook;
    public SideScrollController pCtrl;
    private WeponRecoil recoil;
    private LineRenderer line;
    private fInput inputCtrl;
    private SoftJointLimit jointLimits;
    public List<Vector3> anchors = new List<Vector3>();
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
        gm = FindObjectOfType<GM>();
        ps = FindObjectOfType<ParameterScreen>();
        recoil = FindObjectOfType<WeponRecoil>();
        line = gameObject.GetComponent<LineRenderer>();
        inputCtrl = FindObjectOfType<fInput>();
        joint = gameObject.GetComponent<ConfigurableJoint>();
        pCtrl = FindObjectOfType<SideScrollController>();
        staticHook = Instantiate(HookStaticModel, barrel.transform);
        jointLimits = joint.linearLimit;
        Retract();
    }

    private void Update()
    {
        if ((inputCtrl.isShooting || inputCtrl.reset && curHook != null) && (!pCtrl.isDead || ps.isPaused))
        {
            Shoot();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //HandleLine();
        //SimpleLine();
        SimpleLine();
        Mathf.Clamp(massInfluence, 0f, 3f);
        lineCoef = ropeDroop / 10f;

        if (pCtrl.isSwinging)
        {
            
            HandleRopeLength();
            if (!pCtrl.isGrounded)
            {
                HandleRopeCollision();
                HandleSwingingPhysics(pCtrl.horizontal);
            }
        }
        else
        {
            //HandleLine();
            jointLimits.limit = range;
        }
        joint.linearLimit = jointLimits;
    }

    void HandleRopeLength()//broken(max length forced initially, not smooth)
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

    void HandleSwingingPhysics(float horizontal)
    {
        //find angle perpendicular to raduis of swing by swapping x and y of direction vector
        Vector3 dir = curHook.transform.position - pCtrl.transform.position;

        Vector3 angle = new Vector3(-dir.y, dir.x, 0f).normalized;
        angle = -angle;

        float curSwingAngle = Mathf.Abs(Vector2.SignedAngle(dir, Vector3.up));
        float normalizedPower = (effectiveSwingAngle - curSwingAngle) / effectiveSwingAngle;

        
        if (drawDebug)
        {
            Debug.DrawRay(pCtrl.transform.position, angle, Color.red);
            //Debug.Log("Angle: " + Mathf.Abs(curSwingAngle));
            Debug.Log("Power: " + normalizedPower);

            Vector3 rotatedVector1 = Quaternion.AngleAxis(effectiveSwingAngle, Vector3.forward) * -Vector3.up;
            Vector3 rotatedVector2 = Quaternion.AngleAxis(-effectiveSwingAngle, Vector3.forward) * -Vector3.up;
            Debug.DrawRay(pCtrl.transform.position, dir, Color.red);
            Debug.DrawRay(curHook.transform.position, rotatedVector1.normalized * dir.magnitude, Color.yellow);
            Debug.DrawRay(curHook.transform.position, rotatedVector2.normalized * dir.magnitude, Color.yellow);
        }
        


        if (curSwingAngle <= effectiveSwingAngle)//fakes parametric resonance (aplifies motion by changing length of raduis).  On a real life swing you pull yourself up, here just adding force
        {
            pCtrl.playerRb.AddForce((angle * (horizontal * swingPower)) * normalizedPower, ForceMode.Impulse);
        }
        else //exagerates gravity when outside of angle threshold
        {
            if (pCtrl.yVelocity <= 0f && curSwingAngle > 45f)
            {
                pCtrl.playerRb.AddForce(-Vector3.up * (fallPower), ForceMode.Force);
            }
            else
            {
                pCtrl.playerRb.AddForce(angle * Mathf.Sign(curHook.transform.position.x - pCtrl.transform.position.x) * (fallPower * .4f), ForceMode.Force);
            }
        }
    }

    void HandleRopeCollision()
    {


        Vector3 curDir = anchors[anchors.Count-1] - pCtrl.transform.position;

        Ray ropeRay = new Ray(pCtrl.transform.position + curDir * .1f, curDir);

        RaycastHit ropeCollision;

        Debug.DrawRay(pCtrl.transform.position + curDir * .1f, curDir *.8f, Color.green);

        //add new anchor
        if (Physics.Raycast(ropeRay, out ropeCollision, curDir.magnitude*.8f,ropeCollisionMask)) 
        {
            anchors.Add(ropeCollision.point);
            curHook.transform.position = ropeCollision.point;
        }

        if (anchors.Count > 1)
        {
            Vector3 parentDir = anchors[anchors.Count - 2] - pCtrl.transform.position;

            Ray playerToLastAnchor = new Ray(pCtrl.transform.position + parentDir * .1f, curDir);

            Debug.DrawRay(pCtrl.transform.position + parentDir * .1f, parentDir * .8f, Color.yellow);

            if (!Physics.Raycast(playerToLastAnchor, parentDir.magnitude * .8f, ropeCollisionMask) && !Physics.Raycast(ropeRay, curDir.magnitude * .8f, ropeCollisionMask))
            {
                jointLimits.limit = parentDir.magnitude;
                joint.linearLimit = jointLimits;
                curHook.transform.position = anchors[anchors.Count - 2];
                anchors.Remove(anchors[anchors.Count - 1]);
                if(anchors.Count==2)
                {
                    //curHook.transform.position = anchors[0];
                }
            }
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

    public void SimpleLine()
    {
        if (curHook == null)
        {
            line.enabled = false;
        }
        else
        {
            line.enabled = true;
            line.positionCount = anchors.Count+1;
            line.SetPosition(anchors.Count, barrel.transform.position);
            for (int i = 0; i < anchors.Count; i++)
            {
                line.SetPosition(i, anchors[i]);
            }
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

            //plays shoot sound ~~JK&HA
            SoundManager.PlaySFX(shootClip, true, 1f);

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
        anchors.Clear();
        Destroy(curParticle);
        staticHook.SetActive(true);
        pCtrl.isSwinging = false;
        curAnchorRb = null;
        joint.connectedBody = null;
        joint.massScale = 0f;
        Destroy(curHook);
        curHook = null;
    }

    //if hook collides make the joint anchor pos same as hook rigidbody pos
    //takes in the hit rigidbody as the anchor point
    //called from HookController.cs
    public void SetHook(Rigidbody anchor)
    {
        anchors.Add(curHook.transform.position);
        pCtrl.isSwinging = true;
        //checks to see whether the anchor is dynamic or static for pulling/swinging
        if (anchor == null)//look for rigidbody on object
        {

            //need to make move with objects;
            curHookRb.isKinematic = true;
            jointLimits.limit = Vector3.Distance(transform.position, curHook.transform.position);
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