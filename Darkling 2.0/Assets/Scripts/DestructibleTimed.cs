using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleTimed : MonoBehaviour {

    public float respawnTime = 3f;

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("PlayerWeapon"))
    //    {
    //        gameObject.SetActive(false);
    //        Invoke("SpawnObject", respawnTime);
    //    }

    //}


    public void Destroy()
    {
        Invoke("SpawnObject", respawnTime);
        gameObject.SetActive(false);
    }

    void SpawnObject()
    {
        gameObject.SetActive(true);
    }
}
