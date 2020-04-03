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
    public GameObject FinishMenu;

    public static UIManager Instance { get => _instance; set => _instance = value; }

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

    /// <summary>
    /// This method is invoked when the "Play" button is pressed, loads the new maze
    /// </summary>
    public void StartGame()
    {
        MainMenu.SetActive(false);
        GameManager.Instance.LoadLevel("Maze");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void Update()
    {

    }

    /// <summary>
    /// Displays the finish menu, when a player gets to the end point
    /// </summary>
    public void ShowFinishMenu()
    {
        LightManager.Instance.TurnOn();
        FinishMenu.SetActive(true);
    }

    /// <summary>
    /// Replays player's movements from the start
    /// </summary>
    public void WatchReplay()
    {
        FinishMenu.SetActive(false);
        StartCoroutine(Player.Instance.ReplayMovementsFromStart(
            onComplete: () => FinishMenu.SetActive(true)
        ));
    }

    /// <summary>
    /// Replays player's movements from finish to the start and reloads the maze
    /// </summary>
    public void GoToNextLevel()
    {
        FinishMenu.SetActive(false);
        StartCoroutine(Player.Instance.ReplayMovementsFromFinish());
    }

    /// <summary>
    /// Goes to the MainMenu Scene and displays the main menu again
    /// </summary>
    public void GoToMainMenu()
    {
        FinishMenu.SetActive(false);
        MainMenu.SetActive(true);
        LightManager.Instance.TurnOff();
        CameraManager.Instance.FocusOnMenu(MainMenu.transform.position);
        GameManager.Instance.LoadLevel("MainMenu");
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
