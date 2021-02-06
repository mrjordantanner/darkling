using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;

public class EnemyCharacter : MonoBehaviour
{
    public int currentHP, maxHP;
    public int contactDamage;

    public void TakeDamage(float damage, bool ignoreInvulnerability)
    {
        currentHP -= (int)(damage);
        if (currentHP <= 0)
            EnemyDeath();

        if (ignoreInvulnerability)
            StartHitFlash();

    }

    bool playerContact;
    float playerContactTimer;
    public float contactDamageInterval = 0.25f;

  //  public GameObject EnemyGraphics;
    public Animator anim;

    public bool canMove = true;
    public bool dealContactDamage = false;
    public bool enemyFrozen = false;
    public bool invulnerable;
    public string immunity;

    [Header("Drops")]
    public GameObject HealthPickup;
    public float healthDropRate;
    public GameObject LootA;
    public float lootADropRate;
    public GameObject LootB;
    public float lootBDropRate;
    public GameObject LootC;
    public float lootCDropRate;

    // This is probably a better way to handle audio, 
    // have references to the sounds on the objects that make them
    // All sounds actually "stored" in the AudioManager
   // [Header("Enemy Audio")]
   // public string moveSound;
   // public string attackSound;
   // public string deathSound;

    [HideInInspector]
    public Material currentMaterial;

    [Header("Prefabs")]
    public GameObject[] Explosions;

    [HideInInspector]
    public SpriteRenderer spriteRenderer;

    PlayerCharacter player;
    CapsuleCollider ContactTrigger;
  //  [HideInInspector]
   // Renderer renderer;
    Rigidbody rb;

    // Hit flash
    [HideInInspector]
    public bool hitFlash = false;
    float flashTimer = 0f;
    float flasher = 0f;
    float flashRate = 0.05f;
    float flashDuration = 0.3f;

    [HideInInspector]
    public bool isColliding;

    void Start()
    {
        ContactTrigger = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
      //  renderer = EnemyGraphics.GetComponent<Renderer>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();

        Init();


    }

    public void Init()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        spriteRenderer.material = MaterialRef.Instance.normal;
        currentMaterial = MaterialRef.Instance.normal;
        flashTimer = flasher = 0f;
        player = PlayerRef.Instance.player;
        hitFlash = false;
        currentHP = maxHP;
    }


    void Update()
    {

        if (playerContact) ContactDamageTimer();

        isColliding = false;

        // Hit FLash
        if (hitFlash && (flashTimer < flashDuration))
        {
            flashTimer += Time.deltaTime;
            flasher += Time.deltaTime;

            if (flasher >= flashRate)
            {
                flasher = 0;
                HitFlash();
            }
        }

        // stop flashing
        if (flashTimer >= flashDuration && !enemyFrozen)
        {
            flashTimer = 0;
            flasher = 0;
            hitFlash = false;
            spriteRenderer.material = MaterialRef.Instance.normal;
            currentMaterial = MaterialRef.Instance.normal;
        }
        else if (flashTimer >= flashDuration && enemyFrozen)
        {
            flashTimer = 0;
            flasher = 0;
            hitFlash = false;
            spriteRenderer.material = MaterialRef.Instance.frozen;
            currentMaterial = MaterialRef.Instance.frozen;
        }



    }


    void HitFlash()
    {
        if (currentMaterial == MaterialRef.Instance.normal)
        {
            spriteRenderer.material = MaterialRef.Instance.red;
            currentMaterial = MaterialRef.Instance.red;
        }
        else
        {
            spriteRenderer.material = MaterialRef.Instance.normal;
            currentMaterial = MaterialRef.Instance.normal;

        }

    }

    public void StartHitFlash()
    {
        hitFlash = true;
    }

    public void EnemyDeath()
    {
        AudioManager.Instance.Play("Enemy Death");
        ExplosionSingle();

        SimplePool.Despawn(gameObject);
        SpawnerController.Instance.totalEnemies--;
        Stats.Instance.kills++;
        WaveController.Instance.killsThisWave++;
        WaveController.Instance.KillQuotaCheck();

        CalculateDrops();


    }

    void ExplosionSingle()
    {
        if (Explosions.Length > 0)
        {
            foreach (var Explosion in Explosions)
            {
                var explosionInstance = Instantiate(Explosion, transform.position, Quaternion.identity);
                explosionInstance.transform.SetParent(Combat.Instance.VFXContainer.transform);
            }
        }
    }


    void Explode()
    {
        foreach (var Explosion in Explosions)
        {
            Vector2 randomPos = new Vector2(transform.position.x + Random.Range(-1f, 1f), transform.position.y + Random.Range(-1f, 1f));
            var explosion = Instantiate(Explosion, randomPos, Quaternion.identity);
            explosion.transform.SetParent(Combat.Instance.VFXContainer.transform);
        }
    }

    float tempHealthPickupDropRate;

    public void CalculateDrops()
    {
        var healthDropRoll = Random.value;
        //if (healthDropRoll <= healthDropRate && HealthPickup != null)

        // If Wave 10 or above, reduce drop rate of green health orbs
        if (WaveController.Instance.currentWave < 10) tempHealthPickupDropRate = Stats.Instance.healthPickupDropRateA;
        else tempHealthPickupDropRate = Stats.Instance.healthPickupDropRateB;

        if (healthDropRoll <= tempHealthPickupDropRate && HealthPickup != null)
            Instantiate(HealthPickup, transform.position, Quaternion.identity);

        var dropLootA = Random.value;
        if (LootA != null && dropLootA <= lootADropRate)
        {
             Instantiate(LootA, transform.position, Quaternion.identity);
        }

        var dropLootB = Random.value;
        if (LootB != null && dropLootB <= lootBDropRate)
        {
            Instantiate(LootB, transform.position, Quaternion.identity);
        }

        var dropLootC = Random.value;
        if (LootC != null && dropLootC <= lootCDropRate)
        {
            Instantiate(LootC, transform.position, Quaternion.identity);
        }
    }


    void ContactDamageTimer()
    {
        playerContactTimer -= Time.deltaTime;
        if (playerContactTimer <= 0) playerContact = false;

    }

    void OnTriggerEnter(Collider other)
    {
        if (playerContact) return;

        if (other.gameObject.CompareTag("Player") && !player.invulnerable && !player.dead && !enemyFrozen && dealContactDamage)
        {
            playerContact = true;
            playerContactTimer = contactDamageInterval;
            Combat.Instance.PlayerHit(contactDamage);

            if (Stats.Instance.hasDemonGlove)
            {
                TakeDamage(100, false);
            }
        }

    }

    void OnTriggerStay(Collider other)
    {
        if (playerContact) return;

        if (other.gameObject.CompareTag("Player") && !player.invulnerable && !player.dead && !enemyFrozen && dealContactDamage)
        {
            playerContact = true;
            playerContactTimer = contactDamageInterval;
            Combat.Instance.PlayerHit(contactDamage);

            if (Stats.Instance.hasDemonGlove)
            {
                TakeDamage(100, false);
            }
        }

    }

}


