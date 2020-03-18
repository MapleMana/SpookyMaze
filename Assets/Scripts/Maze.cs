using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MazeCell
{
    private Vector2Int _pos;
    private Dictionary<Vector2Int, bool> _canGoTo;

    public static List<Vector2Int> neighbours = new List<Vector2Int> { Vector2Int.up, 
                                                                       Vector2Int.left, 
                                                                       Vector2Int.down, 
                                                                       Vector2Int.right };

    public MazeCell(int r, int c, bool top, bool left, bool bottom, bool right)
    {
        _pos = new Vector2Int(r, c);
        _canGoTo = new Dictionary<Vector2Int, bool>();
        _canGoTo[Vector2Int.up] = top;
        _canGoTo[Vector2Int.left] = left;
        _canGoTo[Vector2Int.down] = bottom;
        _canGoTo[Vector2Int.right] = right;
    }

    public Vector2Int Pos { get => _pos; set => _pos = value; }
    internal Dictionary<Vector2Int, bool> CanGoTo { get => _canGoTo; set => _canGoTo = value; }
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

    private void Fill(bool isEmpty=false)
    {
        for (int r = 0; r < height; r++)
        {
            for (int c = 0; c < width; c++)
            {
                _grid[new Vector2Int(r, c)] = new MazeCell(r, c, isEmpty, isEmpty, isEmpty, isEmpty);
            }
        }
    }

    public void Generate()
    {
        Fill();


    }

    private void PutWall(Vector3 pos, bool horizontal=true)
    {
        GameObject wall = Instantiate(wallTemplate, pos, Quaternion.identity);
        if (horizontal)
        {
            wall.transform.localScale = new Vector3(CELL_WIDTH + WALL_WIDTH, CELL_WIDTH, WALL_WIDTH);
        }
        else
        {
            wall.transform.localScale = new Vector3(WALL_WIDTH, CELL_WIDTH, CELL_WIDTH + WALL_WIDTH);
        }
    }

    public void Display()
    {
        foreach (var kvPair in _grid)
        {
            MazeCell cell = kvPair.Value;
            Vector2Int pos = kvPair.Key;
            if (!cell.CanGoTo[Vector2Int.left])
            {
                PutWall(new Vector3(pos.x, 0, -(pos.y + CELL_WIDTH / 2)), false);
            }
            if (!cell.CanGoTo[Vector2Int.down])
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
