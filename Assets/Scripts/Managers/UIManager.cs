using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    public GameObject aboutMenu;
    public GameObject dailyMenu;
    public GameObject endGameMenu;
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
    public GameObject helpMenu;
    public Image nextPlayButtonImage;
    public Sprite nextLevelImage;
    public Sprite replayLevelImage;

    public Text purchaseBtnCoinsText;

    public Text coinText;

    private bool _levelCompleted;
    public bool LevelCompleted { get => _levelCompleted; set => _levelCompleted = value; }

    private void Start()
    {
        UpdateTextOnPurchaseMenuButton();
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
        ToggleEndGameMenu();
        LevelCompleted = mazeCompleted;
        SetNextActionText();
        coinText.text = $"{PlayerPrefs.GetInt("PlayersCoins", 0)}";
    }

    /// <summary>
    /// Sets the text on the button for showing the next available action
    /// </summary>
    public void SetNextActionText()
    {
        nextPlayButtonImage.GetComponent<Image>().sprite = _levelCompleted ? nextLevelImage : replayLevelImage;
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
        inGameMenu.SetActive(!inGameMenu.activeInHierarchy);
    }

    public void ToggleAboutMenu(bool active)
    {
        aboutMenu.SetActive(active);
    }

    public void ToggleSettingsMenu()
    {
        settingsMenu.SetActive(!settingsMenu.activeInHierarchy);
    }

    public void ToggleLevelSelectMenu(string modeName)
    {
        if (classicLevelSelectMenu.activeInHierarchy)
        {
            classicLevelSelectMenu.GetComponent<LevelSelectMenu>().ClearPanel();
            classicLevelSelectMenu.SetActive(false);
        }
        else
        {
            GameManager.Instance.CurrentSettings.gameMode = modeName;
            GameManager.Instance.CurrentSettings.isDaily = false;
            classicLevelSelectMenu.GetComponent<LevelSelectMenu>().LoadDimensions();
            classicLevelSelectMenu.SetActive(true);
        }
    }

    public void TogglePurchaseMenu()
    {
        purchaseMenu.SetActive(!purchaseMenu.activeInHierarchy);
    }

    public void ToggleStatsMenu()
    {
        statsMenu.SetActive(!statsMenu.activeInHierarchy);
    }

    public void ToggleEndGameMenu()
    {
       if(!endGameMenu.activeInHierarchy)
       {
            endGameMenu.SetActive(true);
            endGameNextLevelButton.interactable = !GameManager.Instance.IsLastLevel();
       }
       else
       {
            endGameMenu.SetActive(false);
       }
    }

    public void ToggleOnReplyMenu()
    {
        onReplayMenu.SetActive(!onReplayMenu.activeInHierarchy);
    }

    public void ToggleDailyMenu()
    {
        dailyMenu.SetActive(!dailyMenu.activeInHierarchy);
    }

    public void GoToDailyMenu()
    {
        LevelGenerator.GenerateDailyLevels();
        dailyMenu.SetActive(true);
    }

    public void ToggleHelpMenu()
    {
        helpMenu.SetActive(!helpMenu.activeInHierarchy);
    }

    public void HideAllMenus()
    {
        classicLevelSelectMenu.GetComponent<LevelSelectMenu>().ClearPanel();
        mainMenu.SetActive(false);
        aboutMenu.SetActive(false);
        settingsMenu.SetActive(false);
        classicLevelSelectMenu.SetActive(false);
        purchaseMenu.SetActive(false);
        statsMenu.SetActive(false);
        endGameMenu.SetActive(false);
        onReplayMenu.SetActive(false);
        dailyMenu.SetActive(false);
        inGameMenu.SetActive(false);
        helpMenu.SetActive(false);
    }

    public void UpdateTextOnPurchaseMenuButton()
    {
        purchaseBtnCoinsText.text = $"{PlayerPrefs.GetInt("PlayersCoins", 0)}";
    }
}