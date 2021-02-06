using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eyeball : MonoBehaviour
{
    public float speed, rotationSpeed, lifetime = 6f;
    public int aoeDamage;
    public GameObject Explosion;

    PlayerCharacter player;

    private void Start()
    {
        Invoke("Explode", lifetime);
        player = PlayerRef.Instance.player;
    }

    public void Explode()   
    {
        ExplosionSingle();
        gameObject.SetActive(false);
        Destroy(gameObject, 1f);

    }

    private void FixedUpdate()
    {
        Vector3 targetDirection = player.transform.position - transform.position;  // expensive?
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0F);
        transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    void ExplosionSingle()
    {
        var explosionInstance = Instantiate(Explosion, transform.position, Quaternion.identity);
        explosionInstance.transform.SetParent(Combat.Instance.VFXContainer.transform);
        var aoe = explosionInstance.GetComponentInChildren<AreaOfEffect>();
        if ( aoe != null)
        {
            aoe.damage = aoeDamage;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Player Weapon"))
        {
            Explode();
        }
    }
}
