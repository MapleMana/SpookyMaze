using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum LevelState
{
    None,
    InProgress,
    Completed,
    Failed,
    InReplay,
    InReplayReversed
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    private int _mazeWidth;
    private int _mazeHeight;
    private float _timeLeft;
    private LevelState _levelState;
    private bool _mazeCompleted;
    private float _finalPlayerLightAngle;      // the player light angle at the end of the level

    public int initialMazeWidth;
    public int initialMazeHeight;
    public int mazeSizeIncrement;
    public float timeDecrement;
    [Range(0f, 500f)]
    public float levelTime = 20;
    public float replayTime = 10;
    public float reversedReplayTime = 5;
    public int levelReached = 1;

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
    public static GameManager Instance => _instance;
    public LevelState GameState => _levelState;
    
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
        }
    }

    /// <summary>
    /// Initializes, loads and displays the new maze
    /// </summary>
    public void StartNewLevel()
    {
        _levelState = LevelState.InProgress;
        _mazeCompleted = false;
        _timeLeft = levelTime;

        Maze.Instance.Initialize(_mazeWidth, _mazeHeight, new BranchedDFSGeneration());
        Maze.Instance.Generate();
        Maze.Instance.Display();

        Player.Instance.ResetState();
    }

    /// <summary>
    /// Called when the level ends (player wins/loses)
    /// </summary>
    public void EndLevel(bool mazeCompleted)
    {
        _mazeCompleted = mazeCompleted;
        _levelState = mazeCompleted ? LevelState.Completed : LevelState.Failed;
        Player.Instance.CanBeMoved = Player.Instance.Moving = false;
        _finalPlayerLightAngle = Player.Instance.PlayerLight.spotAngle;

        if (mazeCompleted)
        {
            LightManager.Instance.TurnOn();
            _mazeHeight += mazeSizeIncrement;
            _mazeWidth += mazeSizeIncrement;
            levelTime -= timeDecrement;
            levelReached += 1;
            UIManager.Instance.LoadLevels();
        }
        UIManager.Instance.ShowFinishMenu();
    }

    /// <summary>
    /// Replays player movements from the start.
    /// </summary>
    /// <param name="onComplete">Action to perform when the replay is complete</param>
    public void WatchReplay(Action onComplete)
    {
        _levelState = LevelState.InReplay;
        _timeLeft = replayTime;
        StartCoroutine(Player.Instance.PlayCommands(
            initialPosition: Maze.Instance.StartPos,
            playTime: replayTime,
            onComplete: onComplete
        ));
    }

    /// <summary>
    /// Replays the player movements reversely. Transitions to the next.
    /// </summary>
    public void GoToNextLevel()
    {
        _levelState = LevelState.InReplayReversed;
        _timeLeft = 0;
        LightManager.Instance.TurnOff();
        StartCoroutine(Player.Instance.PlayCommands(
            reversed: true,
            playTime: reversedReplayTime,
            onComplete: () => StartNewLevel()
        ));
    }

    public void Update()
    {
        switch (_levelState)
        {
            case LevelState.InProgress:
                if (_timeLeft < 0)
                {
                    EndLevel(mazeCompleted: false);
                }
                else
                {
                    _timeLeft -= Time.deltaTime;
                    Player.Instance.LerpLightAngle(coef: _timeLeft / levelTime);
                    CameraManager.Instance.FocusOnPlayer();
                }
                break;
            case LevelState.InReplay:
                if (_timeLeft > 0)
                {
                    _timeLeft -= Time.deltaTime;
                    Player.Instance.LerpLightAngle(
                        min: _finalPlayerLightAngle,
                        coef: _timeLeft / replayTime
                    );

                    if (_mazeCompleted)
                    {
                        CameraManager.Instance.FocusOnMaze(Maze.Instance);
                    }
                    else
                    {
                        CameraManager.Instance.FocusOnPlayer();
                    }
                }
                break;
            case LevelState.InReplayReversed:
                if (_timeLeft < reversedReplayTime)
                {
                    _timeLeft += Time.deltaTime;
                    Player.Instance.LerpLightAngle(
                        min: _finalPlayerLightAngle,
                        coef: _timeLeft / reversedReplayTime
                    );

                    if (_mazeCompleted)
                    {
                        CameraManager.Instance.FocusOnMaze(Maze.Instance);
                    }
                    else
                    {
                        CameraManager.Instance.FocusOnPlayer();
                    }
                }
                break;
            default:
                break;
        }
    }
}
