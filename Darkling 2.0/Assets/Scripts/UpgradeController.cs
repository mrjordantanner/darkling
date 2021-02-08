using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UpgradeTier { I, II, III, IV, V, VI}

public class UpgradeController : MonoBehaviour
{
    #region Singleton
    public static UpgradeController Instance;

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

    public UpgradeItem upgradeItem;
    public enum UpgradeItem { Shot1, Shot2, Health1, Health2, Speed1, Jump1, Regen1, Regen2, Shield1, Shield2, DoubleGrenades, DemonGlove }
    public UpgradeTier currentTier;
    public Text maxHPText;
    public CanvasGroup upgradeMenuCanvasGroup;
    public Text upgradeDescriptionField;

    public Upgrade[] upgradeBank;
    public List<Upgrade> purchasedUpgrades = new List<Upgrade>();
    PlayerCharacter player;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        player = PlayerRef.Instance.player;
        upgradeMenuCanvasGroup.alpha = 0;
        upgradeMenuCanvasGroup.interactable = false;
        upgradeMenuCanvasGroup.blocksRaycasts = false;
        InitializeUpgrades();
    }

    // Create and Load all upgrades at game start
    public void InitializeUpgrades()
    {
        player.spriteRenderer.color = Color.white;
        player.ghostTrails.on = false;

        purchasedUpgrades.Clear();
        upgradeBank = FindObjectsOfType<Upgrade>();
        CheckAvailability();

    }

    
    public void CheckAvailability()
    {
        maxHPText.text = Stats.Instance.maxHP.ToString();
        currentTier = TierCheck();

        foreach (var upgrade in upgradeBank)
        {
            if (upgrade.upgradeTier <= currentTier && !purchasedUpgrades.Contains(upgrade))
            {
                upgrade.available = true;
                if (upgrade.button != null)
                     upgrade.button.interactable = true;
            }
            else
            {
                upgrade.available = false;
                if (upgrade.button != null)
                    upgrade.button.interactable = false;
            }
        }

    }


    public void OpenUpgradeMenu()
    {
       // AudioManager.Instance.ReduceMusicVolume();

        HUD.Instance.ClearMessage();
        HUD.Instance.ClearMessageCenter();
        GameManager.Instance.upgradeInProgress = true;
        upgradeMenuCanvasGroup.alpha = 1;
        upgradeMenuCanvasGroup.interactable = true;
        upgradeMenuCanvasGroup.blocksRaycasts = true;
        AudioManager.Instance.Play("Menu Sound");
        CheckAvailability();
        GameManager.Instance.Pause();
    }

    public void CloseUpgradeMenu()
    {
        AudioManager.Instance.RestoreMusicVolume();

        GameManager.Instance.upgradeInProgress = false;
        upgradeMenuCanvasGroup.alpha = 0;
        upgradeMenuCanvasGroup.interactable = false;
        upgradeMenuCanvasGroup.blocksRaycasts = false;
        GameManager.Instance.Unpause();
    }

    public void Purchase(Upgrade upgrade)
    {
        if (upgrade.lifeCost <= Stats.Instance.maxHP - Stats.Instance.minimumHPAllowed)
        {
            Stats.Instance.lifeSpent += upgrade.lifeCost;
            var newMaxHP = Stats.Instance.maxHP - upgrade.lifeCost;
            Stats.Instance.ChangeMaxHP(newMaxHP);
            AudioManager.Instance.Play("Take Damage");
            AudioManager.Instance.Play("Upgrade");
            StartCoroutine(HUD.Instance.ScreenFlashRed(0.25f));
            upgrade.button.interactable = false;
            upgrade.available = false;
            purchasedUpgrades.Add(upgrade);
            GrantUpgradeEffects(upgrade);
            CheckAvailability();
        }
        else
        {
            StartCoroutine(HUD.Instance.ShowMessage("Not enough Life to give.", Color.red, 36, 3f));
        }

    }

    Upgrade GetUpgrade(string upgradeName)
    {
        foreach (var upgrade in upgradeBank)
        {
            if (upgrade.upgradeName == upgradeName)
            {
                var u = upgrade;
                return u;
            }
        }

        return null;
    }

    public UpgradeTier TierCheck()
    {
        var wave = WaveController.Instance.currentWave;
        if (wave == 1) currentTier = UpgradeTier.I;
        if (wave == 2 || wave == 3) currentTier = UpgradeTier.II;
        if (wave == 4 || wave == 5) currentTier = UpgradeTier.III;
        if (wave == 6 || wave == 7) currentTier = UpgradeTier.IV;
        if (wave == 8 || wave == 9) currentTier = UpgradeTier.V;
        if (wave > 9) currentTier = UpgradeTier.VI;
        return currentTier;
    }

   
    public void GrantUpgradeEffects(Upgrade upgrade)
    {
        //if (upgrade.upgradeName == "Shot1") Stats.Instance.hasShot1 = true;
        //if (upgrade.upgradeName == "Shot2") Stats.Instance.hasShot2 = true;

        //if (upgrade.upgradeName == "Health1") Stats.Instance.hasHealth1 = true;
        //if (upgrade.upgradeName == "Health2") Stats.Instance.hasHealth2 = true;

        //if (upgrade.upgradeName == "Speed1") Stats.Instance.hasSpeed1 = true;
        //// if (upgrade.upgradeName == "Speed2") Stats.Instance.hasSpeed1 = true;

        //if (upgrade.upgradeName == "Jump1")  Stats.Instance.hasJump1 = true;
        //// if (upgrade.upgradeName == "Jump2") Stats.Instance.hasJump1 = true;

        //if (upgrade.upgradeName == "Regen1") Stats.Instance.hasRegen1 = true;
        //if (upgrade.upgradeName == "Regen2") Stats.Instance.hasRegen2 = true;

        //if (upgrade.upgradeName == "Shield1") Stats.Instance.hasShield1 = true;
        //if (upgrade.upgradeName == "Shield2") Stats.Instance.hasShield2 = true;

        //if (upgrade.upgradeName == "DoubleGrenades") Stats.Instance.hasDoubleGrenades = true;
        //if (upgrade.upgradeName == "DemonGlove")
        //{
        //    GrantDemonGlove();

        //}

        if (upgrade.upgradeItem == UpgradeItem.Shot1) Stats.Instance.hasShot1 = true;
        if (upgrade.upgradeItem == UpgradeItem.Shot2) Stats.Instance.hasShot2 = true;

        if (upgrade.upgradeItem == UpgradeItem.Health1) Stats.Instance.hasHealth1 = true;
        if (upgrade.upgradeItem == UpgradeItem.Health2) Stats.Instance.hasHealth2 = true;

        if (upgrade.upgradeItem == UpgradeItem.Speed1) Stats.Instance.hasSpeed1 = true;
        // if (upgrade.upgradeName == "Speed2") Stats.Instance.hasSpeed1 = true;

        if (upgrade.upgradeItem == UpgradeItem.Jump1) Stats.Instance.hasJump1 = true;
        // if (upgrade.upgradeName == "Jump2") Stats.Instance.hasJump1 = true;

        if (upgrade.upgradeItem == UpgradeItem.Regen1) Stats.Instance.hasRegen1 = true;
        if (upgrade.upgradeItem == UpgradeItem.Regen2) Stats.Instance.hasRegen2 = true;

        if (upgrade.upgradeItem == UpgradeItem.Shield1) Stats.Instance.hasShield1 = true;
        if (upgrade.upgradeItem == UpgradeItem.Shield2) Stats.Instance.hasShield2 = true;

        if (upgrade.upgradeItem == UpgradeItem.DoubleGrenades) Stats.Instance.hasDoubleGrenades = true;
        if (upgrade.upgradeItem == UpgradeItem.DemonGlove) {
            Stats.Instance.hasDemonGlove = true;
            player.spriteRenderer.color = demonGloveColor;
            player.ghostTrails.on = true;
        }


        //if (upgrade.upgradeName == "TripleShot")
        //{
        //    GetUpgrade("TwinShot").available = false;
        //    Stats.Instance.hasTwinShot = true;
        //    Stats.Instance.hasTripleShot = true;
        //}

        //if (upgrade.upgradeName == "Regen2")
        //{
        //    GetUpgrade("Regen1").available = false;
        //    Stats.Instance.hasRegen1 = true;
        //    Stats.Instance.hasRegen2 = true;
        //}

    }

    public Color demonGloveColor;


}
