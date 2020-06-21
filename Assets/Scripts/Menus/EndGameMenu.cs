using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameMenu : Menu<EndGameMenu>
{
    private bool _levelCompleted;

    public TextMeshProUGUI NextPlay;
    public TMP_Text Score;
    public bool LevelCompleted { get => _levelCompleted; set => _levelCompleted = value; }

    protected override void Awake()
    {
        base.Awake();
        InstantiateScore();
    }

    private void InstantiateScore()
    {
        TMP_Text playerScore = Instantiate(Score);
        playerScore.transform.SetParent(EndGameMenu.Instance.transform, false);
        playerScore.transform.position = new Vector3(playerScore.transform.position.x, 150f, playerScore.transform.position.z);
        playerScore.fontSize = 80;
        playerScore.text = $"Score: {PlayerPrefs.GetInt("PlayerScore", 0)}";
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

    /// <summary>
    /// Replays player's movements from the start. Fired from FinishMenu.
    /// </summary>
    public void ReplayPlayersMovement()
    {
        EndGameMenu.Close();
        LevelManager.Instance.WatchReplay(
            onComplete: () => UIManager.Instance.ShowFinishMenu(_levelCompleted)
        );
    }

    /// <summary>
    /// Replays player's movements from finish to the start. Fired from FinishMenu.
    /// </summary>
    public void GoToNextLevel()
    {
        EndGameMenu.Close();
        OnReplayMenu.Open();
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
