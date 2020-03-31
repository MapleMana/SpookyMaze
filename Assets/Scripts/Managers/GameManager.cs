using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    private int _mazeWidth;
    private int _mazeHeight;

    public int initialMazeWidth = 10;
    public int initialMazeHeight = 10;

    // public setters and getters for _mazeWidth and _mazeHeight
    public int MazeWidth
    {
        get
        {
            return this._mazeWidth;
        }
        set
        {
            this._mazeWidth = value;
        }
    }
    public int MazeHeight
    {
        get
        {
            return this._mazeHeight;
        }
        set
        {
            this._mazeHeight = value;
        }
    }

    public static GameManager Instance { get => _instance; }

    private GameManager() { }

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

    private void OnEnable()
    {
        _mazeWidth = initialMazeWidth;
        _mazeHeight = initialMazeHeight;
        SceneManager.sceneLoaded += OnFullLoad;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnFullLoad;
    }

    private void OnFullLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Maze")
        {
            Maze.Instance.Initialize(_mazeWidth, _mazeHeight);
            Maze.Instance.Generate();
            Maze.Instance.Display();

            CameraManager.Instance.FocusOn(Maze.Instance);

            Player.Instance.PlaceOnMaze();
        }
    }

    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
