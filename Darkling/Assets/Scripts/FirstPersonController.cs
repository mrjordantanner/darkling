using System;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections;


[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    public bool canJump;
    public int jumpsTaken, maxJumps;
    public bool m_IsWalking;
    public float m_WalkSpeed;
    public float m_RunSpeed;
    public float m_JumpSpeed;
    public float m_StickToGroundForce;
    public float m_GravityMultiplier;
    public MouseLook m_MouseLook;
    public bool isGrounded;
    public bool isJumping;
    public bool jumpButtonPressed;

    public Camera m_Camera;
    // private float m_YRotation;
    public Vector2 m_Input;
    private Vector3 m_MoveDir = Vector3.zero;
    private CharacterController m_CharacterController;
    private CollisionFlags m_CollisionFlags;
    private bool m_PreviouslyGrounded;
    private Vector3 m_OriginalCameraPosition;

    Rigidbody rb;
    PlayerCharacter player;

    private void Start()
    {
        player = GetComponent<PlayerCharacter>();
        m_CharacterController = GetComponent<CharacterController>();
        m_OriginalCameraPosition = m_Camera.transform.localPosition;
        isJumping = false;
        m_MouseLook.Init(transform, m_Camera.transform);
        rb = GetComponent<Rigidbody>();
    }



    void HandleJump()
    {
        maxJumps = 1;
        if (Stats.Instance.hasJump1) maxJumps = 2;
        if (Stats.Instance.hasJump2) maxJumps = 3;

        if (jumpsTaken < maxJumps)
        {
            canJump = true;
        }
        else
        {
            canJump = false;
        }

        if (canJump && Input.GetKeyDown(InputManager.Instance.jump))
        {
            //print("Jump input");
            // StartCoroutine(StartJump());
            isJumping = true;
           // jumpsTaken++;

        }
    }

    //IEnumerator StartJump()
    //{
    //    isJumping = true;
    //    yield return new WaitForSeconds(0.05f);
    //    jumpsTaken++;


    //}
      
    private void Update()
    {
        isGrounded = m_CharacterController.isGrounded;

        jumpButtonPressed = Input.GetKeyDown(InputManager.Instance.jump);

        if (!GameManager.Instance.gamePaused)
            RotateView();

        HandleJump();

        // Landing
        if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
        {
            m_MoveDir.y = 0f;
            isJumping = false;
            jumpsTaken = 0;
        }

        // Stay on ground
        if (!m_CharacterController.isGrounded && !isJumping && m_PreviouslyGrounded)
        {
            m_MoveDir.y = 0f;
        }

        m_PreviouslyGrounded = m_CharacterController.isGrounded;
    }


    private void FixedUpdate()
    {
        float speed;
        GetInput(out speed);

        // always move along the camera forward as it is the direction that it being aimed at
        Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

        // get a normal for the surface that is being touched to move along it
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                            m_CharacterController.height / 2f, ~0, QueryTriggerInteraction.Ignore);

        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

        m_MoveDir.x = desiredMove.x * speed;
        m_MoveDir.z = desiredMove.z * speed;





        if (m_CharacterController.isGrounded)                // IF GROUNDED
        {
            m_MoveDir.y = -m_StickToGroundForce;               // ADD STICK TO GROUND FORCE

            // Jump while grounded
            if (canJump && isJumping)                                      // IF JUMP, ADD JUMP FORCE
            {
                m_MoveDir.y = m_JumpSpeed;
                jumpButtonPressed = false;
                jumpsTaken++; 
            }


        }  // Jump while airborne
        //else if (jumpButtonPressed && canJump && isJumping)
        //{
        //    m_MoveDir.y = m_JumpSpeed;
        //    jumpButtonPressed = false;
        //    jumpsTaken++;
        //}
        else                                                   // when NOT grounded, add Gravity forced
        {
            m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
        }

        // Jump while airborne
        if (!isGrounded && jumpButtonPressed && canJump && isJumping)
        {
            m_MoveDir.y = m_JumpSpeed;
            jumpButtonPressed = false;
            jumpsTaken++;
        }





        m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);
        m_MouseLook.UpdateCursorLock();
    }


    private void GetInput(out float speed)
    {
        // Read input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
        // On standalone builds, walk/run speed is modified by a key press.
        // keep track of whether or not the character is walking or running

        // bug: cant jump while running forward, has to do with keyboard keyjamming of W + L Shift + Space
        //m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
        m_IsWalking = !Input.GetKey(KeyCode.CapsLock);
#endif

        float tempSpeed1 = 0;
        float tempSpeed2 = 0;

        // TODO: is this approach performance optimal?  not sure
        if (Stats.Instance.hasSpeed1) tempSpeed1 = Stats.Instance.moveSpeed1Amount;
        if (Stats.Instance.hasDemonGlove) tempSpeed2 = Stats.Instance.demonGloveMoveSpeed;

        // set the desired speed to be walking or running
        speed = m_IsWalking ? (m_WalkSpeed * (1 + tempSpeed1 + tempSpeed2) * Stats.Instance.speedPowerupBoost) : (m_RunSpeed * (1 + tempSpeed1 + tempSpeed2) * Stats.Instance.speedPowerupBoost);

        m_Input = new Vector2(horizontal, vertical);

        // normalize input if it exceeds 1 in combined length:
        if (m_Input.sqrMagnitude > 1)
        {
            m_Input.Normalize();
        }
    }


    private void RotateView()
    {
        m_MouseLook.LookRotation(transform, m_Camera.transform);
    }

    /*
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        //dont move the rigidbody if the character is on top of it
        if (m_CollisionFlags == CollisionFlags.Below)
        {
            return;
        }

        if (body == null || body.isKinematic)
        {
            return;
        }

        body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
    }
    */
}

