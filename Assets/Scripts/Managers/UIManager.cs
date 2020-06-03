using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{    
    public TextMeshProUGUI NextPlay;

    /// <summary>
    /// This method loads the new maze and starts the game
    /// </summary>
    public void StartGame()
    {
        LevelSelectMenu.Close();
        MainMenu.Close();
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
        NextPlay.text = mazeCompleted ? "Go to the Next Level" : "Play Again";
    }

    /// <summary>
    /// Replays player's movements from the start. Fired from FinishMenu.
    /// </summary>
    public void WatchReplay()
    {
        EndGameMenu.Close();
        LevelManager.Instance.WatchReplay(
            onComplete: () => EndGameMenu.Open()
        );
    }

    /// <summary>
    /// Replays player's movements from finish to the start. Fired from FinishMenu.
    /// </summary>
    public void GoToNextLevel()
    {
        EndGameMenu.Close();
        LevelManager.Instance.LoadCurrentLevel();
    }

    /// <summary>
    /// Goes to the MainMenu Scene and displays the main menu again
    /// </summary>
    public void GoToMainMenu()
    {
        EndGameMenu.Close();
        MainMenu.Open();
        LightManager.Instance.TurnOff();
        SceneManager.UnloadSceneAsync("Maze");
        CameraManager.Instance.FocusOnMenu(MainMenu.Instance.gameObject.transform.position);
    }
}
