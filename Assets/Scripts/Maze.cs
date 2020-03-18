using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum WallState
{
    Destroyed,
    Exists
}

class MazeCell
{
    private Vector2Int _pos;
    private Dictionary<Vector2Int, WallState> _wallState;

    public static List<Vector2Int> neighbours = new List<Vector2Int> { Vector2Int.up, 
                                                                       Vector2Int.left, 
                                                                       Vector2Int.down, 
                                                                       Vector2Int.right };

    public MazeCell(int r, int c, WallState top, WallState left, WallState bottom, WallState right)
    {
        _pos = new Vector2Int(r, c);
        _wallState = new Dictionary<Vector2Int, WallState>();
        _wallState[Vector2Int.up] = top;
        _wallState[Vector2Int.left] = left;
        _wallState[Vector2Int.down] = bottom;
        _wallState[Vector2Int.right] = right;
    }

    public Vector2Int Pos { get => _pos; set => _pos = value; }

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
    const float CELL_WIDTH = 1;
    const float WALL_WIDTH = 0.25f;

    public int width;
    public int height;
    public GameObject wallTemplate;

    private Dictionary<Vector2Int, MazeCell> _grid;

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
                _grid[new Vector2Int(x, y)] = new MazeCell(y, x, wallState, wallState, wallState, wallState);
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
        for (int r = 0; r < height; r++)
        {
            for (int c = 0; c < width; c++)
            {
                visited[new Vector2Int(r, c)] = false;
            }
        }
        Vector2Int start = new Vector2Int(0, 0);
        path.Push(start);
        while (path.Count != 0)
        {
            Vector2Int curPos = path.Pop();
            MazeCell curCell = _grid[curPos];

            List<Vector2Int> ways = MazeCell.neighbours;
            ways.Sort((a, b) => Random.value.CompareTo(Random.value));

            foreach (Vector2Int direction in ways)
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
            MazeCell cell = kvPair.Value;
            Vector2Int pos = kvPair.Key;
            if (cell.WallExists(Vector2Int.left))
            {
                PutWall(new Vector3(pos.x, 0, -(pos.y + CELL_WIDTH / 2)), false);
            }
            if (cell.WallExists(Vector2Int.down))
            {
                PutWall(new Vector3(pos.x + CELL_WIDTH / 2, 0, -(pos.y + CELL_WIDTH)), true);
            }

            if (pos.y == 0)
            {
                PutWall(new Vector3(pos.x + CELL_WIDTH / 2, 0, -pos.y), true);
            }
            if (pos.x == width-1)
            {
                PutWall(new Vector3(pos.x + CELL_WIDTH, 0, -(pos.y + CELL_WIDTH / 2)), false);
            }
        }
    }
}
