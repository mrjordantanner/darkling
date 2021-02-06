using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaOfEffect : MonoBehaviour
{
    SphereCollider sphereCollider;
    public int damage;
    public float lifeSpan = 0.1f;

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeSpan);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            var enemy = collision.GetComponent<EnemyCharacter>();
            //if (enemy.isColliding) return;
           // enemy.isColliding = true;
            Combat.Instance.DamageEnemy(damage, enemy);

        }

        if (collision.gameObject.CompareTag("EnemyProjectile"))
        {
            Destroy(collision.gameObject);

        }


    }

}