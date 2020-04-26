using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{    
    private static UIManager _instance;

    public Text Width;
    public Text Height;
    public GameObject MainMenu;
    public GameObject LevelSelect;
    public GameObject FinishMenu;
    public GameObject SettingsMenu;
    public Button[] levelButtons;

    public static UIManager Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        WidthChanged(GameManager.Instance.MazeWidth);
        HeightChanged(GameManager.Instance.MazeHeight);

        LoadLevels();
    }

    public void LoadLevels()
    {
        int levelReached = GameManager.Instance.levelReached;

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if(i + 1 > levelReached)
            {
                levelButtons[i].interactable = false;
            }
            else
            {
                levelButtons[i].interactable = true;
            }
        }
    }

    /// <summary>
    /// This method is invoked when the "Play" button is pressed, loads the new maze
    /// </summary>
    public void StartGame()
    {
        LevelSelect.SetActive(false);
        SceneManager.LoadScene("Maze");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Displays the finish menu, when a player gets to the end point
    /// </summary>
    public void ShowFinishMenu()
    {
        FinishMenu.SetActive(true);
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
        GameManager.Instance.GoToNextLevel();
    }

    /// <summary>
    /// Goes to the MainMenu Scene and displays the main menu again
    /// </summary>
    public void GoToMainMenu()
    {
        FinishMenu.SetActive(false);
        MainMenu.SetActive(true);
        LightManager.Instance.TurnOff();
        SceneManager.LoadScene("MainMenu");
        CameraManager.Instance.FocusOnMenu(MainMenu.transform.position);
    }

    /// <summary>
    /// Called when slider value is changed and passes the new width to the GM
    /// </summary>
    /// <param name="width"></param>
    public void WidthChanged(float width)
    {
        Width.text = width.ToString();
        GameManager.Instance.MazeWidth = (int) width;
    }

    /// <summary>
    /// Called when slider value is changed and passes the new height to the GM
    /// </summary>
    /// <param name="height"></param>
    public void HeightChanged(float height)
    {
        Height.text = height.ToString();
        GameManager.Instance.MazeHeight = (int) height;
    }
}
