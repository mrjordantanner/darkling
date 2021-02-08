using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour
{
    // Physics-based explosion on impact, except if it hits a wall 
    // or ceiling before 'minimumLifeTime' seconds

    // Collision-based bounce approach won't work on it's own
    // needs to probably be based on the
    // projectile's velocity and/or contact hit points in order to determine 
    // whether to bounce or not

   // public float groundCollisionOffset = 2f;
    public string explosionSound;

    public float cameraShakeStrength, cameraShakeDuration;
    public bool grenadeBehavior;
    public int aoeDamage;
    public float minimumLifeTime = 3f;   // for bouncing grenades, etc
    float lifeTime = 0;

   // [Header("Physics")]
   // public float blastRadius = 10f;
   // public float blastForce = 12f;
   // public float blastLift = 6f;
    public GameObject ExplosionPrefab;
   // public Vector3 explosionOffset;
    AreaOfEffect aoe;

    void Start()
    {
        aoe = ExplosionPrefab.GetComponentInChildren<AreaOfEffect>();
        if (aoe != null && aoe.damage >  0)
            aoe.damage = aoeDamage;

    }

    void Update()
    {
        lifeTime += Time.deltaTime;
    }



private void OnCollisionEnter(Collision collision)
{

        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Enemy"))
        {
            Explosion();
        }

        if (grenadeBehavior && lifeTime >= minimumLifeTime)
        {
            Explosion();
        }

        if (collision.gameObject.layer == 29)
        {
            Explosion();
        }

}


    private void OnTriggerEnter(Collider collider)
    {

        if (collider.gameObject.CompareTag("Ground") || collider.gameObject.CompareTag("Enemy"))
        {
            Explosion();
        }

        if (grenadeBehavior && lifeTime >= minimumLifeTime)
        {
            Explosion();
        }

        if (collider.gameObject.layer == 29)
        {
            Explosion();
        }

    }


void Explosion()
{
    // Instantiate Explosion VFX
    Instantiate(ExplosionPrefab, transform.localPosition, Quaternion.identity, Combat.Instance.VFXContainer.transform);
   // SimplePool.Spawn(ExplosionPrefab, transform.localPosition, Quaternion.identity, Combat.Instance.VFXContainer);
    CameraShake.Instance.Shake(cameraShakeStrength, cameraShakeDuration);
    AudioManager.Instance.Play(explosionSound);

        /*
    // Apply physics forces
    Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);
    foreach (var coll in colliders)
    {
        var rb = coll.GetComponent<Rigidbody>();
        if (rb != null)
            coll.GetComponent<Rigidbody>().AddExplosionForce(blastForce, transform.position, blastRadius, blastLift);
    }
    */

    Destroy(gameObject);
    // SimplePool.Despawn(gameObject);
   }


}
