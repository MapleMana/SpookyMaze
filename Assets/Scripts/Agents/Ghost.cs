using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Movable
{
    private static bool _canBeMoved = false;

    public float Speed;
    public static bool CanBeMoved { get => _canBeMoved; set => _canBeMoved = value; }

    private void Start()
    {
        
    }
    
    void Update()
    {
        if (_canBeMoved)
        {
            MazeCell.neighbours.Shuffle();
            Move(MazeCell.neighbours[0]);
            
        }
    }
    
    override public bool Move(Vector2Int direction)
    {
        if (Maze.Instance.InBounds(_mazePosition + direction))
        {
            _mazePosition += direction;
            SyncRealPosition();
            return true;
        }
        return false;
    }
}
