using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour {

    public float lifeSpan;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, lifeSpan);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
