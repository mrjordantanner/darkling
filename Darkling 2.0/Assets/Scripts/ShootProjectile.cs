using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

public class ShootProjectile : MonoBehaviour {

  //  public enum FirePattern {DummyTimed, LockTimed, LockProximity}

    public GameObject EnemyProjectile;
    EnemyProjectile enemyProjectile;

    EnemyCharacter enemy;
    PlayerCharacter player;

    // public GameObject Log;
    // public Log log;
    public int baseDamage = 65;
    public int damage;

    public bool targetPlayer = false;
    public bool targetTracking = false;      // targets last player position before firing
    public bool constantTracking = false;   // targets current player position during firing
    bool shouldShoot = false;
    public float startDelay;
    Transform[] firingPoints;
    public Transform target;
    GameObject ProjectileInstance;
    public Vector2 velocity;
    public Vector2 offset = new Vector2(0.0f, 0.0f);
    public float firingTimer = 0f;
    float shooter;
    public float firingRate = 0.1f;
    public float firingDuration = 1f;
    public float cooldown = 2.5f;
    public float projectileSpeed = 6.0f;
    public float projectileLifespan = 3f;
    public float projectileRotation = 0f;

    EnemyProjectile projectile;

    void Start ()
    {

        damage = baseDamage;
        player = FindObjectOfType<PlayerCharacter>();

        //  enemy = GetComponentInParent<EnemyCharacter>();
        StartCoroutine(StartDelay());

    }

    IEnumerator StartDelay()
    {
        yield return new WaitForSecondsRealtime(startDelay);
        StartCoroutine(Cooldown(cooldown));
    }

	void Update ()

    {
        if (targetPlayer && (targetTracking || constantTracking))
            UpdateTarget();

        /*if (shouldShoot && !enemy.enemyFrozen)
        {
            SingleTimedProjectile();
            shouldShoot = false;
            StartCoroutine(Cooldown(cooldown));
        }
        */

        if (shouldShoot) // && !enemy.enemyFrozen) 
        {
            targetTracking = false;  // stop tracking during firing process


            if (shouldShoot && (firingTimer < firingDuration))
            {
                firingTimer += Time.deltaTime;
                shooter += Time.deltaTime;

                if (shooter >= firingRate)
                {
                    shooter = 0;
                    FireProjectile();

                }
                if (firingTimer >= firingDuration)  // firing cooldown and reset
                {
                    firingTimer = 0;
                    shooter = 0;
                    shouldShoot = false;
                    targetTracking = true;
                    StartCoroutine(Cooldown(cooldown));
                }
            }
        }


    }

    void FireProjectile()
    {
        GameObject ProjectileInstance = Instantiate(EnemyProjectile, (Vector2)transform.position + offset * transform.localScale.x, Quaternion.identity);
        ProjectileInstance.GetComponent<Rigidbody2D>().velocity = transform.right * projectileSpeed;

      //  projectile = ProjectileInstance.GetComponent<EnemyProjectile>();
       // projectile.damage = Mathf.RoundToInt(baseDamage * Mathf.Pow(Difficulty.Instance.enemyDamage, Difficulty.Instance.difficultyLevel));
      //  projectile.rotation = projectileRotation;
      //  projectile.lifespan = projectileLifespan;
       // projectile.selfDestruct = true;
    }


    // to be called from Update when the target is being tracked
    void UpdateTarget()    
    {
        target = player.gameObject.transform;
        transform.right = new Vector3(target.position.x - transform.position.x, target.position.y - transform.position.y, 0);
        //Debug.DrawRay(transform.position, target.position, Color.yellow);
    }

    IEnumerator Cooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        shouldShoot = true;
    }

}
