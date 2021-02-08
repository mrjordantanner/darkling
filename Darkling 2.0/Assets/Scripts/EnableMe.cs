using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableMe : MonoBehaviour {

    public GameObject[] Object;


	void Start ()
    {

        int objects = Object.Length;
        for (int i = 0; i < objects; i++)
        {
            GameObject CurrentObject = Object[i];
            CurrentObject.SetActive(true);
        }




    }

}
