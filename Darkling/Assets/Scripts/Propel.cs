using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propel : MonoBehaviour
{
    public Vector3 vector;
    public float velocity;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.AddForce(vector * velocity * Time.fixedDeltaTime);

    }


}
