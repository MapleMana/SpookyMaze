using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public const int NUM_OF_LEVELS = 10;

    private static GameManager _instance;

    private int _mazeWidth;
    private int _mazeHeight;
    private float _timeLeft;
    private LevelState _levelState;
    private GameMode _gameMode;
    private float _finalPlayerLightAngle;      // the player light angle at the end of the level
    private int _currentLevel = 1;

    public int initialMazeWidth;
    public int initialMazeHeight;
    public int mazeSizeIncrement;
    public float timeDecrement;
    [Range(0f, 500f)]
    public float levelTime;
    public float replayMultiplier;
    public float reversedReplayMultiplier;

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
    public GameMode GameMode { get => _gameMode; set => _gameMode = value; }
    public int CurrentLevel { get => _currentLevel; set => _currentLevel = value; }
    public float TimeLeft { get => _timeLeft; set => _timeLeft = value; }

    public bool LevelIs(LevelState state) => (_levelState & state) != 0;
    public float ReplayTime => (levelTime - _timeLeft) * replayMultiplier;
    public float ReversedReplayTime => (levelTime - _timeLeft) * reversedReplayMultiplier;

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

    private void Start()
    {
        _gameMode = new ClassicGameMode();
        Maze.Initialize();
        int levelsGenerated = PlayerPrefs.GetInt("generated", 0);
        if (levelsGenerated == 0)
        {
            GenerateLevels();
            PlayerPrefs.SetInt("generated", 1);
        }
    }

    private void OnFullLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Maze")
        {
            LoadLevel(_currentLevel);
        }
    }

    /// <summary>
    /// Adds the specified percentage of total time to current time
    /// </summary>
    /// <param name="ratio">The percentage of the total time to add</param>
    public void AddTime(float ratio)
    {
        float timeToAdd = 0;
        if (LevelIs(LevelState.InProgress))
        {
            timeToAdd = ratio * levelTime;
        }
        else if (LevelIs(LevelState.InReplay))
        {
            timeToAdd = ratio * ReplayTime;
        }
        else if (LevelIs(LevelState.InReplayReversed))
        {
            timeToAdd = ratio * ReversedReplayTime;
        }
        _timeLeft += timeToAdd;
    }

    /// <summary>
    /// Generates a new levels and saves them to a file. This method is for generation only and should not be used while gameplay.
    /// </summary>
    public void GenerateLevels()
    {
        for (int i = 0; i < NUM_OF_LEVELS; i++)
        {
            Maze.Instance.SetDimensions(_mazeWidth, _mazeHeight);
            new BranchedDFSGeneration().Generate();
            MazeState state = new MazeState(Maze.Instance);
            state.SaveTo($"/{i}.maze");
            _mazeHeight += mazeSizeIncrement;
            _mazeWidth += mazeSizeIncrement;
        }
    }

    /// <summary>
    /// Loads the appropriate level from the file
    /// </summary>
    /// <param name="levelNumber">The level number to load</param>
    public void LoadLevel(int levelNumber)
    {
        _levelState = LevelState.InProgress;
        _timeLeft = levelTime;
        _currentLevel = levelNumber;

        MazeState state = MazeState.LoadFrom($"/{levelNumber}.maze");
        state.Load();
        Maze.Instance.GenerateItems(_gameMode.GetItems());
        Maze.Instance.SaveState();
        Maze.Instance.Display();
        
        Player.Instance.Controllable = true;
        Movable.ResetState();
        _gameMode.Initialize();
    }

    /// <summary>
    /// Called when the level ends (player wins/loses)
    /// </summary>
    public void EndLevel(bool mazeCompleted)
    {
        _levelState = mazeCompleted ? LevelState.Completed : LevelState.Failed;
        Player.Instance.Controllable = Player.Instance.Moving = false;
        Ghost.CanBeMoved = false;
        _finalPlayerLightAngle = Player.Instance.PlayerLight.spotAngle;
       
        if (mazeCompleted)
        {
            LightManager.Instance.TurnOn();
            CameraManager.Instance.FocusOnMaze(Maze.Instance);
            levelTime -= timeDecrement;
            _currentLevel += 1;
            UIManager.Instance.UnlockLevel(_currentLevel);
        }
        UIManager.Instance.ShowFinishMenu(mazeCompleted);
    }

    /// <summary>
    /// Replays player movements from the start.
    /// </summary>
    /// <param name="onComplete">Action to perform when the replay is complete</param>
    public void WatchReplay(Action onComplete)
    {
        Maze.Instance.Restore();
        Maze.Instance.Display();
        _levelState |= LevelState.InReplay;
        _timeLeft = ReplayTime;
        _gameMode.Reset();
        StartCoroutine(Movable.ReplayCommands(
            timeMultiplier: replayMultiplier,
            onComplete: () => {
                _levelState ^= LevelState.InReplay;
                onComplete();
            }
        ));
    }

    /// <summary>
    /// Replays the player movements reversely.
    /// Either renders the same level or the next one.
    /// </summary>
    public void LoadCurrentLevel()
    {
        _levelState |= LevelState.InReplayReversed;
        _timeLeft = 0;
        LightManager.Instance.TurnOff();
        StartCoroutine(Movable.ReplayCommands(
            reversed: true,
            timeMultiplier: reversedReplayMultiplier,
            onComplete: () => LoadLevel(_currentLevel)
        ));
    }
    
    public void Update()
    {
        if (LevelIs(LevelState.InProgress) || 
           (!LevelIs(LevelState.Completed) && LevelIs(LevelState.InReplay | LevelState.InReplayReversed)))
        {
            CameraManager.Instance.FocusOnPlayer();
        }

        if (LevelIs(LevelState.InProgress))
        {
            if (_timeLeft < 0)
            {
                EndLevel(mazeCompleted: false);
            }
            else
            {
                _timeLeft -= Time.deltaTime;
                Player.Instance.LerpLightAngle(coef: _timeLeft / levelTime);
            }

            if (_gameMode.GameEnded())
            {
                EndLevel(mazeCompleted: true);
            }
        }
        else if (LevelIs(LevelState.InReplay))
        {
            if (_timeLeft > 0)
            {
                _timeLeft -= Time.deltaTime;// / replayMultiplier;
                Player.Instance.LerpLightAngle(
                    min: _finalPlayerLightAngle,
                    coef: _timeLeft / ReplayTime
                );
            }
        }
        else if (LevelIs(LevelState.InReplayReversed))
        {
            if (_timeLeft < ReversedReplayTime)
            {
                _timeLeft += Time.deltaTime;// / reversedReplayMultiplier;
                Player.Instance.LerpLightAngle(
                    min: _finalPlayerLightAngle,
                    coef: _timeLeft / ReversedReplayTime
                );
            }
        }
    }
}
