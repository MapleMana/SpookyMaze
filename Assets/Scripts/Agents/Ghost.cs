using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Movable
{
    private static bool _canMove = false;
    private PlayerCommand command;
    private List<PlayerCommand> commandSequence = new List<PlayerCommand>();

    public float ghostSpeed;
    public static bool CanBeMoved { get => _canMove; set => _canMove = value; }
    
    void Update()
    {
        if (_canMove)
        {
            if (!_moving) {
                StartCoroutine(PlayCommandsInRealTime(
                    playerCommands: new List<PlayerCommand> { PlayerMovementCommand.FromVector(getRandomDirection()) },
                    pauseBetween: 1 / ghostSpeed
                    ));
            }
        }
    }
    
    public override bool Move(Vector2Int direction)
    {
        MazePosition += direction;
        return true;
    }

    private Vector2Int getRandomDirection()
    {
        MazeCell.neighbours.Shuffle();
        foreach (Vector2Int possibleDirection in MazeCell.neighbours)
        {
            if (Maze.Instance.InBounds(MazePosition + possibleDirection))
            {
                return possibleDirection;
            }
        }
        
        return MazeCell.neighbours[0];
    }
}
