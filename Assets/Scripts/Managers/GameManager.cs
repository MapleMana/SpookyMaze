﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Flags]
public enum GameMode
{
    Classic = 0,
    Door = 1,
    Ghost = 2
}

[Flags]
public enum LevelState
{
    None = 0,
    InProgress = 1,
    Completed = 2,
    Failed = 4,
    InReplay = 8,
    InReplayReversed = 16
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    private int _mazeWidth;
    private int _mazeHeight;
    private float _timeLeft;
    private LevelState _levelState;
    private float _finalPlayerLightAngle;      // the player light angle at the end of the level

    public int initialMazeWidth;
    public int initialMazeHeight;
    public int mazeSizeIncrement;
    public float timeDecrement;
    [Range(0f, 500f)]
    public float levelTime = 20;
    public float replayTime = 10;
    public float reversedReplayTime = 5;

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
    public bool LevelIs(LevelState state) => (_levelState & state) != 0;


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
        _levelState = mazeCompleted ? LevelState.Completed : LevelState.Failed;
        Player.Instance.CanBeMoved = Player.Instance.Moving = false;
        _finalPlayerLightAngle = Player.Instance.PlayerLight.spotAngle;
        if (mazeCompleted)
        {
            LightManager.Instance.TurnOn();
            _mazeHeight += mazeSizeIncrement;
            _mazeWidth += mazeSizeIncrement;
            levelTime -= timeDecrement;
        }
        UIManager.Instance.ShowFinishMenu();
    }

    /// <summary>
    /// Replays player movements from the start.
    /// </summary>
    /// <param name="onComplete">Action to perform when the replay is complete</param>
    public void WatchReplay(Action onComplete)
    {
        _levelState |= LevelState.InReplay;
        _timeLeft = replayTime;
        StartCoroutine(Player.Instance.PlayCommands(
            initialPosition: Maze.Instance.StartPos,
            playTime: replayTime,
            onComplete: () => {
                _levelState ^= LevelState.InReplay;
                onComplete();
            }
        ));
    }

    /// <summary>
    /// Replays the player movements reversely. Transitions to the next.
    /// </summary>
    public void GoToNextLevel()
    {
        _levelState |= LevelState.InReplayReversed;
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
                CameraManager.Instance.FocusOnPlayer();
            }
        }
        else if (LevelIs(LevelState.InReplay))
        {
            if (_timeLeft > 0)
            {
                _timeLeft -= Time.deltaTime;
                Player.Instance.LerpLightAngle(
                    min: _finalPlayerLightAngle,
                    coef: _timeLeft / replayTime
                );

                if (LevelIs(LevelState.Completed))
                {
                    CameraManager.Instance.FocusOnMaze(Maze.Instance);
                }
                else
                {
                    CameraManager.Instance.FocusOnPlayer();
                }
            }
        }
        else if (LevelIs(LevelState.InReplayReversed))
        {
            if (_timeLeft < reversedReplayTime)
            {
                _timeLeft += Time.deltaTime;
                Player.Instance.LerpLightAngle(
                    min: _finalPlayerLightAngle,
                    coef: _timeLeft / reversedReplayTime
                );

                if (LevelIs(LevelState.Completed))
                {
                    CameraManager.Instance.FocusOnMaze(Maze.Instance);
                }
                else
                {
                    CameraManager.Instance.FocusOnPlayer();
                }
            }
        }
    }
}
