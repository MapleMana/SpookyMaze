using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public GameObject aboutMenu;
    public GameObject settingsMenu;
    public GameObject levelSelectMenu;

    /// <summary>
    /// This method loads the new maze and starts the game
    /// </summary>
    public void StartGame()
    {
        MenuManager.Instance.ClearMenuStack();
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
        EndGameMenu.Open();
        EndGameMenu.Instance.LevelCompleted = mazeCompleted;
        EndGameMenu.Instance.SetNextActionText();
    }

    // show and hide panels
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
}
