using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Health : MonoBehaviour
{
    // Base script for EnemyHealth and PlayerHealth
    // Holds HP
    // When a Damager makes contact with a Health script, it triggers an event
    // Can be told to take damage by 
    // Can be given health 

    public float Hp;// { get; set; }
    public float MaxHp;// { get; set; }
    public float BaseMaxHp;// { get; set; }

    public bool InitOnEnable = true;


    // Consolidate damage so we can produce less damageText objects
   // float damageWindowTimer, damageThisWindow, damageWindow = 0.3f;
   // bool hasBeenDamaged = false;

    public virtual void Start()
    {
        Init();
    }

    void OnEnable()
    {
        if (InitOnEnable)
            Init();
    }

    public void Init()
    {
        Hp = MaxHp = BaseMaxHp;

        //if (GetComponent<Enemy>() != null)
        //{
        //    isEnemy = true;
        //    enemy = GetComponent<Enemy>();
        //}
        
    }

    private void Update()
    {
       // if (hasBeenDamaged) HandleDamageWindowTimer();
    }

    public void GainHealth(float amount)
    {
        Hp += amount;

        if (Hp > MaxHp)
            Hp = MaxHp;

        OnGainHealth();

        //if (isEnemy)
        //    enemy.enemyUI.UpdateUI();
        //else
        //    HUD.Instance.UpdatePlayerUI();
    }

    public virtual void OnGainHealth() { }


    //void HandleDamageWindowTimer()
    //{
    //    damageWindowTimer -= Time.deltaTime;

    //    if (damageWindowTimer <= 0)
    //    {
    //        if (damageThisWindow > 0)
    //        {
    //           // enemy.enemyUI.CreateDamageText(damageThisWindow);
    //        }

    //        damageThisWindow = 0;
    //        damageWindowTimer = damageWindow;
    //    }
    //}


    //public virtual void TakeDamage(float damage)
    //{
    //    Hp -= damage;
    //    DeathCheck();
    //}

    public virtual void OnHit(Damager damager) { }


    public void DeathCheck()
    {
        if (Hp <= 0)
        {

            if (GetComponent<DestructibleTimed>() != null)
                GetComponent<DestructibleTimed>().Destroy();

            OnDeath();
        }
    }

    public virtual void OnDeath() { }


}


