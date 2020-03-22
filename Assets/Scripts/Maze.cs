using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtension
{
    public static void Shuffle<T>(this List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(0, list.Count);
            int k = Random.Range(0, list.Count);
            T value = list[k];
            list[k] = list[j];
            list[j] = value;
        }
    }
}

public enum WallState
{
    Destroyed,
    Exists
}

public class MazeCell
{
    private Dictionary<Vector2Int, WallState> _wallState = new Dictionary<Vector2Int, WallState>();
    private Vector2Int _position;

    public const float CELL_WIDTH = 10;

    public static List<Vector2Int> neighbours = new List<Vector2Int> { Vector2Int.up, 
                                                                       Vector2Int.left, 
                                                                       Vector2Int.down, 
                                                                       Vector2Int.right };

    public MazeCell(Vector2Int pos, WallState up, WallState left, WallState down, WallState right)
    {
        _position = pos;
        _wallState[Vector2Int.up] = up;
        _wallState[Vector2Int.left] = left;
        _wallState[Vector2Int.down] = down;
        _wallState[Vector2Int.right] = right;
    }

    /// <summary>
    /// Changes the state of a specific wall adjacent to the cell
    /// </summary>
    /// <param name="direction">The direction where the wall is located relative to the center of the cell</param>
    /// <param name="state">The desired wall state</param>
    public void SetWall(Vector2Int direction, WallState state)
    {
        _wallState[direction] = state;
    }

    /// <summary>
    /// Checks if the wall exists at the specified direction
    /// </summary>
    /// <param name="direction">The direction where the wall is located relative to the center of the cell</param>
    /// <returns></returns>
    public bool WallExists(Vector2Int direction) {
        return _wallState[direction] == WallState.Exists;
    }

    public Vector2 CellCenter()
    {
        return new Vector2(CELL_WIDTH * (_position.x + 0.5f), CELL_WIDTH * (_position.y + 0.5f));
    }
}

public class Maze : MonoBehaviour
{
    private static Maze _instance;

    private int _width = 10;
    private int _height = 10;
    private Dictionary<Vector2Int, MazeCell> _grid = new Dictionary<Vector2Int, MazeCell>();
    private static Random _generator = new Random();

    public const float WALL_WIDTH = 2.5f;

    public GameObject wallTemplate;

    public MazeCell start;
    public MazeCell finish;

    public int Width { get => _width; }
    public int Height { get => _height; }
    public static Maze Instance { get => _instance; set => _instance = value; }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Specifies the initial maze state
    /// </summary>
    /// <param name="width">The width (X) of the maze</param>
    /// <param name="height">The height (Z) of the maze</param>
    public void Initialize(int width, int height)
    {
        _width = width;
        _height = height;

        Fill();

        start = _grid[new Vector2Int(0, height - 1)];
        finish = _grid[new Vector2Int(width - 1, 0)];
    }

    /// <summary>
    /// Fills all the walls of the maze with a specific state
    /// </summary>
    /// <param name="wallState">The state of the walls to fill with</param>
    private void Fill(WallState wallState=WallState.Exists)
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                _grid[pos] = new MazeCell(pos, wallState, wallState, wallState, wallState);
            }
        }
    }

    /// <summary>
    /// Checks if the position is in bounds of the maze
    /// </summary>
    /// <param name="pos">The position to check</param>
    /// <returns></returns>
    private bool InBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < _width && pos.y >= 0 && pos.y < _height;
    }

    /// <summary>
    /// Changes the state of the specified wall
    /// </summary>
    /// <param name="position">The position of the MazeCell that is adjacent to the target wall</param>
    /// <param name="direction">Determines which wall to change relative to the specified MazeCell</param>
    /// <param name="wallState">The desired state of the wall</param>
    private void ChangeWall(Vector2Int position, Vector2Int direction, WallState wallState)
    {
        Vector2Int neighbour = position + direction;
        _grid[position].SetWall(direction, wallState);
        _grid[neighbour].SetWall(direction * -1, wallState);
    }

    
    /// <summary>
    /// Generates the maze using the DFS algorithm. All the walls should exist at the start of execution.
    /// </summary>
    public void Generate()
    {
        Stack<Vector2Int> path = new Stack<Vector2Int>();
        Dictionary<Vector2Int, bool> visited = new Dictionary<Vector2Int, bool>();
        foreach (var kvPair in _grid)
        {
            visited[kvPair.Key] = false;
        }
        Vector2Int start = new Vector2Int(0, 0);
        path.Push(start);
        while (path.Count != 0)
        {
            Vector2Int curPos = path.Pop();
            MazeCell curCell = _grid[curPos];

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

    /// <summary>
    /// Creates a wall GameObject at the specified position
    /// </summary>
    /// <param name="pos">The position to place the wall at</param>
    /// <param name="horizontal">Determines whether the wall is horizontal or vertical</param>
    private void PutWall(Vector3 pos, bool horizontal=true)
    {
        GameObject wall = Instantiate(wallTemplate, pos, Quaternion.identity);
        float wallWidth = horizontal ? MazeCell.CELL_WIDTH + WALL_WIDTH : WALL_WIDTH;
        float wallHeight = horizontal ? WALL_WIDTH : MazeCell.CELL_WIDTH + WALL_WIDTH;
        wall.transform.localScale = new Vector3(wallWidth, MazeCell.CELL_WIDTH, wallHeight);
    }

    /// <summary>
    /// For each MazeCell in _grid creates an actual Wall in 3D space
    /// </summary>
    public void Display()
    {
        foreach (var kvPair in _grid)
        {
            Vector2Int pos = kvPair.Key;
            MazeCell cell = kvPair.Value;
            if (cell.WallExists(Vector2Int.right))
            {
                PutWall(new Vector3(MazeCell.CELL_WIDTH * (pos.x + 1), 0, MazeCell.CELL_WIDTH * (pos.y + 0.5f)), false);
            }
            if (cell.WallExists(Vector2Int.up))
            {
                PutWall(new Vector3(MazeCell.CELL_WIDTH * (pos.x + 0.5f), 0, MazeCell.CELL_WIDTH * (pos.y + 1)), true);
            }

            if (pos.y == 0)
            {
                PutWall(new Vector3(MazeCell.CELL_WIDTH * (pos.x + 0.5f), 0, MazeCell.CELL_WIDTH * pos.y), true);
            }
            if (pos.x == 0)
            {
                PutWall(new Vector3(MazeCell.CELL_WIDTH * pos.x, 0, MazeCell.CELL_WIDTH * (pos.y + 0.5f)), false);
            }
        }
    }
}
