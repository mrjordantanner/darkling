using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedAttack : MonoBehaviour, IEnemyAttack
{
    public float AnticipationTime { get { return anticipationTime; } }
    public float AttackTime { get { return attackTime; } }
    public float ResetTime { get { return resetTime; } }

    public enum FiringStyle { SingleShot, RapidFire }//, SweepingBurst, Mortar }
  //  public enum TargetingStyle { ConstantDirection, LockOnPlayer }

    [Header("Firing Behavior")]
    public FiringStyle firingStyle;
    public float startDelay;
    public float rapidFireInterval;
    public float anticipationTime;
    public float attackTime;
    public float resetTime;
  //  public float knockback;

  //  [Header("Targeting")]
  //  public TargetingStyle targetingStyle;
  //  public LayerMask HittableLayers;       // set this to Player and Obstacle
   // bool targetTracking;                   // targets last player position before firing
                                           // public bool constantTracking;    // targets current player position during firing
    public Vector3 offset;
    //public float maxAttackRange;
   // public bool requireLineOfSight;
   // public bool hasLineOfSight;

    [Header("Projectile")]
    public GameObject EnemyProjectile;
    EnemyProjectile enemyProjectile;
    public float projectileSpeed = 6.0f;

   // public Sound anticipationSound, attackSound, resetSound;

   // RaycastHit hit;
    Transform target;
    float rapidFireTimer;
    float totalFiringTimer;
    bool canShoot;
    bool onCooldown;

    EnemyAttackController enemyAttackController;
    PlayerCharacter player;
    EnemyCharacter enemy;


    void Start()
    {
        player = PlayerRef.Instance.player;
        enemy = GetComponentInParent<EnemyCharacter>();
        enemyProjectile = GetComponent<EnemyProjectile>();
        enemyAttackController = GetComponentInParent<EnemyAttackController>();
        StartCoroutine(StartDelay());
        target = player.transform;

    }

    IEnumerator StartDelay()
    {
        yield return new WaitForSecondsRealtime(startDelay);
        StartCoroutine(Cooldown(ResetTime));
    }


    public void Attack()
    {
        canShoot = true;
        onCooldown = false;
    }


    void Update()
    {

        if (player.dead || player.respawning)
        {
            totalFiringTimer = 0;
            rapidFireTimer = 0;
        }
        else
            HandleFiringStyle();

    }

    void HandleFiringStyle()
    {
        // SINGLE SHOT
        if (firingStyle == FiringStyle.SingleShot && canShoot && !enemy.enemyFrozen && !onCooldown)
        {
            FireProjectile();
            canShoot = false;
            StartCoroutine(Cooldown(ResetTime));
        }


        // RAPID FIRE
        if (firingStyle == FiringStyle.RapidFire && canShoot && !enemy.enemyFrozen && !onCooldown)
        {
           // targetTracking = false;  // stop tracking during firing process

            if (totalFiringTimer < AttackTime)
            {
                // Increment timers
                totalFiringTimer += Time.deltaTime;
                rapidFireTimer += Time.deltaTime;

                // Rapid Fire Interval
                if (rapidFireTimer >= rapidFireInterval)
                {
                    rapidFireTimer = 0;
                    FireProjectile();
                }
            }

            // Total Fire Time complete, start cooldown
            if (totalFiringTimer >= AttackTime)
            {
                totalFiringTimer = 0;
                rapidFireTimer = 0;
                canShoot = false;
                onCooldown = true;
              //  targetTracking = true;
                StartCoroutine(Cooldown(ResetTime));
            }
        }
    }



    void FireProjectile()
    {
        // GameObject ProjectileInstance = Instantiate(EnemyProjectile, transform.position + offset, Quaternion.identity, Combat.Instance.EnemyProjectileContainer.transform);
        GameObject ProjectileInstance = SimplePool.Spawn(EnemyProjectile, transform.position + offset, Quaternion.identity, Combat.Instance.EnemyProjectileContainer);
        ProjectileInstance.GetComponent<EnemyProjectile>().Init();
      //  ProjectileInstance.GetComponent<Rigidbody>().velocity = transform.forward * projectileSpeed;
    }

    IEnumerator Cooldown(float resetTime)
    {
        yield return new WaitForSeconds(resetTime);
        onCooldown = false;
    }




}
