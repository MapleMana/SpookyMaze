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
    
    void Update()
    {
        if (_canBeMoved)
        {
            if (!_moving) {
                SyncRealPosition();
                
                StartCoroutine(PlayCommandsInRealTime(
                    playerCommands: new List<PlayerCommand> { PlayerMovementCommand.FromVector(MazeCell.neighbours[0]) },
                    pauseBetween: 1 / ghostSpeed
            ));
            }
        }
    }
    
    override public bool Move(Vector2Int direction)
    {
        _mazePosition += direction;
        return true;
    }

    private Vector2Int getRandomDirection()
    {
        MazeCell.neighbours.Shuffle();
        foreach (Vector2Int possibleDirection in MazeCell.neighbours)
        {
            if (Maze.Instance.InBounds(_mazePosition + possibleDirection))
            {
                return possibleDirection;
            }
        }
        
        return MazeCell.neighbours[0];
    }
}
