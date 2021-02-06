using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDespawn : MonoBehaviour
{
    public float lifeSpan;

    void Start()
    {
        Invoke("Despawn", lifeSpan);
    }

    void Despawn()
    {
        SimplePool.Despawn(gameObject);
    }
}
