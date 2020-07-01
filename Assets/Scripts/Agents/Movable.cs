﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movable : MonoBehaviour
{
    private Vector2Int _mazePosition;
    private static float _previousCommandTime;
    protected static List<KeyValuePair<Movable, MovableCommand>> _commandHistory;
    protected Vector3 _target;

    public float speed;

    public bool Moving { get; set; } = false;
    public bool AtMazeEnd => MazePosition == Maze.Instance.EndPos;

    public Vector2Int StartingPosition { get; set; }

    public Vector2Int MazePosition {
        get => _mazePosition;
        set
        {
            _mazePosition = value;
            MazeCell currentCell = Maze.Instance[value];
            _target = currentCell.CellCenter(y: transform.position.y);
        }
    }

    public void SetMazePositionWithoutLerp(Vector2Int value)
    {
        _mazePosition = value;
        MazeCell currentCell = Maze.Instance[value];
        _target = transform.position = currentCell.CellCenter(y: transform.position.y);
    }

    protected virtual void Awake()
    {
        _commandHistory = new List<KeyValuePair<Movable, MovableCommand>>();
        _target = transform.position;
    }

    protected virtual void Update()
    {
        if (!Moving && LevelManager.Instance.LevelIs(LevelState.InProgress))
        {
            PerformMovement();
        }
        if (Moving || 
            LevelManager.Instance.LevelIs(LevelState.InReplay) || 
            LevelManager.Instance.LevelIs(LevelState.InReplayReversed))
        {
            // TODO: simulate constant movement
            transform.position = Vector3.Lerp(transform.position, _target, 1 / speed);
        }
    }

    /// <summary>
    /// Is being called on update. Complex movement logic.
    /// </summary>
    public abstract void PerformMovement();

    /// <summary>
    /// Atomic (between 2 cells) movement logic
    /// </summary>
    /// <param name="direction"></param>
    /// <returns>true if the movement succeeded</returns>
    public abstract bool Move(Vector2Int direction);

    public static void ClearHistory()
    {
        _commandHistory.Clear();
        _previousCommandTime = Time.time;
    }

    public void Reset()
    {
        SetMazePositionWithoutLerp(StartingPosition);
    }

    public void AddToHistory(Movable movingObject, MovableCommand command)
    {
        float timeDiff = Time.time - _previousCommandTime;
        _previousCommandTime = Time.time;
        _commandHistory.Add(new KeyValuePair<Movable, MovableCommand>(movingObject, MovableCommand.CreateIdle(timeDiff)));
        _commandHistory.Add(new KeyValuePair<Movable, MovableCommand>(movingObject, command));
    }

    /// <summary>
    /// Replays player command history
    /// </summary>
    /// <param name="reversed">Whether the execution should be reversed (both order and individual commands)</param>
    /// <param name="initialPosition">The starting position of the player. If null, current position is taken.</param>
    /// <param name="timeMultiplier">The number of times the replay should be sped up</param>
    /// <param name="onComplete">Action to perform after the replay is comlete</param>
    /// <returns>A coroutine to execute</returns>
    public static IEnumerator ReplayCommands(
        bool reversed = false,
        float timeMultiplier = 1,
        Action onComplete = null)
    {
        if (reversed)
        {
            _commandHistory.Reverse();
        }

        foreach (var command in _commandHistory)
        {
            float executionTime = reversed
                ? command.Value.ExecuteReversed(command.Key).Time
                : command.Value.Execute(command.Key).Time;
            yield return new WaitForSeconds(executionTime * timeMultiplier);
        }
        onComplete?.Invoke();
    }

    /// <summary>
    /// Executes commands to decision point and saves them to history
    /// </summary>
    /// <param name="playerCommands"></param>
    /// <param name="waitBefore"></param>
    /// <returns></returns>
    protected IEnumerator PlayCommandsInRealTime(
        List<MovableCommand> playerCommands,
        bool waitBefore=false)
    {
        //pauseBetweenCommands -= Time.deltaTime;

        Moving = true;

        if (waitBefore)
        {
            yield return new WaitForSeconds(1 / speed);
        }

        foreach (MovableCommand command in playerCommands)
        {
            if (Moving)
            {
                command.Execute(this);
                AddToHistory(this, command);

                yield return new WaitForSeconds(1 / speed);
            }
        }
        Moving = false;
    }
}
