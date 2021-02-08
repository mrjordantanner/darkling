using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveToAttackPosition: MonoBehaviour
{
    EnemyAttackController attackController;
    NavMeshAgent agent;
    EnemyCharacter enemy;
    PlayerCharacter player;

    public void Start()
    {
        attackController = GetComponentInChildren<EnemyAttackController>();
        agent = GetComponentInParent<NavMeshAgent>();
        enemy = GetComponentInParent<EnemyCharacter>();
        player = PlayerRef.Instance.player;       

    }

    private void Update()
    {
        // If already attacking, do nothing here
        //  if (attackController.attackCycleActive)
        //      return;

        if (enemy.canMove && agent.isOnNavMesh)// && enemy.grounded)
        {
            agent.SetDestination(player.transform.position);
        }

        if (!agent.isOnNavMesh)
        {
            Destroy(enemy.gameObject);
            SpawnerController.Instance.totalEnemies--;
        }

    }



}
