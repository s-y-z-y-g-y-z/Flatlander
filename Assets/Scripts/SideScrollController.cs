using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * USED
 * BEN SPURR
 * 
 * ******************
 * SCRIPT REFERENCES
 * 
 * fInupt.cs
 * ------------------
 * 
 * The heavyweight
 * Controls player movement and actions and animations
 * 
 * ATTACHED TO Player
 * 
*/

public class SideScrollController : MonoBehaviour
{
    //PUBLICS

    //Multipliers for movement values
    [Header("Movement Modifiers")]
    public float groundAccelerationPower;
    public float airAccelerationPower;
    public float jumpPower;
    public float groundCheckDistance;   //ray distance for ground check
    public float maxSpeed;
    public float slowSpeed;
    public float stickForce;            //downward grounded force

    //General condition values
    [Header("Conditions and Info")]
    public bool isSideScrolling = true; //checks persp.
    public float currentVelocity;
    public float yVelocity;
    public bool isGrounded;
    public bool isSwinging;             //from GrappleController.cs
    public bool facingLeft;
    public bool isSlowing;              //player is decelerating horizontally
    public bool inContact;              //checks if the player is in contact with any other collider
    public bool drawDebug = false;
    public LayerMask groundMask;        //Layers the ground rays can hit
    public PhysicMaterial playerPhys;
    float impactForce;
    bool flipped;

    //Procedural animation values
    [Header("Leaning and Rotating")]
    public float turnSpeed = 15f;
    public float leanSpeed = 1f;
    public float leanAmount = .5f;
    public float topLeanAmount = .5f;
    float turnRot;
    float curLean;
    float leanZ;
    float leanX;
    
    //PUBLIC INTERNALS
    public Rigidbody playerRb;
    public Vector3 localVelocity;

    //handles player animation for aiming
    public Transform shoulderIkTrans;
    public Transform rightShoulder;
    public Vector3 lookPos;

    //PRIVATE INTERNALS

    //all pretty self explanatory
    private CapsuleCollider playerCollider;
    private RaycastHit groundHitFront;
    private RaycastHit groundHitMid;
    private RaycastHit groundHitBack;
    private Vector3 xForceDirection;
    private Vector2 groundCheckHeights;
    private Vector3 groundNormal;
    private Animator anim;
    private Transform modelTrans;
    private fInput inputCtrl;
    private GrappleController grapple;
    private Vector3 initPlayerPos;
    //player aiming
    private GameObject rightShoulderPoint;
    private float lastRotate;

    //USER INPUT PARAMETERS
    public float horizontal;
    public float vertical;
    private float topForceDir;
    private bool isJumping;
    private Vector3 xzForceDirection;
    
    //initializes objects and sets up animator
    void Start ()
    {
        playerRb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        SetupAnimator();
        rightShoulderPoint = new GameObject();
        rightShoulderPoint.name = transform.root.name + "Right Shoulder IK Helper";
        inputCtrl = FindObjectOfType<fInput>();
        initPlayerPos = transform.position;
    }
    
    //for non-phsyics physics and calculations
    private void Update()
    {
        HandleAnimValues();
        HandleJump();
        if(inputCtrl.reset)
        {
            transform.position = initPlayerPos;
            playerRb.velocity = Vector3.zero;
        }
        horizontal = inputCtrl.horizontal;
        vertical = inputCtrl.vertical;
        isJumping = inputCtrl.isJumping;
        isSideScrolling = inputCtrl.isSideScrolling;
        groundCheckHeights = new Vector2(groundHitFront.point.y, groundHitBack.point.y); //checks slopes of surface relative to player forward
        yVelocity = playerRb.velocity.y;
        
    }

    //physics functions and calclations
    public void FixedUpdate()
    {
        currentVelocity = playerRb.velocity.magnitude;
        localVelocity.x= transform.InverseTransformDirection(playerRb.velocity).x;
        localVelocity.z= transform.InverseTransformDirection(playerRb.velocity).z;
        HandleGroundCheck();
        HandleMovement();
        HandleRotation();
        HandleAimingPos();
        HandleFriction();
        HandleShoulder();
    }

//HANDLERS
    //handles movement forces for top-down and sidescrolling
    void HandleMovement()
    {
        if (isSideScrolling)
        {
            //movement forces for while sidescrolling
            if (isGrounded)
            {
                if (currentVelocity < maxSpeed)
                {
                    playerRb.AddForce(xForceDirection * groundAccelerationPower, ForceMode.Force);
                }
            }
            else
            {
                playerRb.AddForce(xForceDirection * airAccelerationPower, ForceMode.Force);
            }

            //slowing forces for horizontal momentum
            if (isGrounded && horizontal == 0f)
            {
                isSlowing = true;
                playerRb.AddForce(new Vector3(-playerRb.velocity.magnitude, 0f, 0f) * slowSpeed, ForceMode.Acceleration);
            }
            else
            {
                isSlowing = false;
            }
        }
        else
        {
            //top-down movement
            if (isGrounded)
            {
                xzForceDirection = new Vector3(horizontal, 0f, vertical).normalized;
                //xzForceDirection = Camera.main.transform.TransformDirection(xzForceDirection);
                xzForceDirection.y = 0.0f;

                //rotation towards the target
                //Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

                if (currentVelocity < maxSpeed)
                {
                    playerRb.AddForce(xzForceDirection * groundAccelerationPower, ForceMode.Force);
                }
            }
        }
    }

    //handles jump forces
    void HandleJump()
    {
        //jump
        if (isGrounded && isJumping && isSideScrolling)
        {
            playerRb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }

    //checks if the player is on the ground and handles surface slopes
    void HandleGroundCheck()
    {
        //finds the center of volume from the origin point
        Vector3 offset = new Vector3(0f, playerCollider.height / 2f, 0f);

        //ground normal check
        if (Physics.Raycast(transform.position+offset, Vector3.down, out groundHitMid, groundCheckDistance * 2f, groundMask))
        {
            groundNormal = groundHitMid.normal;

            HandleFriction();
        }
        else
        {
            groundNormal = Vector3.up;
        }

        //find tangent of ground normal for force direction
        Vector3 tangent = Vector3.Cross(groundNormal, transform.forward);

        //makes movement on slopes easier through directional forces
        if (tangent.magnitude == 0)
        {
            tangent = Vector3.Cross(groundNormal, Vector3.up);
        }

        xForceDirection = new Vector3(horizontal, tangent.y, 0f);
        
        //checks if the player is on the ground
        if (Physics.Raycast(transform.position+offset + (transform.forward * .3f), Vector3.down, out groundHitFront, groundCheckDistance, groundMask) ||
            Physics.Raycast(transform.position+offset + (-transform.forward * .3f), Vector3.down, out groundHitBack, groundCheckDistance, groundMask))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        //draws rays for debugging
        if(drawDebug)
        {
            Debug.DrawRay(transform.position + offset + (transform.forward * .3f), Vector3.down * groundCheckDistance, Color.blue);
            Debug.DrawRay(transform.position + offset + (-transform.forward * .3f), Vector3.down * groundCheckDistance, Color.blue);
            Debug.DrawRay(groundHitMid.point, groundNormal, Color.red);
        }
    }

    //controls the friction for the colliders
    void HandleFriction()
    {
        //high friction when input, no friction when no input
        if (horizontal != 0 || vertical != 0)
        {
            playerPhys.dynamicFriction = .1f;
            playerPhys.staticFriction = .1f;
            playerRb.AddForce(-groundNormal * stickForce, ForceMode.Force);
        }
        else
        {
            playerPhys.dynamicFriction = 1f;
            playerPhys.staticFriction = 5f;
            playerRb.AddForce(-groundNormal * stickForce * 5f, ForceMode.Force);
        }
    }

    //sends values to animator
    void HandleAnimValues()
    {
        //checks if facing left
        if (lookPos.x < transform.position.x)
        {
            facingLeft = false;
        }
        else
        {
            facingLeft = true;
        }

        //sends values to animator
        anim.SetBool("OnAir", !isGrounded);
        anim.SetBool("isSideScrolling", isSideScrolling);
        anim.SetFloat("MovementX", localVelocity.x/maxSpeed);
        anim.SetFloat("MovementZ", localVelocity.z/maxSpeed);
        anim.SetFloat("AirMovement", playerRb.velocity.y);
    }

//////NEEDS FIXIN'
    //Finds a position for the character to aim at for it to rotate
    void HandleAimingPos()
    {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Vector3 point = hit.point;

                //limits aiming depending persp.
                if (isSideScrolling)
                {
                    point.z = transform.position.z;
                }
                else if(isSwinging)
                {
                    point = grapple.lineEnd;
                }
                else
                {
                    point.y = rightShoulderPoint.transform.position.y;
                }

                lookPos = point;
            }
        

    }

    //handles player rotation
    void HandleRotation()
    {

        //checks if sidescrolling, handles flip values for leaning
        if (isSideScrolling)
        {
            if (facingLeft && playerRb.velocity.x < 0f)
            {
                flipped = true;
            }
            else if (!facingLeft && playerRb.velocity.x > 0f)
            {
                flipped = true;
            }
            else
            {
                flipped = false;
            }
        }
        else
        {
            flipped = false;
        }

        //turning

        //if (horizontal > 0f && isGrounded)
        //{
        //    facingLeft = false;
        //    turnRot = Mathf.Lerp(turnRot, 90f, Time.deltaTime * turnSpeed);
        //}
        //else if (horizontal < 0f && isGrounded)
        //{
        //    facingLeft = true;
        //    turnRot = Mathf.Lerp(turnRot, 270f, Time.deltaTime * turnSpeed);
        //}

        //aim turning
        Vector3 lookDir = (lookPos - transform.position);
        lookDir.y = 0;
        float turnRot = Quaternion.LookRotation(lookDir).eulerAngles.y;

        //handles leaning animation
        if (isSideScrolling)
        {
            //handles flip
            float flip;
            if (flipped)
            {
                flip = -1f;
            }
            else
            {
                flip = 1f;
            }

            //leaning
            if (isSlowing)
            {
                leanZ = Mathf.Lerp(leanZ, flip * -60f * (currentVelocity / maxSpeed) * leanAmount, Time.deltaTime * 8f);
            }
            else if (!isGrounded)
            {
                leanZ = Mathf.Lerp(leanZ, flip * 30f * leanAmount, Time.deltaTime * 3f);
            }
            else
            {
                leanZ = Mathf.Lerp(leanZ, (flip * 40f * (currentVelocity / maxSpeed)) * leanAmount, Time.deltaTime * 20f);
            }
        }
        else
        {
            
        }

        //the target rotation for the player rotation
        Quaternion targetRotation;

        //ELSE STATEMENT NEEDS FIXIN'
        if (isSideScrolling)
        {
            targetRotation = Quaternion.Euler(leanZ, turnRot, 0f);
            playerRb.transform.rotation = Quaternion.Slerp(playerRb.transform.rotation, targetRotation, Time.deltaTime * leanSpeed);
        }
        else
        {
            //CURRENTLY DOES NOT LEAN WITH MOMENTUM
            targetRotation = Quaternion.Euler(0f, turnRot, 0f);
            playerRb.transform.rotation = Quaternion.Slerp(playerRb.transform.rotation, targetRotation, Time.deltaTime * leanSpeed); //IS BROKE
        }
    }

    //handles root IK target position
    void HandleShoulder()
    {
        ///procedural animation for aiming
        shoulderIkTrans.LookAt(lookPos);

        Vector3 rightShoulderPos = rightShoulder.TransformPoint(Vector3.zero);
        rightShoulderPoint.transform.position = rightShoulderPos;
        rightShoulderPoint.transform.parent = transform;

        shoulderIkTrans.position = rightShoulderPoint.transform.position;
    }

    //SETUPS
    //sets up the animator
    void SetupAnimator()
    {
        //ref to animator component on root
        anim = GetComponent<Animator>();

        //use avatar from a child animator component if present (for easy swap of char model as child node)
        foreach (var childAnimator in GetComponentsInChildren<Animator>())
        {
            if (childAnimator != anim)
            {
                anim.avatar = childAnimator.avatar;
                modelTrans = childAnimator.transform;
                Destroy(childAnimator);
                //if first animator found, stop search
                break;
            }
        }
    }

//COLLISION CHECKS
    //checks collision entering
    private void OnCollisionEnter(Collision collision)
    {
        //checks the impact force
        impactForce = collision.impulse.magnitude;

        inContact = true;
    }

    //checks if exiting collider
    private void OnCollisionExit(Collision collision)
    {
        inContact = false;
    }
}
