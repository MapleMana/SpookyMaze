using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{    
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
}
