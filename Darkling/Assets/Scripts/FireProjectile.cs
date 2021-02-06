using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    // Fires a (probably) physics-based projectile from a transform position
    // Dictates fire velocity

    public bool isSpecial;
    public Transform[] specialFiringPoints;
    public Transform[] specialFiringPoints2;
    public int healthCost;
    public KeyCode fireButton;
    public GameObject Projectile;
    public float fireRate;
    public float velocity;
    public Transform firingPoint;
    bool canShoot;
    Camera cam;
    //public Vector3 offset;
    public Transform secondFiringPoint, thirdFiringPoint;
    Animator anim;
    PlayerCharacter player;

    void Start()
    {
        canShoot = true;
        cam = GetComponentInParent<Camera>();
        cam = Camera.main;
        anim = GetComponentInChildren<Animator>();
        player = PlayerRef.Instance.player; 

    }


    void Update()
    {
        // Special Attack
        if (isSpecial)
        {
            if (Input.GetKeyDown(fireButton) && canShoot && !player.dead)
            {
                if (Stats.Instance.currentHP >= healthCost + 5)// || Stats.Instance.hasLifeCostRemoved)
                {
                    UseSpecial();
                }
                else
                {
                    StartCoroutine(HUD.Instance.ShowMessage("Not Enough Life to Use", Color.red, 28, 2f));
                    return;
                }
            }
        }

        // Rapid Fire Attack
        else if (Input.GetKey(fireButton) && canShoot && !GameManager.Instance.gamePaused && !player.dead)
            RapidFire();

        if (Input.GetKeyUp(fireButton) && !GameManager.Instance.gamePaused && !player.dead)
        {
            anim.ResetTrigger("StartAttacking");
            anim.SetBool("Attacking", false);
            GunShake.Instance.shake = false;
        }
    }


    void UseSpecial()
    {
        StartCoroutine(FireCooldown());

       // if (!Stats.Instance.hasLifeCostRemoved)
      //  {
            Stats.Instance.currentHP -= healthCost;
            if (Stats.Instance.currentHP <= 0) Stats.Instance.currentHP = 1;
      //  }

        AudioManager.Instance.Play("Fire Special Weapon");
        anim.SetTrigger("StartAttacking");
       // anim.SetBool("Attacking", true);

        foreach (var firingPoint in specialFiringPoints)
        {
            ProjectileGameObject = Instantiate(Projectile, firingPoint.position, Quaternion.identity);
            ProjectileGameObject.GetComponent<Rigidbody>().velocity = velocity * cam.transform.forward;
        }


        if (Stats.Instance.hasDoubleGrenades)
        {
            foreach (var firingPoint in specialFiringPoints2)
            {
                ProjectileGameObject = Instantiate(Projectile, firingPoint.position, Quaternion.identity);
                ProjectileGameObject.GetComponent<Rigidbody>().velocity = velocity * cam.transform.forward;
            }
        }

    }


    GameObject ProjectileGameObject;

    void RapidFire()
    {
        Stats.Instance.shotsFired++;
        StartCoroutine(FireCooldown());
        GunShake.Instance.shake = true;
        AudioManager.Instance.Play("Fire Primary Weapon");
        anim.SetTrigger("StartAttacking");
        anim.SetBool("Attacking", true);

        // Single
       // ProjectileGameObject = Instantiate(Projectile, firingPoint.position, Quaternion.identity);
        ProjectileGameObject = SimplePool.Spawn(Projectile, firingPoint.position, Quaternion.identity, Combat.Instance.EnemyProjectileContainer);
        ProjectileGameObject.GetComponent<PlayerProjectile>().Init();
        ProjectileGameObject.GetComponent<Rigidbody>().velocity = velocity * cam.transform.forward;

       // if (!Stats.Instance.hasShot1) return;

        // Double
        if (Stats.Instance.hasShot1)
        {
            ProjectileGameObject = SimplePool.Spawn(Projectile, secondFiringPoint.position, Quaternion.identity, Combat.Instance.EnemyProjectileContainer);
            ProjectileGameObject.GetComponent<PlayerProjectile>().Init();
            ProjectileGameObject.GetComponent<Rigidbody>().velocity = velocity * cam.transform.forward;
        }

       // if (!Stats.Instance.hasShot2) return;

        // Triple
        if (Stats.Instance.hasShot2)
        {
            ProjectileGameObject = SimplePool.Spawn(Projectile, thirdFiringPoint.position, Quaternion.identity, Combat.Instance.EnemyProjectileContainer);
            ProjectileGameObject.GetComponent<PlayerProjectile>().Init();
            ProjectileGameObject.GetComponent<Rigidbody>().velocity = velocity * cam.transform.forward;
        }
    }

    IEnumerator FireCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(fireRate);
        canShoot = true;

    }
}
