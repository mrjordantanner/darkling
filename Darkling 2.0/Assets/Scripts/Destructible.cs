using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour {

	
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player Weapon"))
        {
            Destroy(gameObject);
           // play destroy animation



        }



    }
}
