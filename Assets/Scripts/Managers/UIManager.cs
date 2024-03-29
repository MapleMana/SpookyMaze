﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.Localization.Settings;

public class UIManager : Singleton<UIManager>
{
    public GameObject aboutMenu;
    public GameObject dailyMenu;
    public GameObject endGameMenu;
    public GameObject earnCoinPanel;
    public Button endGameNextLevelButton;
    public GameObject classicLevelSelectMenu;
    public GameObject dungeonLevelSelectMenu;
    public GameObject cursedHouseLevelSelectMenu;
    public GameObject mainMenu;
    public GameObject onReplayMenu;
    public GameObject purchaseMenu;
    public GameObject settingsMenu;
    public GameObject statsMenu;
    public GameObject inGameMenu;
    public GameObject inGameControls;
    public Text levelNumText;
    public Text levelSizeText;
    public GameObject dungeonKeyPanel;
    public GameObject dungeonLock;
    public Sprite lockedImage;
    public Sprite unlockedImage;
    public GameObject dungeonKey;
    public GameObject helpMenu;
    public Image nextPlayButtonImage;
    public Sprite nextLevelImage;
    public Sprite replayLevelImage;

    [Header ("Control Buttons")]
    public Button touchControl;
    public Button btnControl;

    [Header("Language Buttons")]
    public Button ENBtn; // 0
    public Button FRBtn; // 1
    public Button DEBtn; // 2
    public Button ITBtn; // 3
    public Button ESBtn; // 4

    public Text purchaseBtnCoinsText;

    public Text coinText;

    private bool _levelCompleted;
    public bool LevelCompleted { get => _levelCompleted; set => _levelCompleted = value; }

    private bool _animateEarningCoins;
    private Vector3 _initialPos;

    private Color spookyOrange = new Color(248f / 255f, 148f / 255f, 6f / 255f);

    private void Start()
    {
        UpdateTextOnPurchaseMenuButton();
        _animateEarningCoins = false;
        ChangeControls(PlayerPrefs.GetInt("isTouch", 1)); // 1 is true, 0 is false
    }

    private void Update()
    {
        if (_animateEarningCoins)
        {
            earnCoinPanel.transform.position = Vector3.Lerp(earnCoinPanel.transform.position, coinText.gameObject.transform.position, Time.deltaTime * 2f);
            if (Vector3.Distance(earnCoinPanel.transform.position, coinText.gameObject.transform.position) < 100f)
            {
                earnCoinPanel.SetActive(false);
                earnCoinPanel.transform.position = _initialPos;
                _animateEarningCoins = false;
                SoundManager.Instance.PlaySoundEffect(SoundEffect.CoinsEarned);
                coinText.text = $"{PlayerPrefs.GetInt("PlayersCoins", 0)}";
            }
        }

        // Make sure user is on Android platform
        if (Application.platform == RuntimePlatform.Android || Application.isEditor)
        {
            // Check if Back was pressed this frame
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HandleAndroidBackBtn();
            }
        }
    }

    /// <summary>
    /// This method loads the new maze and starts the game
    /// </summary>
    public void StartGame()
    {
        HideAllMenus();
        SceneManager.LoadScene("Maze", LoadSceneMode.Additive);        
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Displays the finish menu, when a player gets to the end point
    /// </summary>
    public void ShowFinishMenu(bool mazeCompleted)
    {
        LevelCompleted = mazeCompleted;
        ToggleEndGameMenu();        
        SetNextActionText();        
    }

    public void FirstTimeCompletingLevel(bool firstTime)
    {
        if (firstTime)
        {
            //Debug.Log("First Time");
            earnCoinPanel.SetActive(true);
            _initialPos = earnCoinPanel.transform.position;
            coinText.text = $"{PlayerPrefs.GetInt("PlayersCoins", 0) - 4}";
            StartCoroutine(Wait());
        }
        else
        {
            //Debug.Log("Not First Time");
            earnCoinPanel.SetActive(false);
            coinText.text = $"{PlayerPrefs.GetInt("PlayersCoins", 0)}";
            _animateEarningCoins = false;
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.5f);
        _animateEarningCoins = true;
        StopCoroutine(Wait());
    }

    public void ToggleEndGameMenu()
    {
        endGameMenu.SetActive(!endGameMenu.activeInHierarchy);
    }

    /// <summary>
    /// Sets the text on the button for showing the next available action
    /// </summary>
    public void SetNextActionText()
    {
        nextPlayButtonImage.GetComponent<Image>().sprite = _levelCompleted ? nextLevelImage : replayLevelImage;
        if (_levelCompleted)
        {
            endGameNextLevelButton.interactable = !GameManager.Instance.IsLastLevel();
        }
        else
        {
            endGameNextLevelButton.interactable = true;
        }
    }

    /// <summary>
    /// Goes to the MainMenu Scene and displays the main menu again
    /// </summary>
    public void GoToMainMenu()
    {        
        LightManager.Instance.TurnOff();
        SceneManager.UnloadSceneAsync("Maze");
        HideAllMenus();
        ToggleMainMenu();
        UpdateTextOnPurchaseMenuButton();
        MusicManager.Instance.PlayMusic(Music.MenuMusic);
    }

    /// <summary>
    /// Replays player's movements from the start. Fired from FinishMenu.
    /// </summary>
    public void ReplayPlayersMovement()
    {
        ToggleEndGameMenu();
        LevelManager.Instance.WatchReplay(
            onComplete: () => UIManager.Instance.ShowFinishMenu(_levelCompleted)
        );
    }

    /// <summary>
    /// Restarts current level. Fired from InGamehMenu.
    /// </summary>
    public void RestartLevel()
    {
        Movable.ClearHistory();
        ToggleInGameMenu();
        GameManager.Instance.LoadLevel();
    }

    /// <summary>
    /// Replays player's movements from finish to the start. Fired from FinishMenu.
    /// </summary>
    public void GoToNextLevel()
    {
        if (_levelCompleted && !GameManager.Instance.IsLastLevel())
        {
            GameManager.Instance.CurrentSettings.id++;
            if (GameManager.Instance.CurrentSettings.isDaily)
            {
                GameManager.Instance.CurrentSettings.dimensions = LevelIO.GetDailyDimension(GameManager.Instance.CurrentSettings)[0];
            }
        }
        ToggleEndGameMenu();
        ToggleOnReplyMenu();
        LevelManager.Instance.LoadCurrentLevel();
    }

    /// <summary>
    /// Skips replay of player's movements and loads the next level
    /// </summary>
    public void SkipReplay()
    {
        LevelManager.Instance.StopAllCoroutines();
        LevelManager.Instance.Clear();
        GameManager.Instance.LoadLevel();
    }

    /// <summary>
    /// Shows or hide panel functions
    /// Let as separate functions in case a unique action is required when showing or closing
    /// </summary>
    public void ToggleMainMenu()
    {
        mainMenu.SetActive(!mainMenu.activeInHierarchy);
    }

    public void ToggleInGameMenu()
    {
        if (inGameMenu.activeInHierarchy)
        {
            inGameMenu.SetActive(false);
        }
        else
        {
            inGameMenu.SetActive(true);
            int totalLevels = GameManager.Instance.CurrentSettings.isDaily ? 4 : 20;
            levelNumText.text = $"{GameManager.Instance.CurrentSettings.id} / {totalLevels}";
            levelSizeText.text = GameManager.Instance.CurrentSettings.dimensions.ToString();
            if (GameManager.Instance.CurrentSettings.GetReadableGameMode() == "Dungeon")
            {
                dungeonKeyPanel.SetActive(true);
                dungeonKey.SetActive(false);
                dungeonLock.GetComponent<Image>().sprite = lockedImage;
                dungeonLock.SetActive(true);
            }
            else
            {
                dungeonKeyPanel.SetActive(false);
            }
        }        
    }

    // called in ExitDoor when key is in player inventory
    public void PickedUpKey()
    {
        StartCoroutine(PickedUpKeyAnimation());
    }

    IEnumerator PickedUpKeyAnimation()
    {
        dungeonKey.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        dungeonLock.GetComponent<Image>().sprite = unlockedImage;
        yield return new WaitForSeconds(1f);
        dungeonLock.SetActive(false);
    }

    public void ToggleAboutMenu(bool active)
    {
        aboutMenu.SetActive(active);       
    }

    public void ToggleSettingsMenu()
    {
        settingsMenu.SetActive(!settingsMenu.activeInHierarchy);
    }

    public void ToggleClassicLevelSelectMenu(string modeName)
    {
        if (classicLevelSelectMenu.activeInHierarchy)
        {
            classicLevelSelectMenu.SetActive(false);
            classicLevelSelectMenu.GetComponent<LevelSelectMenu>().ClearPanel();
        }
        else
        {
            GameManager.Instance.CurrentSettings.gameMode = modeName;
            GameManager.Instance.CurrentSettings.isDaily = false;            
            classicLevelSelectMenu.GetComponent<LevelSelectMenu>().LoadMenu();
            classicLevelSelectMenu.SetActive(true);
        }
    }

    public void ToggleDungeonLevelSelectMenu(string modeName)
    {
        if (dungeonLevelSelectMenu.activeInHierarchy)
        {
            dungeonLevelSelectMenu.SetActive(false);
            dungeonLevelSelectMenu.GetComponent<LevelSelectMenu>().ClearPanel();
        }
        else
        {
            GameManager.Instance.CurrentSettings.gameMode = modeName;
            GameManager.Instance.CurrentSettings.isDaily = false;
            dungeonLevelSelectMenu.GetComponent<LevelSelectMenu>().LoadMenu();
            dungeonLevelSelectMenu.SetActive(true);
        }
    }

    public void ToggleCursedHouseLevelSelectMenu(string modeName)
    {
        if (cursedHouseLevelSelectMenu.activeInHierarchy)
        {
            cursedHouseLevelSelectMenu.SetActive(false);
            cursedHouseLevelSelectMenu.GetComponent<LevelSelectMenu>().ClearPanel();
        }
        else
        {
            GameManager.Instance.CurrentSettings.gameMode = modeName;
            GameManager.Instance.CurrentSettings.isDaily = false;
            cursedHouseLevelSelectMenu.GetComponent<LevelSelectMenu>().LoadMenu();
            cursedHouseLevelSelectMenu.SetActive(true);
        }
    }

    public void TogglePurchaseMenu()
    {
        purchaseMenu.SetActive(!purchaseMenu.activeInHierarchy);
        UpdateTextOnPurchaseMenuButton();
    }

    public void ToggleStatsMenu()
    {
        statsMenu.SetActive(!statsMenu.activeInHierarchy);
    }    

    public void ToggleOnReplyMenu()
    {
        onReplayMenu.SetActive(!onReplayMenu.activeInHierarchy);
    }

    public void ToggleDailyMenu()
    {
        if (dailyMenu.activeInHierarchy)
        {
            dailyMenu.SetActive(false);
            dailyMenu.GetComponent<DailyLevelSelectMenu>().ClearPanels();
            dailyMenu.GetComponent<DailyLevelSelectMenu>().ResetButtons();
        }
        else
        {
            LevelGenerator.GenerateDailyLevels();
            dailyMenu.GetComponent<DailyLevelSelectMenu>().LoadDailyMenu();
            dailyMenu.SetActive(true);
        }
    }

    public void ToggleHelpMenu()
    {
        helpMenu.SetActive(!helpMenu.activeInHierarchy);
    }

    public void ToggleInGameControls(bool isActive)
    {
        inGameControls.SetActive(isActive);
    }

    public void HideAllMenus()
    {
        mainMenu.SetActive(false);
        aboutMenu.SetActive(false);
        settingsMenu.SetActive(false);
        classicLevelSelectMenu.SetActive(false);
        dungeonLevelSelectMenu.SetActive(false);
        cursedHouseLevelSelectMenu.SetActive(false);
        purchaseMenu.SetActive(false);
        statsMenu.SetActive(false);
        endGameMenu.SetActive(false);
        onReplayMenu.SetActive(false);
        dailyMenu.SetActive(false);
        inGameMenu.SetActive(false);
        helpMenu.SetActive(false);
        inGameControls.SetActive(false);
    }

    public void UpdateTextOnPurchaseMenuButton()
    {
        purchaseBtnCoinsText.text = $"{PlayerPrefs.GetInt("PlayersCoins", 0)}";
        if (coinText != null)
        {
            coinText.text = $"{PlayerPrefs.GetInt("PlayersCoins", 0)}";
        }
    }

    public void WatchAdToEarnCoins()
    {
        Advertisement.Show();
        DailyAdHandler.dailyUnlockAd = false;
    }

    // settings panel config
    public void ChangeLocal(int index)
    {
        ColorBlock normalBtnColor = ENBtn.colors;
        normalBtnColor.normalColor = Color.white;

        ColorBlock selectedBtnColor = ENBtn.colors;
        selectedBtnColor.normalColor = spookyOrange;

        if (LocalizationSettings.InitializationOperation.IsDone)
        {
            ENBtn.colors = normalBtnColor;
            FRBtn.colors = normalBtnColor;
            DEBtn.colors = normalBtnColor;
            ITBtn.colors = normalBtnColor;
            ESBtn.colors = normalBtnColor;

            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];

            if (index == 1)
            {
                FRBtn.colors = selectedBtnColor;
                FRBtn.Select();
                PlayerPrefs.SetString("selected-locale", "fr");
            }
            else if (index == 2)
            {
                DEBtn.colors = selectedBtnColor;
                DEBtn.Select();
                PlayerPrefs.SetString("selected-locale", "de");
            }
            else if (index == 3)
            {
                ITBtn.colors = selectedBtnColor;
                ITBtn.Select();
                PlayerPrefs.SetString("selected-locale", "it");
            }
            else if (index == 4)
            {
                ESBtn.colors = selectedBtnColor;
                ESBtn.Select();
                PlayerPrefs.SetString("selected-locale", "es");
            }
            else
            {
                ENBtn.colors = selectedBtnColor;
                ENBtn.Select();
                PlayerPrefs.SetString("selected-locale", "en");
            }
            PlayerPrefs.Save();
        }        
    }

    public void ChangeControls(int isTouch)
    {
        ColorBlock normalBtnColor = touchControl.colors;
        normalBtnColor.normalColor = Color.white;

        ColorBlock selectedBtnColor = touchControl.colors;
        selectedBtnColor.normalColor = spookyOrange;       

        if (isTouch == 1) // 1 is true, 0 is false
        {
            touchControl.colors = selectedBtnColor;
            btnControl.colors = normalBtnColor;
            PlayerPrefs.SetInt("isTouch", 1);
        }
        else
        {
            touchControl.colors = normalBtnColor;
            btnControl.colors = selectedBtnColor;
            PlayerPrefs.SetInt("isTouch", 0);
        }
        PlayerPrefs.Save();
    }

    private void HandleAndroidBackBtn()
    {
        if (SceneManager.sceneCount > 1)
        {
            GoToMainMenu();
        }
        else
        {
            if (aboutMenu.activeInHierarchy)
            {
                ToggleAboutMenu(false);
            }
            else if (classicLevelSelectMenu.activeInHierarchy)
            {
                ToggleClassicLevelSelectMenu("");
            }
            else if (dungeonLevelSelectMenu.activeInHierarchy)
            {
                ToggleDungeonLevelSelectMenu("");
            }
            else if (cursedHouseLevelSelectMenu.activeInHierarchy)
            {
                ToggleCursedHouseLevelSelectMenu("");
            }
            else if (dailyMenu.activeInHierarchy)
            {
                ToggleDailyMenu();
            }
            else if (purchaseMenu.activeInHierarchy)
            {
                TogglePurchaseMenu();
            }
            else if (statsMenu.activeInHierarchy)
            {
                ToggleStatsMenu();
            }
            else if (settingsMenu.activeInHierarchy)
            {
                ToggleSettingsMenu();
            }
            else if (helpMenu.activeInHierarchy)
            {
                ToggleHelpMenu();
            }
            else
            {
                Application.Quit();
            }
        }
    }
}