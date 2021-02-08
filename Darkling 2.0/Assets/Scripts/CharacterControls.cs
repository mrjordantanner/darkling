using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class CharacterControls : MonoBehaviour
{
    // Locomotion and jumping controller, as well as mouselook

    [Header("Input Data")]
    public Vector3 input;
    public Vector3 velocityMonitor;
    public Vector3 localVelocity;

    [Header("Ground Physics")]
    public float walkSpeed = 10.0f;
    public float runSpeed = 12.0f;
    public float maxVelocityChange = 10.0f;
    public float stickToGroundForce = 1f;
    public Transform groundDetector;
    public LayerMask obstacleLayerMask;
    public float skinWidth = 0.15f;
    bool fastMovement;       // for test movement
    public float aimingSpeedReduction = 0.5f;

    [Header("Air Physics")]
    public float gravity = 13f;
    public float airControl = 3f;
    public int maxJumps = 3, jumpsTaken = 0;
    public float primaryJumpHeight, doubleJumpHeight, tripleJumpHeight;


    [Header("Flags")]
    public bool canJump = true;
    public bool grounded = false;
    public bool previouslyGrounded;
    public bool jumping;
    public bool jumpPressed;
    public bool running = false;
    public bool wasRunning;
    public MouseLook mouseLook;
    // public bool mouseLookEnabled;
    [HideInInspector]
    public Camera cam;
    [HideInInspector]
    public GrappleGun grappleGun;
    [HideInInspector]
    public Rigidbody rigidbody;
    public float airborneTimer;
    public float bumpBuffer = 0.75f;

    void Awake()
    {
        grappleGun = FindObjectOfType<GrappleGun>();
        cam = Camera.main;
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
        rigidbody.useGravity = false;
        mouseLook.Init(transform, cam.transform);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(groundDetector.position, new Vector3(0, -skinWidth, 0));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            fastMovement = !fastMovement;
            rigidbody.useGravity = !rigidbody.useGravity;
        }

        input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // bool isCrouching = crouch && !isSprinting && !isJumping && isGrounded;

        // Mouselook
        if (!GameManager.Instance.gamePaused)// && mouseLookEnabled)
            RotateView();

        // No physics during grapple process
                    //  if (grappleGun.retracting) return;

        // Get jump input
        if (!jumpPressed)
            jumpPressed = Input.GetKeyDown(InputManager.Instance.jump);

        // Count jumps
        JumpCounter();

        // Toggle walk/run
        if (grounded)
            HandleRunState();

        //// Become ungrounded
        //if (!grounded && previouslyGrounded)
        //{
        //    trackAirborneDuration = true; 

        //}

        if (!grounded)
            airborneTimer += Time.deltaTime;

        // Landing
        if (!previouslyGrounded && grounded)
        {
            if (airborneTimer > bumpBuffer)
            {
                GunController.Instance.LandingPhysics();
                input = Vector3.zero;
            }

            airborneTimer = 0;
            jumping = false;
            jumpsTaken = 0;
            jumpPressed = false;

            if (wasRunning)
            {
                StartRunning();
                wasRunning = false;
            }
        }

        previouslyGrounded = grounded;
        if (grounded) rigidbody.AddForce(Vector3.down * stickToGroundForce);
    }


   void FixedUpdate()
    {
        // No physics during grapple process
            // if (grappleGun.retracting) return;

        localVelocity = transform.InverseTransformDirection(rigidbody.velocity);

        // Ground detection
        RaycastHit hit;
        if (Physics.Raycast(groundDetector.position, Vector3.down, out hit, skinWidth, obstacleLayerMask))
            grounded = true;
        else
            grounded = false;

       // if (grounded) print("Standing on " + hit.collider.gameObject.name);
       // else print("Not grounded");

        // Horizontal movement
        if (grounded) rigidbody.AddForce(CalculateHorizontalSpeed(), ForceMode.VelocityChange);

        // Jump from ground
        if (grounded && jumpPressed && canJump)
        {
            if (running)
            {
                wasRunning = true;
                StopRunning();
            }

            if (GunController.Instance.isAiming) GunController.Instance.EndAiming();

            jumping = true;
            jumpPressed = false;

            rigidbody.velocity = new Vector3(rigidbody.velocity.x, CalculateJumpVerticalSpeed(), rigidbody.velocity.z);
            //rigidbody.AddForce(new Vector3(0, CalculateJumpVerticalSpeed(), 0), ForceMode.VelocityChange);

            GunController.Instance.TakeoffPhysics();

            jumpsTaken++;
        }
        // Jump while airborne
        if (!grounded && jumpPressed && canJump)
        {
            jumping = true;
            jumpPressed = false;

            rigidbody.velocity = new Vector3(rigidbody.velocity.x, CalculateJumpVerticalSpeed(), rigidbody.velocity.z);
           // rigidbody.AddForce(new Vector3(0, CalculateJumpVerticalSpeed(), 0), ForceMode.VelocityChange);


            GunController.Instance.TakeoffPhysics();
            jumpsTaken++;
        }

        // Do air control
          if (jumping)
          {
               AirControl();
          }


        // Apply gravity manually
        rigidbody.AddForce(new Vector3(0, -gravity * rigidbody.mass, 0));

        // Set flags
        jumpPressed = false;
        mouseLook.UpdateCursorLock();
    }

    Vector3 targetVelocity;

    Vector3 CalculateHorizontalSpeed()
    {
       // Vector3 targetVelocity;

       // float inputModifyFactor = (input.x != 0.0f && input.y != 0.0f) ? .7071f : 1.0f;

        // cap diagonal movement when grounded
        if (grounded && input.magnitude > 1)
            targetVelocity = input.normalized;
        else
            targetVelocity = input;

        var speed = running ? runSpeed : walkSpeed;
        if (GunController.Instance.isAiming) speed = walkSpeed * aimingSpeedReduction;
        if (fastMovement) speed = 35f;

        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= speed;

        velocityMonitor = targetVelocity;

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = rigidbody.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;

        return velocityChange;
    }

    float CalculateJumpVerticalSpeed()
    {
        var height = primaryJumpHeight;

        switch (jumpsTaken)
        {
            case 0:
            height = primaryJumpHeight;
            break;

            case 1:
            height = doubleJumpHeight;
            break;

            case 2:
            height = tripleJumpHeight;
            break;

            default:
            height = primaryJumpHeight;
            break;
 
        }

        return Mathf.Sqrt(2 * height * gravity);
    }

    public float airControlThreshold = 0.5f;




    // Applies gradual forces based on user input while jumping
    void AirControl()
    {
        //  input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // Forward
        if ((input.z > 0) || (input.z < airControlThreshold))  
        {
            rigidbody.AddForce(transform.forward * airControl * input.z);//, ForceMode.Force);
        }

        // Backward
        if ((input.z < 0) || (input.z > -airControlThreshold))    
        {
            rigidbody.AddForce(transform.forward * airControl * input.z);
        }
        
        // Right
        if ((input.x > 0)   || (input.x < airControlThreshold))   
        {
            rigidbody.AddForce(transform.right * airControl * input.x);
        }

        // Left
        if ((input.x < 0)   || (input.x > -airControlThreshold))    
        {
            rigidbody.AddForce(transform.right * airControl * input.x);
        }



    }

    void JumpCounter()
    {
        if (jumpsTaken < maxJumps)
            canJump = true;
        else
            canJump = false;
    }

    void HandleRunState()
    {
        // Toggle run/walk state
        if (Input.GetKeyDown(InputManager.Instance.run) && !(input.z < 0))  // cant run while moving backwards
        {
            if (running) StopRunning();
            else StartRunning();
        }

        if (running && GunController.Instance.isAiming)
            GunController.Instance.EndAiming();

        // cancel run if move backwards or let off the accelerator
        if ((input.z < 0 && running) || (input.z < 0.75f && running)) StopRunning();

        // stop running when input drops // TODO:  make this more precise
        if (input == Vector3.zero && running) StopRunning();

    }

    public void StartRunning()
    {
        running = true;
        // play takeoff sound
        // do vfx blur or fov kick if desired
        GunController.Instance.StartRunCycle();
    }


    public void StopRunning()
    {
        running = false;
        GunController.Instance.StopRunCycle();
    }

    private void RotateView()
    {
        mouseLook.LookRotation(transform, cam.transform);
    }


}


