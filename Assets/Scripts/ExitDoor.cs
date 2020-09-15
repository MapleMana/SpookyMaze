using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    public void MoveToExit(Maze maze)
    {
        MazeCell currentCell = Maze.Instance[maze.EndPos];
        transform.position = currentCell.CellCenter(y: transform.position.y);
    }
}
