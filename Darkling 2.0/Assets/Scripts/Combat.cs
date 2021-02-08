using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    #region Singleton
    public static Combat Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    #endregion


    [HideInInspector]
    public PlayerCharacter player;

    public GameObject FloatingTextContainer, VFXContainer, EnemyProjectileContainer;

    public void Start()
    {
        player = PlayerRef.Instance.player;
    }

    

    // PLAYER HIT BY ENEMY OR ENEMY PROJECTILE
    public void PlayerHit(float baseDamageMax)
    {
        if (player.dead || player.respawning) return;

        if (player != null)
        {
            float shieldAmt1 = 0;
            float shieldAmt2 = 0;

            CameraShake.Instance.Shake(0.3f, 0.2f);
            StartCoroutine(GunShake.Instance.StartHitShake(0.2f));

            if (Stats.Instance.invulnerable) return;

            if (Stats.Instance.hasShield1) shieldAmt1 = Stats.Instance.shieldBonus1;
            if (Stats.Instance.hasShield1) shieldAmt2 = Stats.Instance.shieldBonus2;

            var damage = baseDamageMax * (1 - (shieldAmt1 + shieldAmt2));


            if (damage <= 0) damage = 1;
            Stats.Instance.TakeDamage(damage, false);
            AudioManager.Instance.Play("Player Hurt");
            //AudioManager.Instance.Play("Breathe");
        }
    }


    // PLAYER PROJECTILE HITS ENEMY
    public void EnemyHitByProjectile(PlayerProjectile projectile, EnemyCharacter enemy)
    {
        //  var damage = RandomizeValueWithinRange(projectile.BaseDamageMax, projectile.DamageRange);

        var damage = projectile.BaseDamageMax;

      //  if (Stats.Instance.hasDoubleDamage)
      //      damage *= 2;

        AudioManager.Instance.Play("Enemy Hit");
        DamageEnemy(damage, enemy);

    }



    public void DamageEnemy(float damage, EnemyCharacter enemy)
    {
        enemy.hitFlash = true;
        if (damage <= 0) damage = 1;
        enemy.TakeDamage(damage, true);

    }


    // Projectile struck by player weapon
    public void ProjectileHit(GameObject CurrentTarget)
    {
        var enemyProjectile = CurrentTarget.GetComponent<EnemyProjectile>();
        if (!enemyProjectile.invulnerable)
        {
            SimplePool.Despawn(CurrentTarget);
        }
    }

    public Vector3 GetPlayerDirection(Transform otherObject)
    {
        if (player == null) print("player null");
        var playerDirection = (player.transform.position - otherObject.position).normalized;
        return playerDirection;
    }

    // Used to roll for random value with a given base Value and range percentage, e.g. Roll within a min/max of 2% of 100 Base Value
    public float RandomizeValueWithinRange(float maxValue, float valueRange)
    {
        var value = Random.Range(maxValue * valueRange, maxValue);
        return value;
    }
}
