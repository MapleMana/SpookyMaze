using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movable : MonoBehaviour
{
    internal Vector2Int _mazePosition;
    internal List<PlayerCommand> _commandHistory;
    internal bool _moving = false;
    internal float _previousCommandTime;

    public bool Moving { get => _moving; set => _moving = value; }
    public bool AtMazeEnd => _mazePosition == Maze.Instance.EndPos;

    public abstract bool Move(Vector2Int direction);

    void Awake()
    {
        _commandHistory = new List<PlayerCommand>();
    }

    /// <summary>
    /// Synchronizes maze position and physical player position
    /// </summary>
    internal void SyncRealPosition()
    {
        MazeCell currentCell = Maze.Instance[_mazePosition];
        transform.position = currentCell.CellCenter(y: transform.position.y);
    }

    public void AddToHistory(PlayerCommand command)
    {
        float timeDiff = Time.time - _previousCommandTime;
        _previousCommandTime = Time.time;
        _commandHistory.Add(PlayerCommand.CreateIdle(timeDiff));
        _commandHistory.Add(command);
    }

    /// <summary>
    /// Replays player command history
    /// </summary>
    /// <param name="reversed">Whether the execution should be reversed (both order and individual commands)</param>
    /// <param name="initialPosition">The starting position of the player. If null, current position is taken.</param>
    /// <param name="timeMultiplier">The number of times the replay should be sped up</param>
    /// <param name="onComplete">Action to perform after the replay is comlete</param>
    /// <returns>A coroutine to execute</returns>
    internal IEnumerator ReplayCommands(
        bool reversed = false,
        Vector2Int? initialPosition = null,
        float timeMultiplier = 1,
        Action onComplete = null)
    {
        if (reversed)
        {
            _commandHistory.Reverse();
        }
        _mazePosition = initialPosition ?? _mazePosition;
        //pauseBetweenCommands -= Time.deltaTime;

        SyncRealPosition();

        Moving = true;
        foreach (PlayerCommand command in _commandHistory)
        {
            if (Moving)
            {
                float executionTime = reversed
                    ? command.ExecuteReversed(this).Time
                    : command.Execute(this).Time;
                yield return new WaitForSeconds(executionTime * timeMultiplier);
            }
        }
        Moving = false;
        onComplete?.Invoke();
    }

    /// <summary>
    /// Executes commands to decision point and saves them to history
    /// </summary>
    /// <param name="playerCommands"></param>
    /// <param name="pauseBetween"></param>
    /// <returns></returns>
    internal IEnumerator PlayCommandsInRealTime(
        List<PlayerCommand> playerCommands,
        float pauseBetween)
    {
        //pauseBetweenCommands -= Time.deltaTime;

        Moving = true;
        foreach (PlayerCommand command in playerCommands)
        {
            if (Moving)
            {
                yield return new WaitForSeconds(pauseBetween);

                command.Execute(this);
                AddToHistory(command);
            }
        }
        Moving = false;
    }
}
