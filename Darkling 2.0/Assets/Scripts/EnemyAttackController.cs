using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    public enum AttackState { Idle, Anticipating, Attacking, Resetting }   // use these as Ints in animator for Animation Triggers

    public AttackState attackState;
    public bool attackCycleActive;                                
    float attackWaitTime;                                         // selected waitTime
    public float minAttackWaitTime, maxAttackWaitTime;           // min/max possible wait times
    public float attackTimer;                                    // current timer


    [SerializeField]
    public IEnemyAttack currentAttack;
    [SerializeField]
    public IEnemyAttack[] enemyAttackPool;

    PlayerCharacter player;
    public EnemyCharacter enemy;
   // IEnemyBehavior enemyBehavior;
    CharacterController controller;
    Rigidbody rb;


    void Start()
    {
        player = PlayerRef.Instance.player;
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();

       // enemyBehavior = GetComponentInParent<IEnemyBehavior>();
        enemyAttackPool = GetComponentsInChildren<IEnemyAttack>();

        // Set initial wait time for attacks 
        attackWaitTime = Random.Range(minAttackWaitTime, maxAttackWaitTime);

    }


    void Update()
    {
        // Increment timer between attacks if not currently attacking
        if (enemy.canMove && !attackCycleActive && !player.dead && !player.respawning)// && inAttackRange)
            HandleAttackTimer();

        if (player.dead || player.respawning)
        {
            attackTimer = 0;
        }
    }


    void HandleAttackTimer()
    { 
        attackTimer += Time.deltaTime;

        if ((attackTimer >= attackWaitTime) && enemy.canMove)
        {
            SelectAttack();
            attackTimer = 0;
        }

    }


    public void SelectAttack()
    {
        // Choose an attack to perform from our list of possible attacks
        if (enemyAttackPool.Length > 0)
        {
            currentAttack = enemyAttackPool[Random.Range(0, enemyAttackPool.Length)];
            StartCoroutine(StartAttackCycle(currentAttack));
        }
        else
            print(enemy.gameObject.ToString() + " has no attacks assigned to it.");
    }


    //  A T T A C K    C Y C L E
    public IEnumerator StartAttackCycle(IEnemyAttack currentAttack)
    {
        attackCycleActive = true;
        //enemy.anim.SetBool("AttackIdle", false);

        //// Anticipation
        //attackState = AttackState.Anticipating;
        //enemy.anim.SetBool("Anticipating", true);
        ////AudioManager.Instance.Play(currentAttack.anticipationSound);
        //yield return new WaitForSeconds(currentAttack.AnticipationTime);
        //enemy.anim.SetBool("Anticipating", false);

        // Attacking
     //   attackState = AttackState.Attacking;
      //  enemy.anim.SetBool("Attacking", true);
        currentAttack.Attack();
        //AudioManager.Instance.Play(currentAttack.attackSound);
        yield return new WaitForSeconds(currentAttack.AttackTime);
     //   enemy.anim.SetBool("Attacking", false);

        //// Resetting
        //attackState = AttackState.Resetting;
        //enemy.anim.SetBool("Resetting", true);
        ////AudioManager.Instance.Play(currentAttack.resetSound);
        //yield return new WaitForSeconds(currentAttack.ResetTime);
        //enemy.anim.SetBool("Resetting", false);

        // Return to Idle
      //  attackState = AttackState.Idle;
      //  enemy.anim.SetBool("AttackIdle", true);
        attackCycleActive = false;
        attackWaitTime = Random.Range(minAttackWaitTime, maxAttackWaitTime);
    }



    // Attack State indicator
    //private void OnDrawGizmos()
    //{
    //    if (attackState == AttackState.Anticipating) Gizmos.color = Color.yellow;
    //    if (attackState == AttackState.Attacking) Gizmos.color = Color.red;
    //    if (attackState == AttackState.Resetting) Gizmos.color = Color.green;

    //    Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z), 0.25f);

    //}


}


