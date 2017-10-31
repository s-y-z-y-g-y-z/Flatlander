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
    public float swingAccelerationPower;
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
    public float maxLean;
    float turnRot;
    public float targetLean;
    float leanVal;
    
    //PUBLIC INTERNALS
    public Rigidbody playerRb;
    public Vector3 localVelocity;

    //handles player animation for aiming
    public GameObject gunObj;
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
    public Vector3 initPlayerPos;
    //player aiming
    //private GameObject rightShoulderPoint;
    private float lastRotate;

    //USER INPUT PARAMETERS
    public float leanAmt;
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

        inputCtrl = FindObjectOfType<fInput>();
        initPlayerPos = transform.position;
    }
    
    //for non-phsyics physics and calculations
    private void Update()
    {
        HandleAnimValues();
        HandleJump();
        lookPos = inputCtrl.lookPos;
        if(inputCtrl.reset)
        {
            resetPosition();
        }
        horizontal = inputCtrl.horizontal;
        vertical = inputCtrl.vertical;
        isJumping = inputCtrl.isJumping;
        isSideScrolling = inputCtrl.isSideScrolling;
        groundCheckHeights = new Vector2(groundHitFront.point.y, groundHitBack.point.y); //checks slopes of surface relative to player forward
        yVelocity = playerRb.velocity.y;
        
    }

    public void resetPosition()
    {
        transform.position = initPlayerPos;
        playerRb.velocity = Vector3.zero;
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
        HandleFriction();
        //HandleShoulder();
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
            else if(isGrounded&&isSwinging)
            {
                playerRb.AddForce(xForceDirection * swingAccelerationPower, ForceMode.VelocityChange);
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

    public void SpecialJump(float power, Vector3 dir, bool useNormalJump)
    {
        if(useNormalJump)
        {
            playerRb.AddForce(dir * jumpPower, ForceMode.Impulse);
        }
        else
        {
            playerRb.AddForce(dir * power, ForceMode.Impulse);
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
            playerPhys.dynamicFriction = .8f;
            playerPhys.staticFriction = 1f;
            playerRb.AddForce(-groundNormal * stickForce * 5f, ForceMode.Force);
        }
    }

    //sends values to animator
    void HandleAnimValues()
    {
        //sends values to animator
        anim.SetBool("OnAir", !isGrounded);
        anim.SetBool("isSideScrolling", isSideScrolling);
        anim.SetFloat("MovementX", localVelocity.x/maxSpeed);
        anim.SetFloat("MovementZ", localVelocity.z/maxSpeed);
        anim.SetFloat("AirMovement", playerRb.velocity.y);
    }

    //handles player rotation
    void HandleRotation()
    {


        float dir = Mathf.Sign(localVelocity.z);
        leanAmt = Mathf.Clamp01(Mathf.Abs(playerRb.velocity.x) / maxSpeed);

        if (isSlowing||isSwinging)
        {
            targetLean= -leanAmt*dir;
        }
        else
        {
            targetLean = leanAmt*dir;
        }

        leanVal = Mathf.Lerp(leanVal, targetLean*maxLean, Time.deltaTime * leanSpeed);

        //get rotation from aimpoint
        Vector3 lookDir;

        lookDir = (lookPos - transform.position);

        float look;
        if (!isSwinging)
        {
            if (lookPos.x < transform.position.x)
            {
                look = -90f;
            }
            else
            {
                look = 90f;
            }
        }
        else
        {
            look = turnRot;
        }

        turnRot = Mathf.Lerp(turnRot, look, Time.deltaTime * turnSpeed);
        //the target rotation for the player rotation
        Quaternion targetRotation;
        targetRotation = Quaternion.Euler(leanVal, turnRot, 0f);

        playerRb.transform.rotation = targetRotation;
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
