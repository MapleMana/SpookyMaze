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

enum WallState
{
    Destroyed,
    Exists
}

class MazeCell
{
    private Dictionary<Vector2Int, WallState> _wallState;

    public static List<Vector2Int> neighbours = new List<Vector2Int> { Vector2Int.up, 
                                                                       Vector2Int.left, 
                                                                       Vector2Int.down, 
                                                                       Vector2Int.right };

    public MazeCell(WallState up, WallState left, WallState down, WallState right)
    {
        _wallState = new Dictionary<Vector2Int, WallState>();
        _wallState[Vector2Int.up] = up;
        _wallState[Vector2Int.left] = left;
        _wallState[Vector2Int.down] = down;
        _wallState[Vector2Int.right] = right;
    }

    public void SetWall(Vector2Int direction, WallState state)
    {
        _wallState[direction] = state;
    }

    public bool WallExists(Vector2Int direction) {
        return _wallState[direction] == WallState.Exists;
    }
}

public class Maze : MonoBehaviour
{
    const float CELL_WIDTH = 10;
    const float WALL_WIDTH = 2.5f;

    public int width;
    public int height;
    public GameObject wallTemplate;

    private Dictionary<Vector2Int, MazeCell> _grid;
    private static Random _generator = new Random();

    private void OnEnable()
    {
        width = 10;
        height = 10;
        _grid = new Dictionary<Vector2Int, MazeCell>();
    }

    void Start()
    {
        
    }

    private void Fill(WallState wallState=WallState.Exists)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _grid[new Vector2Int(x, y)] = new MazeCell(wallState, wallState, wallState, wallState);
            }
        }
    }

    private bool InBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    private void ChangeWall(Vector2Int position, Vector2Int direction, WallState wallState)
    {
        Vector2Int neighbour = position + direction;
        _grid[position].SetWall(direction, wallState);
        _grid[neighbour].SetWall(direction * -1, wallState);
    }

    

    public void Generate()
    {
        Fill();

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

    private void PutWall(Vector3 pos, bool horizontal=true)
    {
        GameObject wall = Instantiate(wallTemplate, pos, Quaternion.identity);
        float wallWidth = horizontal ? CELL_WIDTH + WALL_WIDTH : WALL_WIDTH;
        float wallHeight = horizontal ? WALL_WIDTH : CELL_WIDTH + WALL_WIDTH;
        wall.transform.localScale = new Vector3(wallWidth, CELL_WIDTH, wallHeight);
    }

    public void Display()
    {
        foreach (var kvPair in _grid)
        {
            Vector2Int pos = kvPair.Key;
            MazeCell cell = kvPair.Value;
            if (cell.WallExists(Vector2Int.right))
            {
                PutWall(new Vector3(CELL_WIDTH * (pos.x + 1), 0, CELL_WIDTH * (pos.y + 0.5f)), false);
            }
            if (cell.WallExists(Vector2Int.up))
            {
                PutWall(new Vector3(CELL_WIDTH * (pos.x + 0.5f), 0, CELL_WIDTH * (pos.y + 1)), true);
            }

            if (pos.y == 0)
            {
                PutWall(new Vector3(CELL_WIDTH * (pos.x + 0.5f), 0, CELL_WIDTH * pos.y), true);
            }
            if (pos.x == 0)
            {
                PutWall(new Vector3(CELL_WIDTH * pos.x, 0, CELL_WIDTH * (pos.y + 0.5f)), false);
            }
        }
    }
}
