﻿using System;
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
    public GameObject SettingsMenu;

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
        GameManager.Instance.WatchReplay();
    }

    /// <summary>
    /// Replays player's movements from finish to the start and reloads the maze
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
