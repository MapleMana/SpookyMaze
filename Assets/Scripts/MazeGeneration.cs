using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenerationStrategy
{
    public abstract void Generate();

    /// <summary>
    /// Checks if the position is in bounds of the maze
    /// </summary>
    /// <param name="pos">The position to check</param>
    /// <returns></returns>
    internal bool InBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < Maze.Instance.Width && 
               pos.y >= 0 && pos.y < Maze.Instance.Height;
    }

    /// <summary>
    /// Changes the state of the specified wall
    /// </summary>
    /// <param name="position">The position of the MazeCell that is adjacent to the target wall</param>
    /// <param name="direction">Determines which wall to change relative to the specified MazeCell</param>
    /// <param name="wallState">The desired state of the wall</param>
    internal void ChangeWall(Vector2Int position, Vector2Int direction, WallState wallState)
    {
        Vector2Int neighbour = position + direction;
        Maze.Instance.Grid[position].SetWall(direction, wallState);
        Maze.Instance.Grid[neighbour].SetWall(direction * -1, wallState);
    }
}

public class DFSGeneration : GenerationStrategy
{
    /// <summary>
    /// Generates the maze using the DFS algorithm.
    /// </summary>
    public override void Generate()
    {
        Maze.Instance.Fill();
        Stack<Vector2Int> path = new Stack<Vector2Int>();
        Dictionary<Vector2Int, bool> visited = new Dictionary<Vector2Int, bool>();
        foreach (var kvPair in Maze.Instance.Grid)
        {
            visited[kvPair.Key] = false;
        }
        path.Push(Maze.Instance.Start);
        while (path.Count != 0)
        {
            Vector2Int curPos = path.Peek();
            MazeCell curCell = Maze.Instance.Grid[curPos];
            visited[curPos] = true;

            MazeCell.neighbours.Shuffle();
            bool isDeadEnd = true;
            foreach (Vector2Int direction in MazeCell.neighbours)
            {
                Vector2Int newPos = curPos + direction;
                if (InBounds(newPos) && !visited[newPos])
                {
                    path.Push(newPos);
                    ChangeWall(curPos, direction, WallState.Destroyed);
                    isDeadEnd = false;
                    break; // process the newly added cell
                }
            }
            if (isDeadEnd)
            {
                path.Pop();
            }
        }
    }
}

public class BFSGeneration : GenerationStrategy
{
    /// <summary>
    /// Generates the maze using the BFS algorithm.
    /// </summary>
    public override void Generate()
    {
        Maze.Instance.Fill();
        Stack<Vector2Int> path = new Stack<Vector2Int>();
        Dictionary<Vector2Int, bool> visited = new Dictionary<Vector2Int, bool>();
        foreach (var kvPair in Maze.Instance.Grid)
        {
            visited[kvPair.Key] = false;
        }
        path.Push(Maze.Instance.Start);
        visited[Maze.Instance.Start] = true;
        while (path.Count != 0)
        {
            Vector2Int curPos = path.Pop();
            MazeCell curCell = Maze.Instance.Grid[curPos];

            MazeCell.neighbours.Shuffle();

            foreach (Vector2Int direction in MazeCell.neighbours)
            {
                Vector2Int newPos = curPos + direction;
                if (InBounds(newPos) && !visited[newPos])
                {
                    visited[newPos] = true;
                    path.Push(newPos);
                    ChangeWall(curPos, direction, WallState.Destroyed);
                }
            }
        }
    }
}