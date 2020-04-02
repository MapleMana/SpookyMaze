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
    public GameObject menu;

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
    /// This method is invoked when the "Play" button is pressed
    /// </summary>
    public void StartGame()
    {
        GameManager.Instance.LoadLevel("Maze");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void Update()
    {

    }

    public void ShowMenu()
    {
        LightManager.Instance.TurnOn();
        menu.gameObject.SetActive(true);
    }

    public void PlayAgain()
    {
        menu.gameObject.SetActive(false);
        StartCoroutine(Player.Instance.ReplayMovementsFromFinish());
    }

    public void GoToMainMenu()
    {
        menu.gameObject.SetActive(false);
        Player.Instance.ReplayMovementsFromStart();
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
