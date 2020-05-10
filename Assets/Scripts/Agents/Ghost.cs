using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Movable
{
    private static bool _canBeMoved = false;
    private PlayerCommand command;
    private List<PlayerCommand> commandSequence = new List<PlayerCommand>();

    public float ghostSpeed;
    public static bool CanBeMoved { get => _canBeMoved; set => _canBeMoved = value; }

    private void Start()
    {
        
    }
    
    void Update()
    {
        if (_canBeMoved)
        {
            MazeCell.neighbours.Shuffle();
            if (Move(MazeCell.neighbours[0])) {
                //AddToHistory(command);
                SyncRealPosition();
                StartCoroutine(PlayCommandsInRealTime(
                    playerCommands: commandSequence,
                    pauseBetween: 1 / ghostSpeed
            ));
            }
        }
    }
    
    override public bool Move(Vector2Int direction)
    {
        if (Maze.Instance.InBounds(_mazePosition + direction))
        {
            _mazePosition += direction;
            return true;
        }
        return false;
    }
}
