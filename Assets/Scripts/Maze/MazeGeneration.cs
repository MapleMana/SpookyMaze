using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenerationStrategy
{
    public abstract void Generate();
    
    /// <summary>
    /// Changes the state of the specified wall
    /// </summary>
    /// <param name="position">The position of the MazeCell that is adjacent to the target wall</param>
    /// <param name="direction">Determines which wall to change relative to the specified MazeCell</param>
    /// <param name="wallState">The desired state of the wall</param>
    internal void ChangeWall(Vector2Int position, Vector2Int direction, WallState wallState)
    {
        Vector2Int neighbour = position + direction;
        Maze.Instance[position].SetWall(direction, wallState);
        Maze.Instance[neighbour].SetWall(direction * -1, wallState);
    }
}

public class DFSGeneration : GenerationStrategy
{
    Dictionary<Vector2Int, bool> visited;

    public DFSGeneration()
    {
        visited = new Dictionary<Vector2Int, bool>();
    }

    /// <summary>
    /// Processes a single sell at the specified position. Calls itself for random neighbors.
    /// </summary>
    /// <param name="curPos">The position of the cell to process</param>
    void DFS(Vector2Int curPos)
    {
        visited[curPos] = true;
        MazeCell.neighbours.Shuffle();
        List<Vector2Int> neighboursOrder = new List<Vector2Int>(MazeCell.neighbours);
        foreach (Vector2Int direction in neighboursOrder)
        {
            Vector2Int newPos = curPos + direction;
            if (Maze.Instance.InBounds(newPos) && !visited.ContainsKey(newPos))
            {
                ChangeWall(curPos, direction, WallState.Destroyed);
                DFS(newPos);
            }
        }
    }
    
    /// <summary>
    /// Generates the maze using the Depth First Search algorithm.
    /// </summary>
    public override void Generate()
    {
        Maze.Instance.Fill();
        visited.Clear();
        DFS(Maze.Instance.StartPos);
    }
}

public class BFSGeneration : GenerationStrategy
{
    /// <summary>
    /// Generates the maze using the Breadth First Search algorithm. 
    /// Randomness is added by randomly selecting if the cell will proceed to the next stage of the BFS
    /// or it will be processed further
    /// </summary>
    public override void Generate()
    {
        Maze.Instance.Fill();
        Stack<Vector2Int> stage = new Stack<Vector2Int>();
        Dictionary<Vector2Int, bool> visited = new Dictionary<Vector2Int, bool>();
        stage.Push(Maze.Instance.StartPos);
        visited[Maze.Instance.StartPos] = true;
        while (stage.Count != 0)
        {
            Stack<Vector2Int> nextStage = new Stack<Vector2Int>();
            while (stage.Count != 0)
            {
                Vector2Int curPos = stage.Pop();

                MazeCell.neighbours.Shuffle();
                foreach (Vector2Int direction in MazeCell.neighbours)
                {
                    Vector2Int newPos = curPos + direction;
                    if (Maze.Instance.InBounds(newPos) && !visited.ContainsKey(newPos))
                    {
                        visited[newPos] = true;
                        ChangeWall(curPos, direction, WallState.Destroyed);
                        Stack<Vector2Int> targetStage = (Random.value > 0.5) ? stage : nextStage;
                        targetStage.Push(newPos);
                    }
                }
            }
            stage = nextStage;
        }
    }
}

public class BranchedDFSGeneration : GenerationStrategy
{
    Stack<T> ReverseStack<T>(Stack<T> stack)
    {
        Stack<T> result = new Stack<T>();
        while (stack.Count != 0)
        {
            result.Push(stack.Pop());
        }
        return result;
    }

    /// <summary>
    /// Generates the maze using the Depth First Search algorithm.
    /// </summary>
    public override void Generate()
    {
        Maze.Instance.Fill();
        Stack<Vector2Int> path = new Stack<Vector2Int>();
        Dictionary<Vector2Int, bool> visited = new Dictionary<Vector2Int, bool>();
        path.Push(Maze.Instance.StartPos);
        while (path.Count != 0)
        {
            Vector2Int curPos = path.Peek();
            visited[curPos] = true;

            if (curPos == Maze.Instance.EndPos && path.Count > 1)
            {
                path = ReverseStack<Vector2Int>(path);
                continue;
            }

            MazeCell.neighbours.Shuffle();
            bool isDeadEnd = true;
            foreach (Vector2Int direction in MazeCell.neighbours)
            {
                Vector2Int newPos = curPos + direction;
                if (Maze.Instance.InBounds(newPos) && !visited.ContainsKey(newPos))
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