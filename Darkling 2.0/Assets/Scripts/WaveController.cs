using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveController : MonoBehaviour
{
    #region Singleton
    public static WaveController Instance;

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

    // Survive each timed wave to earn a chance to upgrade

    public bool waveInProgress = false;
    public int currentWave;
   // public int totalWaves;
    public float startingEnemySpawnRate = 0.5f, enemySpawnIncrease = 0.25f, maxEnemySpawnRate = 5f;
    public float startingTime = 30f, timeIncreasePerWave = 15f, maxTimePerWave = 120f;

    public int startingKillQuota = 10, currentKillQuota;
    public float killQuotaIncreaseA = 1.40f;
    public float killQuotaIncreaseB = 1.20f;
    public int killsThisWave;
    public bool killQuotaMet;

    SpawnerController spawnerController;

    public float currentWaveTimer;
    public float currentWaveDuration;
    public float restTime = 5f;
    public float waveStartCountdownTimer, secondsToCountdown;
    public bool countingDownToWaveStart = false;

    public Text currentWaveText;
    public Text waveTimerText, countdownToWaveStartText;
    public Text killsThisWaveText, killQuotaText, killQuotaLabel, killQuotaSlash;


     void Start()
     {
        spawnerController = FindObjectOfType<SpawnerController>();
     }

    void Update()
    {
        if (countingDownToWaveStart)
        {
            waveStartCountdownTimer -= Time.deltaTime;

            if (waveStartCountdownTimer <= 0)
            {
                StartWave();
                waveStartCountdownTimer = secondsToCountdown;
            }
        }

        if (waveInProgress)
        {
            countingDownToWaveStart = false;
            currentWaveTimer -= Time.deltaTime;
            
            if (currentWaveTimer <= 0 && killQuotaMet)
            {
                currentWaveTimer = 0;
                waveInProgress = false;
                StartCoroutine(EndWave());

            }
            else if (currentWaveTimer <= 0 && !killQuotaMet)
            {
                currentWaveTimer = 0;
            }

        }

        

    }

    public void Init()
    {
        currentWave = 0;
        killsThisWave = 0;
        waveInProgress = false;
        countingDownToWaveStart = false;
    }


    public void SetupNextWave()
    {
        //TODO: GameManager.Instance.mainLight.color = etc etc
        // and/or
        // GameManager.Instance.postColorGrading.temperature = Mathf.Random(0,100);

        SpawnerController.Instance.currentMaxEnemies = CalculateMaxEnemies();

        currentWave++;   // currentWave starts at 0, so this will be 1 the first time
        //SpawnerController.Instance.firstSpawnThisWave = true;
        currentWaveDuration = CalculateWaveDuration();
        currentWaveTimer = currentWaveDuration;
        waveStartCountdownTimer = secondsToCountdown;
        currentKillQuota = CalculateKillQuota();
        killsThisWave = 0;

        if (currentWave < 5)
        {
            SpawnerController.Instance.groupA = true;
            SpawnerController.Instance.groupB = false;
            SpawnerController.Instance.groupC = false;
        }
        else if (currentWave > 4 && currentWave < 10)
        {
            SpawnerController.Instance.groupA = true;
            SpawnerController.Instance.groupB = true;
            SpawnerController.Instance.groupC = false;
        }
        else if (currentWave > 9)
        {
            SpawnerController.Instance.groupA = true;
            SpawnerController.Instance.groupB = true;
            SpawnerController.Instance.groupC = true;
        }

        SpawnerController.Instance.GetSpawnRate();

        BeginCountdownToWaveStart();
       // StartCoroutine(ShowObjectives(3f));
    }

    //public IEnumerator ShowObjectives(float duration)
    //{
    //    waveInProgress = false;
    //    GameManager.Instance.objectivesLabel.enabled = true;
    //    GameManager.Instance.objectiveText_Time.text = "Survive for" + currentWaveDuration.ToString();
    //    GameManager.Instance.objectiveText_Kills.text = "Kill " + currentKillQuota.ToString() + " enemies";
    //    yield return new WaitForSeconds(duration);
    //    GameManager.Instance.objectivesLabel.enabled = false;
    //    GameManager.Instance.objectiveText_Time.text = "";
    //    GameManager.Instance.objectiveText_Kills.text = "";

    //    BeginCountdownToWaveStart();
    //}



    public void BeginCountdownToWaveStart()
    {
        waveInProgress = false;
        Stats.Instance.clockRunning = false;
        countingDownToWaveStart = true;
        StartCoroutine(HUD.Instance.ShowMessage("Wave " + currentWave + " starts in", Color.red, 36, secondsToCountdown + 1));

        killsThisWaveText.text = killsThisWave.ToString();
        killQuotaText.text = currentKillQuota.ToString();
        HUD.Instance.ShowKillQuotaUI();
    }

    void StartWave()
    {
        waveInProgress = true;
        Stats.Instance.clockRunning = true;
        countingDownToWaveStart = false;
        AudioManager.Instance.Play("Wave Complete");
        HUD.Instance.ClearMessage();

        // Clear countdown UI
        countdownToWaveStartText.text = "";

        // Start Enemy spawners
        SpawnerController.Instance.StartSpawnWave();

    }

     public IEnumerator EndWave()
     {
        AudioManager.Instance.ReduceMusicVolume();

        waveInProgress = false;
        Stats.Instance.clockRunning = false;
        // Show victory message
        StartCoroutine(HUD.Instance.ShowMessage("Wave " + currentWave + " complete", Color.red, 36, 3f));
        AudioManager.Instance.Play("Wave Complete");
        SpawnerController.Instance.spawning = false;

        // Destroy all enemies and enemy projectiles
        var allEnemies = FindObjectsOfType<EnemyCharacter>();
        foreach (var enemy in allEnemies)
        {
            Destroy(enemy.gameObject);
            SpawnerController.Instance.totalEnemies = 0;
        }

        var allProjectiles = FindObjectsOfType<EnemyProjectile>();
        foreach (var projectile in allProjectiles)
        {
            Destroy(projectile.gameObject);
        }

        // Destroy collectibles other than HPMax's
        //var allCollectibles = FindObjectsOfType<Collectible>();
        //foreach (var collectible in allCollectibles)
        //{
        //   if (collectible.type != Collectible.Type.HPMax) Destroy(collectible.gameObject);
        //}

        yield return new WaitForSeconds(restTime);

        HUD.Instance.HideKillQuotaUI();

        UserController.Instance.CheckForBest();   // upload to cloud after each wave if it's an improvement

        // Show upgrade screen
        UpgradeController.Instance.OpenUpgradeMenu();
       // UpgradeController.Instance.CheckAvailability();

     }

    // calc rate based on wave number
    public float CalculateSpawnRate()
    {
        float spawnRate;
        if (currentWave == 1)
            spawnRate = startingEnemySpawnRate;
        else
            spawnRate = (currentWave * enemySpawnIncrease) + enemySpawnIncrease;

        if (spawnRate > maxEnemySpawnRate) spawnRate = maxEnemySpawnRate;

        return spawnRate;

    }

    float CalculateWaveDuration()
    {
        float currentWaveDur;

        if (currentWave == 1)
            currentWaveDur = startingTime;
        else
            currentWaveDur = (currentWave * timeIncreasePerWave) + timeIncreasePerWave;

        if (currentWaveDur > maxTimePerWave) currentWaveDur = maxTimePerWave;

        return currentWaveDur;
    }

    // (starting * waveNumber * .05) + starting
    int CalculateMaxEnemies()
    {
        int currentMaxEnemies;

        if (currentWave == 1)
            currentMaxEnemies = SpawnerController.Instance.startingMaxEnemies;
        else
            currentMaxEnemies = Mathf.RoundToInt(SpawnerController.Instance.startingMaxEnemies * currentWave * SpawnerController.Instance.enemyMaxIncrease) + SpawnerController.Instance.startingMaxEnemies;

        return currentMaxEnemies;
    }


    public void KillQuotaCheck()
    {
        if (killsThisWave >= currentKillQuota) killQuotaMet = true;
        else killQuotaMet = false;
    }


    int CalculateKillQuota()
    {
        float curKillQuota;

        if (currentWave == 1)
            curKillQuota = startingKillQuota;
        else if (currentWave > 1 && currentWave < 10)
            curKillQuota = currentKillQuota * killQuotaIncreaseA;            // increase  by killQuotaA on waves 2 through 9
        else
            curKillQuota = currentKillQuota * killQuotaIncreaseB;           //  increase by killQuotaB on Wave 10 and beyond

        return (int)curKillQuota;
    }
}
