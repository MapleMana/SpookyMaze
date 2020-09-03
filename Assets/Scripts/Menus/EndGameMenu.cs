using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameMenu : MonoBehaviour
{
    private bool _levelCompleted;

    public TextMeshProUGUI NextPlay;
    public TMP_Text Coins;
    public bool LevelCompleted { get => _levelCompleted; set => _levelCompleted = value; }

    /// <summary>
    /// Goes to the MainMenu Scene and displays the main menu again
    /// </summary>
    public void GoToMainMenu()
    {
        LightManager.Instance.TurnOff();
        SceneManager.UnloadSceneAsync("Maze");
    }

    /// <summary>
    /// Replays player's movements from the start. Fired from FinishMenu.
    /// </summary>
    public void ReplayPlayersMovement()
    {
        LevelManager.Instance.WatchReplay(
            onComplete: () => UIManager.Instance.ShowFinishMenu(_levelCompleted)
        );
    }

    /// <summary>
    /// Replays player's movements from finish to the start. Fired from FinishMenu.
    /// </summary>
    public void GoToNextLevel()
    {
        LevelManager.Instance.LoadCurrentLevel();
    }

    /// <summary>
    /// Sets the text on the button for showing the next available action
    /// </summary>
    public void SetNextActionText()
    {
        NextPlay.text = _levelCompleted ? "Go to the Next Level" : "Play Again";
    }
}
