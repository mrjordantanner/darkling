using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleFollow : MonoBehaviour
{
    PlayerCharacter player;
   // EnemyCharacter enemy;
    public float rotationSpeed = 3.0f;
    public float moveSpeed = 5.0f;

    NavMeshAgent agent;

    void Start()
    {
        player = PlayerRef.Instance.player;
      //  enemy = GetComponent<EnemyCharacter>();
        agent = GetComponent<NavMeshAgent>();

    }

    void Update()
    {
        /*
        transform.rotation = Quaternion.Slerp(transform.rotation
                                              , Quaternion.LookRotation(player.transform.position - transform.position)
                                              , rotationSpeed * Time.deltaTime);

        transform.position += transform.forward * moveSpeed * Time.deltaTime;
        */

        agent.SetDestination(player.transform.position);
    }

}
