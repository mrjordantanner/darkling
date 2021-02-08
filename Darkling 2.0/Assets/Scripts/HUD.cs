using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class HUD : MonoBehaviour {

    #region Singleton
    public static HUD Instance;

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

    [Header("UI Elements")]
    public Text currentHPText;
    public Text maxHPText;
    public Slider HealthBar;
    public Image healthBarFill;
    public Text activeUserText;

    [Header("Messages")]
    public Text message;
    public Text messageCenter;

    [Header("Screen Effects")]
    public GameObject FlashWhite;
    public GameObject FlashRed;
    public GameObject InvulnerabilityTint;

    public Text invulnerableText, speedBoostText;


    private void Start()
    {
        var healthRectTransform = HealthBar.GetComponent<RectTransform>();
        healthRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 10, Stats.Instance.maxHP * 4);
    }
    void Update ()
    {
        UpdateUI();

        if (effectsFade) {
            ProcessEffectsFade();
        }
    }


    public void UpdateUI()
    {
        currentHPText.text = Stats.Instance.currentHP.ToString();
        maxHPText.text = Stats.Instance.maxHP.ToString();
        HealthBar.maxValue = Stats.Instance.maxHP;
        HealthBar.value = Stats.Instance.currentHP;


        // Wave-related UI
        WaveController.Instance.currentWaveText.text = WaveController.Instance.currentWave.ToString();
        WaveController.Instance.waveTimerText.text = Stats.Instance.TimeFormat(WaveController.Instance.currentWaveTimer).ToString();

        // Countdown to wave start text
        if (WaveController.Instance.countingDownToWaveStart)
            WaveController.Instance.countdownToWaveStartText.text = Stats.Instance.TimeFormat(WaveController.Instance.waveStartCountdownTimer).ToString();

        // Kill quota UI
        if (WaveController.Instance.waveInProgress)
        {
            WaveController.Instance.killsThisWaveText.text = WaveController.Instance.killsThisWave.ToString();
            WaveController.Instance.killQuotaText.text = WaveController.Instance.currentKillQuota.ToString();
        }

    }

    public void ShowKillQuotaUI()
    {
        WaveController.Instance.killsThisWaveText.enabled = true;
        WaveController.Instance.killQuotaText.enabled = true;
        WaveController.Instance.killQuotaLabel.enabled = true;
        WaveController.Instance.killQuotaSlash.enabled = true;

    }

    public void HideKillQuotaUI()
    {
        WaveController.Instance.killsThisWaveText.enabled = false;
        WaveController.Instance.killQuotaText.enabled = false;
        WaveController.Instance.killQuotaLabel.enabled = false;
        WaveController.Instance.killQuotaSlash.enabled = false;

    }


    public IEnumerator HealthBarFlash(Material flashMat, float duration)
    {
        var oldMat = healthBarFill.material;
        healthBarFill.material = flashMat;
        yield return new WaitForSecondsRealtime(duration);
        healthBarFill.material = oldMat;

    }

    public IEnumerator ScreenFlashWhite(float duration)
    {
        FlashWhite.SetActive(true);
        yield return new WaitForSecondsRealtime(duration);
        FlashWhite.SetActive(false);
    }

    public IEnumerator ScreenFlashRed(float duration)
    {
        FlashRed.SetActive(true);
        yield return new WaitForSecondsRealtime(duration);
        FlashRed.SetActive(false);
    }

    public IEnumerator ShowMessage(string text, Color color, int size, float duration)
    {
        if (message != null)
        {
            message.color = color;
            message.text = text;
            message.fontSize = size;
            yield return new WaitForSeconds(duration);
            ClearMessage();
        }

    }

    public IEnumerator ShowMessageCenter(string text, Color color, int size, float duration)
    {
        if (messageCenter != null)
        {
            messageCenter.color = color;
            messageCenter.text = text;
            messageCenter.fontSize = size;
            yield return new WaitForSeconds(duration);
            ClearMessageCenter();
        }

    }

    public void ClearMessage()
    {
        message.text = "";
    }

    public void ClearMessageCenter()
    {
        messageCenter.text = "";
    }


    bool effectsFade; 
    public PostProcessVolume effectsVolume_From;
    public PostProcessVolume effectsVolume_To;
    float duration;
    float startTime;

    public void EffectsFade(PostProcessVolume _effectsVolume_From, PostProcessVolume _effectsVolume_To, float _duration)
    {
        effectsFade = true;
        startTime = Time.time;
        effectsVolume_From = _effectsVolume_From;
        effectsVolume_To = _effectsVolume_To;
        duration = _duration;
    }

    void ProcessEffectsFade()
    {
        float t = (Time.time - startTime) / duration;
        effectsVolume_From.weight = Mathf.SmoothStep(1, 0, t);
        effectsVolume_To.weight = Mathf.SmoothStep(0, 1, t);

        if (effectsVolume_From.weight < 0.01f)
        {
            effectsVolume_From.weight = 0;
            effectsFade = false;
        }

        if (effectsVolume_To.weight > 0.99f)
        {
            effectsVolume_To.weight = 1;
            effectsFade = false;
        }
    }
}
