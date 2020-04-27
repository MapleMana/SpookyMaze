using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Maze : MonoBehaviour
{
    private static Maze _instance;

    private int _width = 10;
    private int _height = 10;
    private Vector2Int _start;
    private Vector2Int _end;
    private Dictionary<Vector2Int, MazeCell> _grid = new Dictionary<Vector2Int, MazeCell>();
    private static Random _generator = new Random();
    private GenerationStrategy _genAlgo;

    public GameObject wallTemplate;
    public GameObject keyTemplate;

    public Vector2Int StartPos => _start;
    public Vector2Int EndPos => _end;

    public int Width => _width;
    public int Height => _height;
    public static Maze Instance => _instance;
    public MazeCell this[Vector2Int pos] => _grid[pos];

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
    /// <param name="algorithm">The algorithm to use for maze generation</param>
    public void Initialize(int width, int height, GenerationStrategy algorithm)
    {
        _width = width;
        _height = height;
        _genAlgo = algorithm;

        _start = new Vector2Int(0, height - 1);
        _end = new Vector2Int(width - 1, 0);

        // clear all walls from previous generation
        foreach (var kvPair in _grid)
        {
            kvPair.Value.Dispose();
        }
    }

    /// <summary>
    /// Fills all the walls of the maze with a specific state
    /// </summary>
    /// <param name="wallState">The state of the walls to fill with</param>
    public void Fill(WallState wallState=WallState.Exists)
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

    public void Generate(List<ItemType>items)
    {
        _genAlgo.Generate();
        List<MazeCell> cells = _grid.Values.ToList();
        List<int> ind = new List<int>();
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].Position != _start && cells[i].Position != _end)
            {
                ind.Add(i);
            }
        }
        ind.Shuffle();
        for (int i = 0; i < items.Count; i++)
        {
            MazeCell cell = cells[ind[i]];
            cell.Item = new Item(items[i]);
        }
    }

    /// <summary>
    /// Returns a sequence of directions until the next decision point (eintersenction or dead end)
    /// </summary>
    /// <param name="position">The current position to start from</param>
    /// <param name="incomingDirection">The direction where current position was entered from</param>
    /// <returns></returns>
    public List<Vector2Int> GetSequenceToDicisionPoint(Vector2Int position, Vector2Int incomingDirection)
    {
        List<Vector2Int> sequence = new List<Vector2Int>();
        incomingDirection = _grid[position].GetCorridorOpening(incomingDirection * -1);
        while (incomingDirection != Vector2Int.zero)
        {
            sequence.Add(incomingDirection);
            position += incomingDirection;
            incomingDirection = _grid[position].GetCorridorOpening(incomingDirection * -1);
        }
        return sequence;
    }

    /// <summary>
    /// Display each MazeCell in _grid 
    /// </summary>
    public void Display()
    {
        foreach (var kvPair in _grid)
        {
            kvPair.Value.Display();            
        }
    }
}
