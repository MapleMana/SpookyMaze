using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameMenu : Menu<EndGameMenu>
{
    public TextMeshProUGUI NextPlay;

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

    public void SetNextActionText(bool mazeCompleted)
    {
        NextPlay.text = mazeCompleted ? "Go to the Next Level" : "Play Again";
    }
}
