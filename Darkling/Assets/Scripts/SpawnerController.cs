using System.Collections.Generic;
using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class SpawnerController : MonoBehaviour
{
    #region Singleton
    public static SpawnerController Instance;

    private void Awake()
    {
        if (Application.isEditor)
            Instance = this;
        else
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

    }

    #endregion

    public int totalEnemies, startingMaxEnemies = 130, currentMaxEnemies;
    public float enemyMaxIncrease = 0.05f;

   // public List<EnemyCharacter> allActiveEnemies;
    public EnemySpawner[] enemySpawners;
    public List<EnemySpawner> spawnGroupA, spawnGroupB, spawnGroupC;
    public bool groupA, groupB, groupC;

    public bool spawning;
    public float spawnTimer, currentSpawnRate, currentSpawnFrequency;
    public bool dontSpawn;

   // float enemyAttackWaitTimeDecrease = 0.25f;

    void Start ()
    {
        spawnGroupA = new List<EnemySpawner>();
        spawnGroupB = new List<EnemySpawner>();
        spawnGroupC = new List<EnemySpawner>();

        // allActiveEnemies = new List<EnemyCharacter>();

        Init();
        GetSpawnRate();
    }

    public void Init()
    {
        groupA = true;
        groupB = groupC = false;
        currentMaxEnemies = startingMaxEnemies;
    }

    public void GetSpawnRate()
    {
        currentSpawnRate = WaveController.Instance.CalculateSpawnRate();
        currentSpawnFrequency = (1 / currentSpawnRate);
    }

    private void Update()
    {
        
        if (spawning)
        {
            spawnTimer -= Time.deltaTime;

            if (spawnTimer <= 0)
            {
                Spawn();
                spawnTimer = currentSpawnFrequency;
            }

        }

        if (WaveController.Instance.waveInProgress)
            EnemyMaxCheck();

    }


    void EnemyMaxCheck()
    {
        if (totalEnemies >= currentMaxEnemies)
        {
            dontSpawn = true;
        }
        else if (totalEnemies < currentMaxEnemies)
        {
            dontSpawn = false;
        }
    }


    public int CalculateMaxEnemies()
    {
        var max = (int)(startingMaxEnemies + (WaveController.Instance.currentWave * enemyMaxIncrease));
        return max;
    }



    // Begin the spawning cycle for the current wave
    public void StartSpawnWave()
    {
        spawning = true;

    }

    // End spawning for current wave
    public void EndSpawnWave()
    {
        spawning = false;

    }

    // Ping each enemySpawner to spawn 1 enemy
    public void Spawn()
    {
        ClearGroups();
        PopulateGroups();

        if (groupA)
        {
            foreach (var enemySpawner in spawnGroupA)
            {
                enemySpawner.StartSpawn();
            }
        }

        if (groupB)
        {
            foreach (var enemySpawner in spawnGroupB)
            {
                enemySpawner.StartSpawn();
            }
        }

        if (groupC)
        {
            foreach (var enemySpawner in spawnGroupC)
            {
                enemySpawner.StartSpawn();
            }
        }

        if (!groupA && !groupB && !groupC) print("No Spawn groups selected!");

    }

    public void PopulateGroups()
    {
        enemySpawners = FindObjectsOfType<EnemySpawner>();

        foreach (var spawner in enemySpawners)
        {
            switch (spawner.spawnGroup)
            {
                case EnemySpawner.SpawnGroup.A:
                    spawnGroupA.Add(spawner);
                    break;

                case EnemySpawner.SpawnGroup.B:
                    spawnGroupB.Add(spawner);
                    break;

                case EnemySpawner.SpawnGroup.C:
                    spawnGroupC.Add(spawner);
                    break;
            }
        }
    }

    // Should offset the start of the spawning cycle so they don't 
    // all spawn at exactly the same time the whole wave
    //public bool firstSpawnThisWave = true;

    //public void InitialSpawn()
    //{
    //    if (firstSpawnThisWave)
    //        RandomDelayFirstSpawn();
    //    else
    //        StartEnemySpawn();

    //}

    //IEnumerator RandomDelayFirstSpawn()
    //{
    //    firstSpawnThisWave = false;
    //    yield return new WaitForSeconds(Random.value);
    //    StartEnemySpawn();
    //}


    public void ClearGroups()
    {
        spawnGroupA.Clear();
        spawnGroupB.Clear();
        spawnGroupC.Clear();

    }

    public void DestroyEnemyChildren()
    {
        foreach (var enemySpawner in enemySpawners)
        {
            enemySpawner.DestroyChildren();
            ClearGroups();
        }
    }

    private void OnApplicationQuit()
    {
        DestroyEnemyChildren();
    }


}
