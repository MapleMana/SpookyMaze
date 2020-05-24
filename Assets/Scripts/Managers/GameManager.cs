using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    private LevelState _levelState;
    private float _finalPlayerLightAngle;      // the player light angle at the end of the level

    public float timeDecrement;
    [Range(0f, 500f)]
    public float levelTime;
    public float replayMultiplier;
    public float reversedReplayMultiplier;

    public GameMode GameMode { get; set; }
    public int CurrentLevel { get; set; } = 1;
    public float TimeLeft { get; set; }

    public bool LevelIs(LevelState state) => (_levelState & state) != 0;
    public float ReplayTime => (levelTime - TimeLeft) * replayMultiplier;
    public float ReversedReplayTime => (levelTime - TimeLeft) * reversedReplayMultiplier;

    private GameManager() { }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnFullLoad;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnFullLoad;
    }

    private void Start()
    {
        GameMode = new ClassicGameMode();
        int levelsGenerated = PlayerPrefs.GetInt("generated", 0);
        if (levelsGenerated == 0)
        {
            LevelGenerator.GenerateLevels();
            PlayerPrefs.SetInt("generated", 1);
        }
    }

    private void OnFullLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Maze")
        {
            LoadLevel(CurrentLevel);
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
        TimeLeft += timeToAdd;
    }

    /// <summary>
    /// Loads the appropriate level from the file
    /// </summary>
    /// <param name="levelNumber">The level number to load</param>
    public void LoadLevel(int levelNumber)
    {
        _levelState = LevelState.InProgress;
        TimeLeft = levelTime;
        CurrentLevel = levelNumber;

        MazeState state = MazeState.LoadFrom($"/{levelNumber}.maze");
        Maze.Instance.Load(state);
        Maze.Instance.GenerateItems(GameMode.GetItems());
        Maze.Instance.SaveState();
        Maze.Instance.Display();
        
        Player.Instance.Controllable = true;
        Movable.ResetState();
        GameMode.Initialize();
    }

    /// <summary>
    /// Called when the level ends (player wins/loses)
    /// </summary>
    public void EndLevel(bool mazeCompleted)
    {
        _levelState = mazeCompleted ? LevelState.Completed : LevelState.Failed;
        Player.Instance.Controllable = Player.Instance.Moving = false;
        Ghost.CanBeMoved = false;
        _finalPlayerLightAngle = Player.Instance.Light.spotAngle;
       
        if (mazeCompleted)
        {
            LightManager.Instance.TurnOn();
            CameraManager.Instance.FocusOnMaze(Maze.Instance);
            levelTime -= timeDecrement;
            CurrentLevel += 1;
            UIManager.Instance.UnlockLevel(CurrentLevel);
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
        TimeLeft = ReplayTime;
        GameMode.Reset();
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
        TimeLeft = 0;
        LightManager.Instance.TurnOff();
        StartCoroutine(Movable.ReplayCommands(
            reversed: true,
            timeMultiplier: reversedReplayMultiplier,
            onComplete: () => LoadLevel(CurrentLevel)
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
            if (TimeLeft < 0)
            {
                EndLevel(mazeCompleted: false);
            }
            else
            {
                TimeLeft -= Time.deltaTime;
                Player.Instance.LerpLightAngle(coef: TimeLeft / levelTime);
            }

            if (GameMode.GameEnded())
            {
                EndLevel(mazeCompleted: true);
            }
        }
        else if (LevelIs(LevelState.InReplay))
        {
            if (TimeLeft > 0)
            {
                TimeLeft -= Time.deltaTime;// / replayMultiplier;
                Player.Instance.LerpLightAngle(
                    min: _finalPlayerLightAngle,
                    coef: TimeLeft / ReplayTime
                );
            }
        }
        else if (LevelIs(LevelState.InReplayReversed))
        {
            if (TimeLeft < ReversedReplayTime)
            {
                TimeLeft += Time.deltaTime;// / reversedReplayMultiplier;
                Player.Instance.LerpLightAngle(
                    min: _finalPlayerLightAngle,
                    coef: TimeLeft / ReversedReplayTime
                );
            }
        }
    }
}
