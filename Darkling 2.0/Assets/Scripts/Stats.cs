using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Stats : MonoBehaviour
{
    #region Singleton
    public static Stats Instance;

    private void Awake()
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

    #endregion

    public bool cheated = false;
    public bool newBestRun;
    public int health1regenAmount, health2regenAmount;
    public int healthBonus1 = 5, healthBonus2 = 10;
    public float shieldBonus1 = 0.25f, shieldBonus2 = 0.25f;
    public float demonGloveMoveSpeed = 1.00f;
    public float moveSpeed1Amount = 0.15f;                          // from upgrades
    public float moveSpeed2Amount = 0.15f;                          // from upgrades
    public float speedPowerupBoost = 1f;                            // from powerups 
    //public float jumpBoost = 1.25f;
    public float healthBarInset = 10f;
    public int currentHP, maxHP;
    public int baseMaxHP = 120;
    public int minimumHPAllowed = 5;
    PlayerCharacter player;
    public bool invulnerable, speedUp;
    public float speedUpAmount = 1.25f;
    public float hpRegenRate = 1f;
    float hpRegenTimer;
    float pickupTimer;
    public float invulnerableDuration, speedupDuration = 10f;
    public int maxHealthCap = 500;

    public float healthPickupDropRateA = 0.025f, healthPickupDropRateB = 0.015f;

    public int healthPickupFlatAmount = 30;

    public int lifeSpent;

    public float shotsFired, shotsHit;
    public float accuracy;

    [Header("Upgrades")]
    public bool hasHealth1;
    public bool hasHealth2,
        hasJump1,
        hasJump2,
        hasRegen1,
        hasRegen2,
        hasDoubleGrenades,
        hasDemonGlove,
        hasShield1,
        hasShield2,
        hasSpeed1,
        hasSpeed2,
        hasShot1,
        hasShot2 = false;

    [HideInInspector] public bool regen;

    [Header("Misc")]
    public int kills;
    public float killsPerMinute;  // round these to 2 decimal places
    public int deaths;

    // Health bar colors
    Color healthy = new Color(170, 0, 0);
    Color damaged = new Color(120, 100, 0);

    // Clock
    double startTime;
    public static float clock;
    public bool clockRunning;
   // [HideInInspector]
    public CharacterControls fpsController;
    KeyCombo fullUpgradeCode;

    void Start()
    {
        player = PlayerRef.Instance.player;
        //fpsController = player.GetComponent<FirstPersonController>();
        fpsController = player.GetComponent<CharacterControls>();

        fullUpgradeCode = new KeyCombo(new KeyCode[] { KeyCode.L, KeyCode.D, KeyCode.Alpha4, KeyCode.Alpha4 });
    }

    public void Init()
    {
        clock = 0;

        healthRectTransform = HUD.Instance.HealthBar.GetComponent<RectTransform>();

        hpRegenTimer = hpRegenRate;
        currentHP = maxHP = baseMaxHP;

        // Set initial healthbar size
        newHealthBarSize = maxHP * 3.5f;
        ResizeHealthBar(newHealthBarSize);

        kills = 0;
        killsPerMinute = 0;
        lifeSpent = 0;
        shotsFired = shotsHit = 0;
        accuracy = 0;

        hasHealth1 =
        hasHealth2 =
        hasJump1 =
        hasJump2 =
        hasRegen1 = 
        hasRegen2 = 
        hasDoubleGrenades = 
        hasDemonGlove = 
        hasShield1 =
        hasShield2 =
        hasSpeed1 =
        hasSpeed2 =
        hasShot1 = 
        hasShot2 = false;

        newBestRun = false;
        cheated = false;

        // Reset death post FX
        HUD.Instance.effectsVolume_From.weight = 1;
        HUD.Instance.effectsVolume_To.weight = 0;

        UserController.Instance.activeUser.userData.currentKills = 0;
        UserController.Instance.activeUser.userData.bestKills = 0;

    }

    private void Update()
    {
        if (clockRunning)
        {
            CalculateAccuracy();
            CalculateKPM();
        }

        if (fullUpgradeCode.Check())
        {
            StartCoroutine(HUD.Instance.ShowMessage("Cheat Enabled", Color.red, 36, 3f));
            StartCoroutine(HUD.Instance.ShowMessageCenter("Fully Upgraded", Color.red, 100, 3f));
            hasHealth1 =
            hasHealth2 =
            hasJump1 =
            hasJump2 =
            hasRegen1 =
            hasRegen2 =
            hasDoubleGrenades =
            hasDemonGlove =
            hasShield1 =
            hasShield2 =
            hasSpeed1 =
            hasSpeed2 =
            hasShot1 =
            hasShot2 = true;
            cheated = true;

            UpgradeController.Instance.GrantDemonGlove();
        }

        pickupTimer += Time.deltaTime;
        
        if (clockRunning)
            clock += Time.deltaTime;

        if (hasRegen1 || hasRegen2) HPRegen();

        if (currentHP > maxHP) currentHP = maxHP;

        if (currentHP <= 0)
        {
            currentHP = 0;
            Death();
        }

     
    }

    public void CalculateAccuracy()
    {
        if (shotsFired > 0)
        {
            accuracy = shotsHit / shotsFired * 100f;

        }
    }


    public void CalculateKPM()
    {
        killsPerMinute = (kills / (clock / 60f));
    }

    public void GainHP(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP)
            currentHP = maxHP;

    }

    RectTransform healthRectTransform;


    void ResizeHealthBar(float size)
    {
        healthRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, healthBarInset, size);

    }

    float newHealthBarSize;

    public void ChangeMaxHP(int amount)
    {
        maxHP = amount;
        newHealthBarSize = maxHP * 4;
        ResizeHealthBar(newHealthBarSize);
    }

    public void TakeDamage(float damage, bool ignoreInvulnerability)
    {
        if (player == null) print("Stats: player null");
        if (player != null)
        {
            AudioManager.Instance.Play("Take Damage");
            currentHP -= (int)(damage);

            StartCoroutine(HUD.Instance.ScreenFlashRed(0.1f));
            StartCoroutine(HUD.Instance.HealthBarFlash(MaterialRef.Instance.red, 0.1f));

        //    LowHealthTest();

            // Player death
            if (currentHP <= 0 && !player.dead)
                StartCoroutine(Death());
        }
    }

    int bonus1, bonus2;

    // Collectible
    public void PickupCollectible(Collectible collectible)
    {
        switch (collectible.type)
        {
            case Collectible.Type.Health:
                var healthPickupBonus = 0;

                if (currentHP == maxHP)
                    return;

                else
                {
                    if (hasHealth1) bonus1 = healthBonus1;
                    if (hasHealth2) bonus2 = healthBonus2;
                    PickupHealth(player.pickupDelay, healthPickupFlatAmount + healthPickupBonus + bonus1 + bonus2);
                }

                break;

            case Collectible.Type.HPMax:
                if (maxHP <= maxHealthCap)
                {
                    AudioManager.Instance.Play("Collectible");
                    ChangeMaxHP(maxHP + collectible.value);
                }
                else
                    HUD.Instance.ShowMessage("Max Life reached.", Color.red, 36, 3f);
                break;

            case Collectible.Type.Invulnerable:
                AudioManager.Instance.Play("Collectible");
                StopCoroutine(Invulnerable());   // so we can refresh effect if already applied
                StartCoroutine(Invulnerable());
                break;

            case Collectible.Type.SpeedUp:
                AudioManager.Instance.Play("Collectible");
                StopCoroutine(SpeedUp());   // so we can refresh effect if already applied
                StartCoroutine(SpeedUp());
                break;

            default:
                break;
        }

        AudioManager.Instance.Play("Collectible");
        StartCoroutine(HUD.Instance.ScreenFlashWhite(0.2f));
        Destroy(collectible.gameObject);

    }


    IEnumerator Invulnerable()
    {
        invulnerable = true;
        HUD.Instance.invulnerableText.gameObject.SetActive(true);
        HUD.Instance.InvulnerabilityTint.gameObject.SetActive(true);
        StartCoroutine(HUD.Instance.ShowMessage("INVULNERABLE for " + invulnerableDuration + " sec", Color.cyan, 36, 3f));
        yield return new WaitForSeconds(invulnerableDuration);
        invulnerable = false;
        HUD.Instance.invulnerableText.gameObject.SetActive(false);
        HUD.Instance.InvulnerabilityTint.gameObject.SetActive(false);
    }


    IEnumerator SpeedUp()
    {
        speedUp = true;
        speedPowerupBoost = 1.50f;
        HUD.Instance.speedBoostText.gameObject.SetActive(true);
        StartCoroutine(HUD.Instance.ShowMessage("INCREASED SPEED for " + speedupDuration + " sec", Color.yellow, 36, 3f));
        yield return new WaitForSeconds(speedupDuration);
        speedPowerupBoost = 1f;
        speedUp = false;
        HUD.Instance.speedBoostText.gameObject.SetActive(false);
    }


    public void PickupHealth(float delay, int healthPickupValue)
    {
        AudioManager.Instance.Play("Health Pickup");
        GainHP(healthPickupValue);
    }


    int regenAmount;

    void HPRegen()
    {
        if (!player.dead)
        {
            if (hasRegen1) regenAmount = health1regenAmount;
            if (hasRegen2) regenAmount = health2regenAmount;

            hpRegenTimer -= Time.deltaTime;
            if (hpRegenTimer <= 0)
            {
                GainHP(regenAmount);
                hpRegenTimer = hpRegenRate;
            }
        }
    }

    //void LowHealthTest()
    //{
    //    // Low health test
    //    float curHP = currentHP;
    //    float mHP = maxHP;
    //    var injuryPercent = curHP / mHP;

    //    if (injuryPercent <= lowHealthPercent)
    //        lowHealth = true;
    //    else
    //        lowHealth = false;

    //    if (lowHealth && !player.dead)
    //    {
    //        // damageTint.SetActive(true);
    //        // HUD.Instance.healthBarFill.color = damaged;
    //    }
    //    else
    //    {
    //        // HUD.Instance.healthBarFill.color = healthy;
    //        // damageTint.SetActive(false);
    //    }
   // }

    // Player death
    IEnumerator Death()
    {

        // Save current stats in activeUser.statsThisRun
        UserController.Instance.SetCurrentStats();

        //SpawnerController.Instance.enabled = false;
        //WaveController.Instance.enabled = false;
        SpawnerController.Instance.spawning = false;
        WaveController.Instance.waveInProgress = false;
        WaveController.Instance.countingDownToWaveStart = false;
        fpsController.enabled = false;
        GunShake.Instance.shake = false;
        player.spriteRenderer.enabled = false;
        player.dead = true;
        player.canMove = false;
        player.inputSuspended = true;
        //AudioManager.Instance.Play("Start");
        AudioManager.Instance.Play("Metal Flex");
        StartCoroutine(HUD.Instance.ShowMessage(("You Died..."), Color.red, 100, 3f));
        HUD.Instance.EffectsFade(HUD.Instance.effectsVolume_From, HUD.Instance.effectsVolume_To, 0.25f);
        yield return new WaitForSecondsRealtime(4f);
        //StartCoroutine(HUD.Instance.ShowMessageCenter(("Press R to Restart"), Color.yellow, 100, 3f));
        currentHP = 0;
        regen = false;
        deaths++;

        // Compare / replace stats if they're better
        UserController.Instance.CheckForBest();

        // Display post mortem report
        GameManager.Instance.ShowPostMortemPanel();
    }



    // Total play Clock
    void UpdateClock()
    {

        clock += Time.deltaTime;
        //HUD.Instance.clockText.text = TimeFormat(clock).ToString(); 

    }

    public string TimeFormat(float clock)
    {
        int minutes = Mathf.FloorToInt(clock / 60F);
        int seconds = Mathf.FloorToInt(clock - minutes * 60);
        string formattedTime = string.Format("{0:0}:{1:00}", minutes, seconds);

        return formattedTime;
    }


}
