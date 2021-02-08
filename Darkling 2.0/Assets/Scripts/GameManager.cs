using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance;

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

    public bool enteringText;

    public bool debugMode;
    public bool canPause;
    public bool playerHasControl;
    public bool gamePaused;
    public bool sceneTransition;

    public Light mainLight;

    public Image targetReticle;
    public Image[] menuBGImages;
    public GameObject pauseMenuBG;
    public Button startButton, resumeButton, returnToMenuButton;
    public Button addNewUserButton;
    public Camera orthoCam;
    public Transform startingPoint;

    [Header("Menu Panels")]
    public CanvasGroup hudCanvasGroup;
    public CanvasGroup mainMenu, audioPanel, mainMenuPanel, instructionsPanel, deleteUserPanel, userInfoPanel, selectUserPanel, userNameTakenPanel, newUserPanel;
    [HideInInspector]
    public Animator audioPanelAnim, mainMenuPanelAnim, instructionsPanelAnim, deleteUserPanelAnim, userInfoPanelAnim, selectUserPanelAnim;
    public InputField usernameInputField;

    public GameObject gameLogoA, gameLogoB;

    [Header("User Info Panel")]
    public Text userInfoPanel_userNameText;
    public Text userInfoPanel_bestWaveText;
    public Text userInfoPanel_bestTimeRemaining;

    [Header("Delete User Panel")]
    public Text deleteUserPanel_userNameText;

    [Header("Post Mortem UI")]
    public CanvasGroup postMortemPanel;
    public Text maxWaveReachedText, killsThisWaveText, killsThisRunText, kpmText, accuracyText, lifeSpentText, userNameText;
    public Text bestWaveText, bestKillsText;//, best_killsText, best_kpmText, best_accuracyText;
    public Text newPersonalBestMessage, leaderBoardMessage;


    [Header("Prefab Pools")]
    public GameObject Spider1;
    public GameObject Spider2, Orb, Bullet, ExplosiveBeam;//, ExplosionSmall;
    [HideInInspector]
    public SimplePool.Pool spiderPool1, spiderPool2, enemyOrbPool, enemyBulletPool, explosiveBeamPool;//, explosionSmallPool;

    PlayerCharacter player;
    Animator playerAnim;
    [HideInInspector]
    public bool upgradeInProgress;

    BlinkingText waveBlink, killsBlink;
    LeaderboardTable leaderboardTable;

    void Start()
    {
        audioPanelAnim = audioPanel.GetComponent<Animator>();
        mainMenuPanelAnim = mainMenuPanel.GetComponent<Animator>();
        instructionsPanelAnim = instructionsPanel.GetComponent<Animator>();
        deleteUserPanelAnim = deleteUserPanel.GetComponent<Animator>();
        userInfoPanelAnim = userInfoPanel.GetComponent<Animator>();
        selectUserPanelAnim = selectUserPanel.GetComponent<Animator>();

        waveBlink = maxWaveReachedText.GetComponent<BlinkingText>();
        killsBlink = killsThisWaveText.GetComponent<BlinkingText>();
        leaderboardTable = FindObjectOfType<LeaderboardTable>();

        hudCanvasGroup.alpha = 0;
        SetCursorState(CursorLockMode.None);
        player = PlayerRef.Instance.player;
        playerAnim = player.gameObject.GetComponentInChildren<Animator>();
        player.spriteRenderer.enabled = false;
        player.dead = true;
        player.canMove = false;
        player.inputSuspended = true;
        Stats.Instance.regen = true;
        Stats.Instance.fpsController.enabled = false;

        leaderboardTable.CreateRows();
        CreatePools();

        if (debugMode)
        {
            Initialize();
        }
    }


    public void SetPauseState(bool pausable)
    {
        canPause = pausable;
    }

    void CreatePools()
    {
        spiderPool1 = new SimplePool.Pool(Spider1, 150);
        spiderPool2 = new SimplePool.Pool(Spider2, 150);
        enemyOrbPool = new SimplePool.Pool(Orb, 500);
        enemyBulletPool = new SimplePool.Pool(Bullet, 500);
        explosiveBeamPool = new SimplePool.Pool(ExplosiveBeam, 500);
        //explosionSmallPool = new SimplePool.Pool(ExplosionSmall, 100);

    }

    public void StartReload()
    {
        StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        yield return StartCoroutine(ScreenFader.Instance.FadeSceneOut(1f));
        ReloadGame();
        StartCoroutine(ScreenFader.Instance.FadeSceneIn(1f));

    }


    public void ReloadGame()
    {
        foreach (var image in menuBGImages)
        {
            image.gameObject.SetActive(true);
        }

        gameLogoA.SetActive(true);
        gameLogoB.SetActive(true);

        player.gameObject.SetActive(false);

        canPause = false;

        postMortemPanel.alpha = 0;
        postMortemPanel.interactable = false;
        postMortemPanel.blocksRaycasts = false;

        mainMenu.alpha = 1;  
        mainMenu.interactable = true;
        mainMenu.blocksRaycasts = true;

        mainMenuPanel.alpha = 1;
        mainMenuPanel.interactable = true;
        mainMenuPanel.blocksRaycasts = true;

        OpenMenuPanel(mainMenuPanel, mainMenuPanelAnim);

        SwapButtons(resumeButton.gameObject, startButton.gameObject);
        returnToMenuButton.gameObject.SetActive(false);
        targetReticle.enabled = false;
        hudCanvasGroup.alpha = 0;
        orthoCam.enabled = true;

        SetCursorState(CursorLockMode.None);

        leaderboardTable.CreateRows();

    }


    // Game Start
    public void Initialize()
    {
        // turn off menu bg images
        foreach (var image in menuBGImages)
        {
            image.gameObject.SetActive(false);
        }

        gameLogoA.SetActive(false);
        gameLogoB.SetActive(false);

        player.gameObject.SetActive(true);

        canPause = true;
        UpgradeController.Instance.upgradeMenuCanvasGroup.blocksRaycasts = true;

        mainMenu.alpha = 0;
        mainMenu.interactable = false;
        mainMenu.blocksRaycasts = false;

        SwapButtons(startButton.gameObject, resumeButton.gameObject);
        returnToMenuButton.gameObject.SetActive(true);
        targetReticle.enabled = true;
        hudCanvasGroup.alpha = 1;
        orthoCam.enabled = false;

        SetCursorState(CursorLockMode.Locked);

        UserController.Instance.SetCurrentStats();  // sets current stats this run to 0

        HUD.Instance.activeUserText.text = UserController.Instance.activeUser.userData.userName;

        Restart();

    }
    
    public void Restart()
    {
        // Destroy all enemies and enemy projectiles
        var allEnemies = FindObjectsOfType<EnemyCharacter>();
        foreach (var enemy in allEnemies)
        {
           // Destroy(enemy.gameObject);
            SimplePool.Despawn(enemy.gameObject);
            SpawnerController.Instance.totalEnemies = 0;
        }

        var allProjectiles = FindObjectsOfType<EnemyProjectile>();
        foreach (var projectile in allProjectiles)
        {
            projectile.DestroyOnImpact();
        }

        // Destroy collectibles
        var allCollectibles = FindObjectsOfType<Collectible>();
        foreach (var collectible in allCollectibles)
        {
            Destroy(collectible.gameObject);
        }

        Unpause();

        AudioManager.Instance.RestoreMusicVolume();   // it might have been down during post mortem

        postMortemPanel.alpha = 0;
        postMortemPanel.interactable = false;
        postMortemPanel.blocksRaycasts = false;

        HUD.Instance.FlashRed.gameObject.SetActive(false);
        HUD.Instance.FlashWhite.gameObject.SetActive(false);
        HUD.Instance.ClearMessage();
        HUD.Instance.ClearMessageCenter();

        playerAnim.ResetTrigger("StartAttacking");
        playerAnim.SetBool("Attacking", false);
        GunShake.Instance.shake = false;
        player.spriteRenderer.enabled = true;
        player.dead = false;
        player.canMove = true;
        player.inputSuspended = false;
        Stats.Instance.regen = false;
        player.transform.position = startingPoint.position;

        Stats.Instance.Init();
        UpgradeController.Instance.Init();
        WaveController.Instance.Init();
        SpawnerController.Instance.Init();

        Stats.Instance.fpsController.enabled = true;

        newPersonalBestMessage.text = "";
        leaderBoardMessage.text = "";
        waveBlink.blink = false;
        killsBlink.blink = false;

        if (!debugMode)
            WaveController.Instance.SetupNextWave();
    }

    public void Quit()
    {

//#if UNITY_EDITOR
//            UnityEditor.EditorApplication.isPlaying = false;
//#else
//         Application.Quit();
//#endif

# if UNITY_STANDALONE
        Application.Quit();
#endif

    }

    void ToggleHUD()
    {
        if (hudCanvasGroup.alpha < 1)
        {
            hudCanvasGroup.alpha = 1;
        }
        else
        {
            hudCanvasGroup.alpha = 0;
        }
    }

    void Update()
    {
        //// Toggle HUD
        if (Input.GetKeyDown(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Z) && canPause) ToggleHUD();

        // Pause and Show Main Menu
        if (Input.GetKeyDown(KeyCode.Escape) && !gamePaused && canPause && !upgradeInProgress && !enteringText)
        {
            PauseWithMenu();
            AudioManager.Instance.ReduceMusicVolume();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && gamePaused && canPause && !upgradeInProgress && !enteringText)
        {
            Unpause();
            AudioManager.Instance.RestoreMusicVolume();
        }

        if (Input.GetKeyDown(KeyCode.R) && !gamePaused && canPause && !enteringText)
        {
            Restart();
        }


    }

    public void SetEnteringTextState(bool state)
    {
        enteringText = state;
    }

    public void SetCursorState(CursorLockMode mode)
    {
        Cursor.lockState = mode;
        // Hide cursor when locking
        Cursor.visible = (CursorLockMode.Locked != mode);
    }




    public IEnumerator SceneTransition(float fadeDuration)
    {
        yield return StartCoroutine(ScreenFader.Instance.FadeSceneOut(fadeDuration));
        Initialize();
        StartCoroutine(ScreenFader.Instance.FadeSceneIn(fadeDuration));
    }

    public void PauseWithMenu()
    {
        Pause();

        gameLogoA.SetActive(true);
        gameLogoB.SetActive(true);

        pauseMenuBG.SetActive(true);

        mainMenu.alpha = 1;
        mainMenu.interactable = true;
        mainMenu.blocksRaycasts = true;

        mainMenuPanelAnim.SetTrigger("Open");
        mainMenuPanel.alpha = 1;
        mainMenuPanel.interactable = true;
        mainMenuPanel.blocksRaycasts = true;

    }

    public void Pause()
    {
        mainMenuPanelAnim.ResetTrigger("Open");
        mainMenuPanelAnim.ResetTrigger("Close");

        targetReticle.enabled = false;
        SetCursorState(CursorLockMode.None);
        gamePaused = true;
        Time.timeScale = 0;
        playerHasControl = false;
    }

    public void Unpause()
    {
        mainMenuPanelAnim.ResetTrigger("Open");
        mainMenuPanelAnim.ResetTrigger("Close");

        mainMenu.alpha = 0;
        mainMenu.interactable = false;
        mainMenu.blocksRaycasts = false;

        if (audioPanel.alpha > 0)
        {
            CloseMenuPanel(audioPanel, audioPanelAnim);
            return;
        }

        if (instructionsPanel.alpha > 0)
        {
            CloseMenuPanel(instructionsPanel, instructionsPanelAnim);
            return;
        }

        pauseMenuBG.SetActive(false);
        gameLogoA.SetActive(false);
        gameLogoB.SetActive(false);

        CloseMenuPanel(mainMenuPanel, mainMenuPanelAnim);

        targetReticle.enabled = true;
        SetCursorState(CursorLockMode.Locked);
        gamePaused = false;
        Time.timeScale = 1;
        playerHasControl = true;

    }

    // TODO: Needs work
    public int GetLeaderboardRank()
    {
        var userDataList = Dreamlo.Instance.userDataList;
        Dreamlo.Instance.DownloadAll();

        for (int i = 0; i < userDataList.Count; i++)
        {
            if (userDataList[i].userName == UserController.Instance.activeUser.userData.userName)
            {
                return (i+1);
            }

        }

        return 0;
    }

    // For use by menu buttons when exiting or returning to main menu - skips the post mortem window
    public void CheckAndUpload()
    {
        UserController.Instance.CheckForBest();
        UploadNewBest();
    }


    public void ShowPostMortemPanel()
    {
        Pause();

        AudioManager.Instance.ReduceMusicVolume();

        postMortemPanel.alpha = 1;
        postMortemPanel.interactable = true;
        postMortemPanel.blocksRaycasts = true;

        UploadNewBest();

        // Current Stats UI
        userNameText.text = UserController.Instance.activeUser.userData.userName.ToString();
        maxWaveReachedText.text = UserController.Instance.activeUser.userData.currentWave.ToString();
        killsThisWaveText.text = UserController.Instance.activeUser.userData.currentKills.ToString();

        killsThisRunText.text = Stats.Instance.kills.ToString();
        kpmText.text = Stats.Instance.killsPerMinute.ToString("F1");   // "F2" rounds to 2 decimals
        accuracyText.text = Stats.Instance.accuracy.ToString("F1") + "%";
        lifeSpentText.text = Stats.Instance.lifeSpent.ToString();

        // Best Stats UI
        bestWaveText.text = UserController.Instance.activeUser.userData.bestWave.ToString();
        bestKillsText.text = UserController.Instance.activeUser.userData.bestKills.ToString();

        //best_killsText.text = Stats.Instance.kills.ToString();
        //best_kpmText.text = Stats.Instance.killsPerMinute.ToString();
        //best_accuracyText.text = Stats.Instance.accuracy.ToString();
        //best_lifeSpentText.text = Stats.Instance.lifeSpent.ToString();



    }

    public void UploadNewBest()
    {
        if (Stats.Instance.newBestRun && !Stats.Instance.cheated)
        {
            Dreamlo.Instance.UploadData(UserController.Instance.activeUser);
            newPersonalBestMessage.text = "New Personal Best!";
            //   leaderBoardMessage.text = "Leaderboard Rank: " + GetLeaderboardRank();
            waveBlink.blink = true;
            killsBlink.blink = true;
            // TODO: AudioManager.Instance.Play("New Record");
        }
        else if (Stats.Instance.cheated)
        {
            newPersonalBestMessage.text = "Cheaters Never Win!";
            // leaderBoardMessage.text = "Leaderboard Rank: N/A";
        }

    }

    #region MenuPanels
    public void OpenAudioPanel()
    {
        audioPanelAnim.SetTrigger("Open");
        audioPanel.interactable = true;
        mainMenuPanelAnim.SetTrigger("Close");
        mainMenuPanel.interactable = false;
    }

    public void CloseAudioPanel()
    {
        audioPanelAnim.SetTrigger("Close");
        audioPanel.interactable = false;
        mainMenuPanelAnim.SetTrigger("Open");
        mainMenuPanel.interactable = true;
    }

    public void OpenInstructionsPanel()
    {
        instructionsPanelAnim.SetTrigger("Open");
        instructionsPanel.interactable = true;
        mainMenuPanelAnim.SetTrigger("Close");
        mainMenuPanel.interactable = false;
    }

    public void CloseInstructionsPanel()
    {
        instructionsPanelAnim.SetTrigger("Close");
        instructionsPanel.interactable = false;
        mainMenuPanelAnim.SetTrigger("Open");
        mainMenuPanel.interactable = true;
    }


    public void ClosePostMortemPanel()
    {
        postMortemPanel.alpha = 0;
        postMortemPanel.interactable = false;
        postMortemPanel.blocksRaycasts = false;

    }

    public void ShowUserInfo()
    {
        // Set UI text based on stats
        userInfoPanel_userNameText.text = UserController.Instance.activeUser.userName;
        userInfoPanel_bestWaveText.text = UserController.Instance.activeUser.userData.bestWave.ToString();
        userInfoPanel_bestTimeRemaining.text = UserController.Instance.activeUser.userData.bestKills.ToString();

        // TODO: More user stats....
        // userInfoPanel_totalTimePlayed.text = 
        // userInfoPanel_totalKills.text = 
        // userInfoPanel_totalLifeSpent.text = 
        // etc...
    }

    public void ShowDeleteUserPanel()
    {
        CloseMenuPanel(userInfoPanel, userInfoPanelAnim);
        OpenMenuPanel(deleteUserPanel, deleteUserPanelAnim);

        deleteUserPanel_userNameText.text = UserController.Instance.activeUser.userName;


    }


    public void CheckAddNewButton()
    {
        if (UserController.Instance.HasReachedMaxUsers()) addNewUserButton.interactable = false;
        else addNewUserButton.interactable = true;
    }



    public void ShowUserNameTakenPanel()
    {
        userNameTakenPanel.alpha = 1;
        userNameTakenPanel.interactable = true;
        userNameTakenPanel.blocksRaycasts = true;

        newUserPanel.interactable = false;
        newUserPanel.blocksRaycasts = false;

    }


    public void CloseUserNameTakenPanel()
    {
        userNameTakenPanel.alpha = 0;
        userNameTakenPanel.interactable = false;
        userNameTakenPanel.blocksRaycasts = false;

        newUserPanel.interactable = true;
        newUserPanel.blocksRaycasts = true;

    }


    public void OpenMenuPanel(CanvasGroup panel, Animator anim)
    {
        if (anim != null)
        {
            anim.SetTrigger("Open");
            anim.ResetTrigger("Close");
        }
        panel.interactable = true;
        panel.blocksRaycasts = true;
       
    }

    public void CloseMenuPanel(CanvasGroup panel, Animator anim)
    {
        if (anim != null)
        {
            anim.SetTrigger("Close");
            anim.ResetTrigger("Open");
        }
        panel.interactable = false;
        panel.blocksRaycasts = false;
    }

    public void SwapButtons(GameObject oldButton, GameObject newButton)
    {
        oldButton.gameObject.SetActive(false);
        newButton.gameObject.SetActive(true);
    }

    public void ClearInputText()
    {
        usernameInputField.text = "";
    }
    #endregion

    public void QuitGame()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;

#else
            Application.Quit();

#endif
    }
}
