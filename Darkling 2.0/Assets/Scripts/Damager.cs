using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damager : MonoBehaviour
{
    // Stores baseDamage stats
    // Detects collision with Health scripts and calls OnHit()
    // Parent class of Gun, ProjectileBase, and Weapon
    // Grandparent class of PlayerProjectile, EnemyProjectile, PlayerWeapon, and EnemyWeapon

    public float Damage;
    public float BaseDamage;
    // public float critMultiplier = 3f;
    [HideInInspector]
    public GameObject CurrentTarget, PreviousTarget;

    public virtual void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        Damage = BaseDamage;
    }

    public virtual void OnHit(Health health)
    {

    }

    //public virtual void DealDamage(float dmg, Health target)
    //{
    //    target.TakeDamage(this);
    //}

    //public virtual void DealDamage(float dmg, Health target, float critMultiplier)
    //{
    //    target.TakeDamage(dmg * critMultiplier);

    //}


    public virtual void OnTriggerEnter(Collider other)
    {
        var health = other.GetComponent<Health>();

        if (health != null)
        {
            OnHit(health);
            health.OnHit(this);
        }
    }






}
