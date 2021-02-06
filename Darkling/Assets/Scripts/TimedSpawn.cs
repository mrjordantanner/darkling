using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSpawn : MonoBehaviour
{
    public bool shouldSpawn = true;
    public float spawnInterval = 5f;
    public GameObject ObjectToSpawn;
    float spawnTimer;
    public bool shootAtPlayer;
    public float velocity;

    PlayerCharacter player;

    void Start()
    {
        player = PlayerRef.Instance.player;
        SetTimer();
    }

    void SetTimer()
    {
        spawnTimer = spawnInterval;
    }

    void Update()
    {
        if (shouldSpawn)
        {
            spawnTimer -= Time.deltaTime;
            
            if (spawnTimer <= 0)
            {
                SpawnObject();
                SetTimer();
            }


        }
    }

    void SpawnObject()
    {
        GameObject ObjectInstance = Instantiate(ObjectToSpawn, transform.position, Quaternion.identity);

        if (shootAtPlayer)
        {
            Vector3 targetDirection = player.transform.position - transform.position;
            ObjectInstance.GetComponent<Rigidbody>().AddForce(targetDirection.normalized * velocity);
        }
    }
}
