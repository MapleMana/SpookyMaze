using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movable : MonoBehaviour
{
    internal Vector2Int _mazePosition;
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
}
