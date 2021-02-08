using System.Collections;
using UnityEngine;


public class EnemyProjectile : ProjectileBase
{

    [Header("Projectile Behavior")]
    public bool invulnerable = true;
    public bool collision = true;
    PlayerCharacter player;

    [Header("Seeker")]
    public bool seeker = false;
    public float seekerVelocity, seekerRotation;
    float sensitivity;
    public float homingSensitivity = 0.2f;
    public float initialHomingSensitivity = 1f;
    public Vector3 targetOffset = new Vector3(0, 1, 0);

    public float homingDelay = 1f;
    public bool homingActive = false;
    float homingTimer;


    public void Start()
    {
        Init();
    }

    public void Init()
    {
        Invoke("Despawn", lifespan);
        player = PlayerRef.Instance.player;
        homingTimer = homingDelay;
        homingActive = false;
        sensitivity = initialHomingSensitivity;
    }

    public void FixedUpdate()
    {

        if (seeker)
        {
            SeekTarget();
        }

        if (seeker && !homingActive)
        {
            homingTimer -= Time.deltaTime;

            if (homingTimer <= 0)
            {
                homingActive = true;
                sensitivity = homingSensitivity;
            }
        }

    }


    void Despawn()
    {
        SimplePool.Despawn(gameObject);
    }

    void SeekTarget()
    {

        Vector3 relativePos = player.transform.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(relativePos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, sensitivity);
        transform.Translate(0, 0, seekerVelocity * Time.fixedDeltaTime, Space.Self);

    }


    void OnTriggerEnter(Collider other)
    {
        // if (alreadyHit) return;

        if (other.gameObject.CompareTag("Player") && !player.invulnerable)
        {
            // alreadyHit = true;
            Combat.Instance.PlayerHit(BaseDamageMax);
            DestroyOnImpact();
        }

        if (other.gameObject.layer == 29 && collision)
        {
            DestroyOnImpact();
        }

        if (other.gameObject.CompareTag("Player Weapon"))
        {
            if (!invulnerable)
            {
                AudioManager.Instance.Play("Enemy Death");
                other.gameObject.GetComponent<PlayerProjectile>().DestroyOnImpact();
                DestroyOnImpact();
            }
        }

    }


}





