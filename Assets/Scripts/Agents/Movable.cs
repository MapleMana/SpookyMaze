using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movable : MonoBehaviour
{
    internal Vector2Int _mazePosition;
    internal List<ObjectCommand> _commandHistory;
    internal bool _moving = false;

    public bool Moving { get => _moving; set => _moving = value; }
    public bool AtMazeEnd => _mazePosition == Maze.Instance.EndPos;

    public abstract bool Move(Vector2Int direction);

    /// <summary>
    /// Synchronizes maze position and physical player position
    /// </summary>
    internal void SyncRealPosition()
    {
        MazeCell currentCell = Maze.Instance[_mazePosition];
        transform.position = currentCell.CellCenter(y: transform.position.y);
    }

    /// <summary>
    /// Performs a sequence of commands on the object
    /// </summary>
    /// <param name="commands">The sequence of commands to execute</param>
    /// <param name="reversed">Whether the execution should be reversed (both order and individual commands)</param>
    /// <param name="initialPosition">The starting position of the player. If null, current position is taken.</param>
    /// <param name="playTime">The time the coroutine will take. If null, replay time is taken. This parameter is overriden by pauseBetween</param>
    /// <param name="pauseBetween">Pause between each command. If null, play time is considered.</param>
    /// <param name="onComplete">Action to perform after the replay is comlete</param>
    /// <param name="saveToHistory">Whether the command sequence should be added to player's history</param>
    /// <returns>A coroutine to execute</returns>
    public IEnumerator PlayCommands(
        List<ObjectCommand> commands = null,
        bool reversed = false,
        Vector2Int? initialPosition = null,
        float? playTime = null,
        float? pauseBetween = null,
        System.Action onComplete = null,
        bool saveToHistory = false)
    {
        commands = commands ?? _commandHistory;
        if (reversed)
        {
            commands.Reverse();
        }
        _mazePosition = initialPosition ?? _mazePosition;
        float pauseBetweenCommands = pauseBetween ?? ((playTime ?? 0) / commands.Count);

        SyncRealPosition();
        Moving = true;

        foreach (ObjectCommand command in commands)
        {
            if (Moving)
            {
                yield return new WaitForSeconds(pauseBetweenCommands);
                if (reversed)
                {
                    command.ExecuteReversed(this);
                }
                else
                {
                    command.Execute(this);
                }

                if (saveToHistory)
                {
                    _commandHistory.Add(command);
                }
            }
        }
        Moving = false;
        onComplete?.Invoke();
    }

}
