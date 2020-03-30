using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private int mazeWidth;
    private int mazeHeight;
    private bool reload = false;

    private static UIManager _instance;

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

    public void SliderWidthChanged(int newWidth)
    {
        mazeWidth = newWidth;
        reload = true;
    }

    public void SliderHeightChanged(int newHeight)
    {
        mazeHeight = newHeight;
        reload = true;
    }

    public void ReloadMazeGeneration()
    {
        if (reload)
        {
            GameManager.GenerateMaze(mazeWidth, mazeHeight);
        }
    }
}
