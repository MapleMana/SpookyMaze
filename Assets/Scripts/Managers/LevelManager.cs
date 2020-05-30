﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    private LevelState _levelState;
    private float LevelTime;
    private float _finalPlayerLightAngle;      // the player light angle at the end of the level

    public GameMode GameMode { get; set; }
    public int LevelNumber { get; set; } = 1;
    public float TimeLeft { get; set; }
    public bool LevelIs(LevelState state) => (_levelState & state) != 0;
    public float ReplayTime => (LevelTime - TimeLeft) * GameManager.Instance.replayMultiplier;
    public float ReversedReplayTime => (LevelTime - TimeLeft) * GameManager.Instance.reversedReplayMultiplier;

    /// <summary>
    /// Temporary copy of the above method
    /// </summary>
    /// <param name="levelNumber"></param>
    /// <param name="levelTime"></param>
    public void Initialize(int levelNumber, LevelStatus levelStatus, GameMode gameMode)
    {
        _levelState = LevelState.InProgress;
        TimeLeft = LevelTime = levelStatus.time;
        LevelNumber = levelNumber;
        GameMode = gameMode;

        Maze.Instance.Clear();
        Maze.Instance.Load(levelStatus.mazeState);

        Maze.Instance.SaveState();
        Maze.Instance.Display();

        Movable.ResetState();
        GameMode.Initialize();
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
            timeToAdd = ratio * LevelTime;
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
            timeMultiplier: GameManager.Instance.replayMultiplier,
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
            timeMultiplier: GameManager.Instance.reversedReplayMultiplier,
            onComplete: () => GameManager.Instance.LoadLevel(LevelNumber)
        ));
    }

    /// <summary>
    /// Called when the level ends (player wins/loses)
    /// </summary>
    public void EndLevel(bool mazeCompleted)
    {
        _levelState = mazeCompleted ? LevelState.Completed : LevelState.Failed;
        Ghost.CanBeMoved = false;
        _finalPlayerLightAngle = Player.Instance.Light.spotAngle;

        if (mazeCompleted)
        {
            LightManager.Instance.TurnOn();
            CameraManager.Instance.FocusOnMaze(Maze.Instance);
            UIManager.Instance.UnlockLevel(++LevelNumber);
        }
        UIManager.Instance.ShowFinishMenu(mazeCompleted);
    }

    void Update()
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
                Player.Instance.LerpLightAngle(coef: TimeLeft / LevelTime);
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
