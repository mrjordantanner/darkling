using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;
using UnityEditor;

[ExecuteInEditMode]
public class EnemySpawner : MonoBehaviour
{
    public enum SpawnGroup { A, B, C }
    public SpawnGroup spawnGroup;

    public GameObject EnemyToSpawn;
    public float playerBufferRange = 10f;  // if random spawn point is within this distance of player, re-roll spawn point

    [Header("Position Randomization")]
    public float minRangeX = -10f;
    public float maxRangeX = 10f;
    public float minRangeY = -10f;
    public float maxRangeY = 10f;
    public float minRangeZ = -10f;
    public float maxRangeZ = 10f;

    public Color gizmoColor = Color.red;

    PlayerCharacter player;

    [HideInInspector]
    public EnemyCharacter[] enemyChildren;

    private void Start()
    {
        // player = PlayerRef.Instance.player;
        player = FindObjectOfType<PlayerCharacter>();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawLine(transform.position + Vector3.right * minRangeX, transform.position + Vector3.right * maxRangeX);
        Gizmos.DrawLine(transform.position + Vector3.up * minRangeY, transform.position + Vector3.up * maxRangeY);
        Gizmos.DrawLine(transform.position + Vector3.forward * minRangeZ, transform.position + Vector3.forward * maxRangeZ);

    }

    public void StartSpawn()
    {
        if (!SpawnerController.Instance.dontSpawn)
        {
            SpawnObject(EnemyToSpawn, 1);
        }
    }

    void GetChildren()
    {
        enemyChildren = GetComponentsInChildren<EnemyCharacter>();
    }

    /*
    Vector3 spawnPoint;

    void RaycastForSpawnPoint(Vector3 currentPosition)
    {

        RaycastHit hit;

        if (Physics.Raycast(currentPosition, -Vector3.up, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                print("Ground hit");
                spawnPoint = hit.transform.position;
            }
            else
            {
                print("Ground not hit");
                spawnPoint = currentPosition;
            }
        }


    }
    */


    public void DestroyChildren()
    {
        GetChildren();

        if (enemyChildren != null)
        {
            foreach (var enemy in enemyChildren)
            {
                if (enemy != null)
                {
                   // DestroyImmediate(enemy.gameObject);
                    SimplePool.Despawn(enemy.gameObject);
                    SpawnerController.Instance.totalEnemies--;
                }
            }
        }
    }

    void SpawnObject(GameObject NewObject, int iterations)
    {
        if (!SpawnerController.Instance.dontSpawn)
        {
            for (int i = 0; i < iterations; i++)
            {
               // spawnPoint = transform.position;  // default

                var offsetX = Random.Range(minRangeX, maxRangeX);
                var offsetY = Random.Range(minRangeY, maxRangeY);
                var offsetZ = Random.Range(minRangeZ, maxRangeZ);
                Vector3 offset = new Vector3(offsetX, offsetY, offsetZ);

                if (PlayerProximityCheck(offset))  // re-roll spawn point if too close too player
                {
                    i--;
                    print("Spawn point too close to player -- re-rolling");
                    continue;
                }

              //  RaycastForSpawnPoint(offset);

               // ObjectInstance = Instantiate(NewObject, transform.position + offset, Quaternion.identity, gameObject.transform);
                var ObjectInstance = SimplePool.Spawn(NewObject, transform.position + offset, Quaternion.identity, gameObject);
                ObjectInstance.GetComponent<EnemyCharacter>().Init();
                SpawnerController.Instance.totalEnemies++;
            }

        }
    }

    bool PlayerProximityCheck(Vector3 offset)
    {
        var heading = offset - player.transform.position;
        var distance = heading.magnitude;
        var direction = heading / distance; // This is now the normalized direction.
        if (heading.sqrMagnitude < playerBufferRange * playerBufferRange)
        {
            return true;  // player is too close
        }

        return false;  // player is not too close
    }


    private void OnApplicationQuit()
    {
        SpawnerController.Instance.dontSpawn = true;
        DestroyChildren();
    }

}

