﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    public GameObject mainMenu;
    public GameObject aboutMenu;
    public GameObject settingsMenu;
    public GameObject levelSelectMenu;
    public GameObject purchaseMenu;
    public GameObject statsMenu;
    public GameObject endGameMenu;
    public TMP_Text nextPlayButtonText;
    public GameObject onReplyMenu;

    public TMP_Text coinText;

    private bool _levelCompleted;
    public bool LevelCompleted { get => _levelCompleted; set => _levelCompleted = value; }

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
        nextPlayButtonText.text = _levelCompleted ? "Go to the Next Level" : "Play Again";
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
        if (mainMenu.activeInHierarchy)
        {
            mainMenu.SetActive(false);
        }
        else
        {
            mainMenu.SetActive(true);
        }
    }

    public void ToggleAboutMenu()
    {
        if (aboutMenu.activeInHierarchy)
        {
            aboutMenu.SetActive(false);
        }
        else
        {
            aboutMenu.SetActive(true);
        }
    }

    public void ToggleSettingsMenu()
    {
        if (settingsMenu.activeInHierarchy)
        {
            settingsMenu.SetActive(false);
        }
        else
        {
            settingsMenu.SetActive(true);
        }
    }

    public void ToggleLevelSelectMenu(string modeName)
    {
        if (levelSelectMenu.activeInHierarchy)
        {
            levelSelectMenu.GetComponent<LevelSelectMenu>().ClearPanel();
            levelSelectMenu.SetActive(false);
        }
        else
        {
            GameManager.Instance.CurrentSettings.gameMode = modeName;
            levelSelectMenu.GetComponent<LevelSelectMenu>().LoadDimensions();
            levelSelectMenu.SetActive(true);
        }
    }

    public void TogglePurchaseMenu()
    {
        if (purchaseMenu.activeInHierarchy)
        {
            purchaseMenu.SetActive(false);
        }
        else
        {
            purchaseMenu.SetActive(true);
        }
    }

    public void ToggleStatsMenu()
    {
        if (statsMenu.activeInHierarchy)
        {
            statsMenu.SetActive(false);
        }
        else
        {
            statsMenu.SetActive(true);
        }
    }

    public void ToggleEndGameMenu()
    {
        if (endGameMenu.activeInHierarchy)
        {
            endGameMenu.SetActive(false);
        }
        else
        {
            endGameMenu.SetActive(true);
        }
    }

    public void ToggleOnReplyMenu()
    {
        if (onReplyMenu.activeInHierarchy)
        {
            onReplyMenu.SetActive(false);
        }
        else
        {
            onReplyMenu.SetActive(true);
        }
    }

    public void HideAllMenus()
    {
        levelSelectMenu.GetComponent<LevelSelectMenu>().ClearPanel();
        mainMenu.SetActive(false);
        aboutMenu.SetActive(false);
        settingsMenu.SetActive(false);
        levelSelectMenu.SetActive(false);
        purchaseMenu.SetActive(false);
        statsMenu.SetActive(false);
        endGameMenu.SetActive(false);
        onReplyMenu.SetActive(false);
    }
}
