using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Direction
{
    public readonly int _dtR;
    public readonly int _dtC;

    public static Direction top = new Direction(0, -1);
    public static Direction left = new Direction(-1, 0);
    public static Direction bottom = new Direction(0, 1);
    public static Direction right = new Direction(1, 0);

    public Direction(int deltaR, int deltaC)
    {
        _dtR = deltaR;
        _dtC = deltaC;
    }
}

class MazeCell
{
    private Vector2Int _pos;
    private Dictionary<Direction, bool> _canGoTo;

    public static List<Direction> neighbours = new List<Direction> { Direction.top, 
                                                                     Direction.left, 
                                                                     Direction.bottom, 
                                                                     Direction.right };

    public MazeCell(int r, int c, bool top, bool left, bool bottom, bool right)
    {
        _pos = new Vector2Int(r, c);
        _canGoTo[Direction.top] = top;
        _canGoTo[Direction.left] = left;
        _canGoTo[Direction.bottom] = bottom;
        _canGoTo[Direction.right] = right;
    }

    public Vector2Int Pos { get => _pos; set => _pos = value; }
}

public class Maze : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject wallTemplate;

    private Dictionary<MazeCell, Vector2> _grid;

    private void OnEnable()
    {
        
    }

    void Start()
    {
        
    }

    public void Generate()
    {
        GameObject wall = Instantiate(wallTemplate, new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
        wall.transform.localScale = new Vector3(1.25f, 1, 0.25f);
    }
}
