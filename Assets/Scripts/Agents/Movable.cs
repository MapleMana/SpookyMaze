using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movable : MonoBehaviour
{
    private Vector2Int _mazePosition;
    internal static List<KeyValuePair<Movable, MovableCommand>> _commandHistory;
    internal static float _previousCommandTime;

    public bool Moving { get; set; } = false;
    public bool AtMazeEnd => MazePosition == Maze.Instance.EndPos;

    internal Vector2Int MazePosition {
        get => _mazePosition;
        set
        {
            _mazePosition = value;
            MazeCell currentCell = Maze.Instance[value];
            transform.position = currentCell.CellCenter(y: transform.position.y);
        }
    }

    public abstract bool Move(Vector2Int direction);

    void Awake()
    {
        _commandHistory = new List<KeyValuePair<Movable, MovableCommand>>();
    }

    /// <summary>
    /// Places movable at the start of the maze and inits movable's state
    /// </summary>
    public static void ResetState()
    {
        _commandHistory.Clear();
        _previousCommandTime = Time.time;
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
    /// <param name="pauseBetween"></param>
    /// <returns></returns>
    internal IEnumerator PlayCommandsInRealTime(
        List<MovableCommand> playerCommands,
        float pauseBetween)
    {
        //pauseBetweenCommands -= Time.deltaTime;

        Moving = true;
        foreach (MovableCommand command in playerCommands)
        {
            if (Moving)
            {
                yield return new WaitForSeconds(pauseBetween);

                command.Execute(this);
                AddToHistory(this, command);
            }
        }
        Moving = false;
    }
}
