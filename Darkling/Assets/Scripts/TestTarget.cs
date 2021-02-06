using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTarget : MonoBehaviour
{
    public int hits;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player Weapon"))
        {
            hits++;
            print("hit");
        }

    }
}
