using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    public string impactSound;

    [Header("Damage")]
    public float BaseDamageMax;

    [Header("Behavior")]
    public bool piercing = false;
    public bool obstacleCollision = true;
    public bool canHitProjectiles = false;
    public Vector3 velocity;
    public float rotation, lifespan = 5f;

    [HideInInspector]
    public GameObject CurrentTarget, PreviousTarget;
    [HideInInspector]
    public EnemyCharacter enemy;
    [HideInInspector]
    public Rigidbody rb;

    public Vector3 explosionOffset;
    public GameObject[] Explosions;

    void Update()
    {
        PreviousTarget = null;
    }


    public virtual void DestroyOnImpact()
    {
        if (Explosions.Length > 0)
        {
            // Spawn impact VFX
            foreach (var Explosion in Explosions)
            {
                var position = transform.position + explosionOffset;
                var explosionInstance = Instantiate(Explosion, position, Quaternion.identity, Combat.Instance.VFXContainer.transform);
               // var explosionInstance = SimplePool.Spawn(Explosion, position, Quaternion.identity, Combat.Instance.VFXContainer);
            }
        }

       // AudioManager.Instance.Play(impactSound);
        //Destroy(gameObject);
        SimplePool.Despawn(gameObject);
    }







}
