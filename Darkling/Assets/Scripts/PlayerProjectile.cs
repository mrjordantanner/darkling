using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : ProjectileBase
{

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        Invoke("DestroyOnImpact", lifespan);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            CurrentTarget = other.gameObject;
            
            // Saves on some getcomponent calls if firing at same enemy repeatedly
            if (CurrentTarget != PreviousTarget)
            {
                enemy = CurrentTarget.GetComponent<EnemyCharacter>();
            }

            Stats.Instance.shotsHit++;
            Combat.Instance.EnemyHitByProjectile(this, enemy);

            PreviousTarget = CurrentTarget;

            if (!piercing)
                DestroyOnImpact();

        }

        if (other.gameObject.CompareTag("EnemyProjectile") && canHitProjectiles)
        {
            CurrentTarget = other.gameObject;
            Stats.Instance.shotsHit++;
            Combat.Instance.ProjectileHit(CurrentTarget);

            if (!piercing)
                DestroyOnImpact();
        }

        if (other.gameObject.layer == 29)
        {
            DestroyOnImpact();
        }
    }

}
