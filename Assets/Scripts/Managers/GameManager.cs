using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    private int _mazeWidth;
    private int _mazeHeight;
    private float _timeLeft;
    private bool _levelStarted = false;

    public int initialMazeWidth;
    public int initialMazeHeight;
    public int mazeSizeIncrement;
    public float timeDecrement;
    [Range(0f, 500f)]
    public float levelTime = 20.0f;

    public int MazeWidth
    {
        get => _mazeWidth;
        set
        {
            _mazeWidth = value > 0 ? value : _mazeWidth;
        }
    }
    public int MazeHeight
    {
        get => _mazeHeight;
        set
        {
            _mazeHeight = value > 0 ? value : _mazeHeight;
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
            StartNewLevel();

            _levelStarted = true;
            _timeLeft = levelTime;
        }
    }

    /// <summary>
    /// Initializes, loads and displays the new maze
    /// </summary>
    public void StartNewLevel()
    {
        Maze.Instance.Initialize(_mazeWidth, _mazeHeight, new BranchedDFSGeneration());
        Maze.Instance.Generate();
        Maze.Instance.Display();

        CameraManager.Instance.FocusOn(Maze.Instance);

        Player.Instance.ResetState();
    }

    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Called when the level ends (player wins/loses)
    /// </summary>
    public void EndLevel(bool mazeComplete)
    {
        _levelStarted = false;
        UIManager.Instance.ShowFinishMenu();
        Player.Instance.CanMove = false;
        if (mazeComplete)
        {
            _mazeHeight += mazeSizeIncrement;
            _mazeWidth += mazeSizeIncrement;
            levelTime -= timeDecrement;
        }
    }

    public void Update()
    {
        if (_levelStarted)
        {
            _timeLeft -= Time.deltaTime;
            if (_timeLeft < 0)
            {
                EndLevel(mazeComplete: false);
            }
            Player.Instance.SetLightAngle(_timeLeft / levelTime);
        }
    }
}
