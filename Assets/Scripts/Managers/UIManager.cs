using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{    
    public Text Width;
    public Text Height;
    public TextMeshProUGUI NextPlay;

    public GameObject MainMenu;
    public GameObject LevelSelect;
    public GameObject FinishMenu;
    public GameObject SettingsMenu;
    public Toggle Key;
    public Toggle Oil;
    public Toggle Ghost;

    public GameObject ButtonsPanel;
    public Button ButtonTemplate;
    public List<Button> buttonList;

    private void Start()
    {
        LoadLevels();
    }

    /// <summary>
    /// Executed when one of the level select buttons is pressed
    /// </summary>
    /// <param name="levelNumber">The level number to load</param>
    /// <returns></returns>
    public UnityEngine.Events.UnityAction OnLevelOptionClick(int levelNumber)
    {
        return () =>
            {
                GameManager.Instance.CurrentLevel = levelNumber;
                StartGame();
            };
    }

    /// <summary>
    /// Invoked when the game starts and loads level buttons to the Level Select screen
    /// </summary>
    public void LoadLevels()
    {
        buttonList = new List<Button>();
        int levelReached = PlayerPrefs.GetInt("levelReached", 1);

        for (int i = 1; i <= LevelGenerator.NUM_OF_LEVELS; i++)
        {
            Button newButton = Instantiate(ButtonTemplate);
            newButton.GetComponentInChildren<Text>().text = i.ToString();
            newButton.onClick.AddListener(OnLevelOptionClick(i));
            newButton.interactable = (i <= levelReached);
            newButton.transform.SetParent(ButtonsPanel.transform, false);

            buttonList.Add(newButton);
        }
    }

    /// <summary>
    /// Makes "levelNumber" button interactable
    /// </summary>
    /// <param name="levelNumber">Level to be inlocked</param>
    public void UnlockLevel(int levelNumber)
    {
        buttonList[levelNumber - 1].interactable = true;
    }

    /// <summary>
    /// This method loads the new maze and starts the game
    /// </summary>
    public void StartGame()
    {
        LevelSelect.SetActive(false);
        MainMenu.SetActive(false);
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
        FinishMenu.SetActive(true);
        NextPlay.text = mazeCompleted ? "Go to the Next Level" : "Play Again";
    }

    /// <summary>
    /// Replays player's movements from the start. Fired from FinishMenu.
    /// </summary>
    public void WatchReplay()
    {
        FinishMenu.SetActive(false);
        GameManager.Instance.WatchReplay(
            onComplete: () => FinishMenu.SetActive(true)
        );
    }

    /// <summary>
    /// Replays player's movements from finish to the start. Fired from FinishMenu.
    /// </summary>
    public void GoToNextLevel()
    {
        FinishMenu.SetActive(false);
        GameManager.Instance.LoadCurrentLevel();
    }

    /// <summary>
    /// Goes to the MainMenu Scene and displays the main menu again
    /// </summary>
    public void GoToMainMenu()
    {
        FinishMenu.SetActive(false);
        MainMenu.SetActive(true);
        LightManager.Instance.TurnOff();
        SceneManager.UnloadSceneAsync("Maze");
        CameraManager.Instance.FocusOnMenu(MainMenu.transform.position);
    }

    public void ModeToggled()
    {
        if (Key.isOn)
        {
            GameManager.Instance.GameMode = new DoorKeyGameMode();
        }
        if (Oil.isOn)
        {
            GameManager.Instance.GameMode = new OilGameMode();
        }
        if (Ghost.isOn)
        {
            GameManager.Instance.GameMode = new GhostGameMode();
        }
    }
}
