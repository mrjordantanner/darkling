using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public static PlayerCharacter Instance;

    public float pickupDelay = 0.1f;

    public int jumpState;
    [HideInInspector]
    public int maxJumps;

    [HideInInspector]
    public bool invulnerable, canDoubleJump, canSuperJump = true, canMove = true,
    canDash = true, canAttack = true,
    isDashing, inputSuspended, respawning, dead, knockback;

    public float knockbackDuration = 1f;
    public Vector3 knockbackVelocity = new Vector3(10, 10, 10);

    Rigidbody rb;
    public SpriteRenderer spriteRenderer;
    CharacterControls fpsController;
    [HideInInspector]
    public GhostTrails ghostTrails;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        ghostTrails = GetComponentInChildren<GhostTrails>();
        fpsController = FindObjectOfType<CharacterControls>();
        gameObject.name = "Player";
        invulnerable = false;
        canMove = true;
        rb = GetComponent<Rigidbody>();

        // spriteRenderer = GetComponentInChildren<SpriteRenderer>();

    }
    
    private void Update()
    {
        if (dead)
        {
            canMove = false;
            inputSuspended = true;

        }

        HandleJumpState();

    }

    IEnumerator DoubleJumpDelay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        jumpState = 1;
      //  canDoubleJump = true;

    }

    //public int jumps;

    void HandleJumpState()
    {
        //if (Stats.Instance.hasJump1) maxJumps = 2;
        //if (Stats.Instance.hasJump2) maxJumps = 3;
        //else maxJumps = 1;

        //if (canMove)
        //{
        //    // J U M P 
        //    // Grounded  
        //    // STATE 0
        //    if (fpsController.isGrounded)
        //    {
        //        jumpState = 0;
        //        jumps = 0;
        //    }

        //    // STATE 1                      
        //    if (Input.GetKeyDown(InputManager.Instance.jump))
        //    {
        //        if (fpsController.isGrounded)
        //        {
        //            StartCoroutine(DoubleJumpDelay(0.1f));   // wait briefly then set jumpState to 1
        //        }
        //        // STATE 2               
        //        else if (fpsController.isJumping && jumpState <= maxJumps)
        //        {
        //            jumpState = 2;

        //        }
        //        // STATE 3              
        //        else if (fpsController.isJumping && jumpState == 2)
        //        {
        //            jumpState = 3;

        //        }
        //    }

            //if (jumpState == 2 && velocity.y < 0)  // stop double jump animation once descending again
            //    anim.SetBool("DoubleJump", false);

            //// Jump cancel
            //if (Input.GetKeyUp(InputManager.Instance.jump_keyboard) ||
            //    Input.GetKeyUp(InputManager.Instance.jump_gamepad) ||
            //    Input.GetKeyUp(KeyCode.W) ||
            //    Input.GetKeyUp(KeyCode.UpArrow))
            //{
            //    if (velocity.y > 0.01f)
            //        velocity.y = 0.01f;
            //}
        
    }

    //public IEnumerator Knockback(Vector3 knockbackDirection)
    //{
    //    // rb.freezeRotation = true;
    //    //var direction = enemyDamager.gameObject.GetComponent<EnemyMovement>().GetPlayerDirection();
    //    //var direction = 1;
    //    // rb.mass = knockbackMass;
    //    // rb.gravityScale = knockbackGravity;
    //    // knockbackVel = knockbackDirection * knockbackVelocity;
    //    // rb.AddForce(knockbackVel);
    //   // rb.velocity = new Vector3(knockbackDirection.x, knockbackDirection.y, knockbackDirection.z);// * knockbackVelocity;
    //    rb.velocity = knockbackDirection + knockbackVelocity;
    //    knockback = true;
    //    inputSuspended = true;
    //    yield return new WaitForSeconds(knockbackDuration);
    //    knockback = false;
    //    inputSuspended = false;

    //    // yield return new WaitForSeconds(0);
    //}

}
