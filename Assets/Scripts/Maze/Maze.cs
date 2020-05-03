using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Maze: System.IDisposable
{
    private static Maze _instance;

    private int _width = 10;
    private int _height = 10;
    private Vector2Int _start;
    private Vector2Int _end;
    private string  _beforeStart;
    private static Random _generator = new Random();
    private Dictionary<Vector2Int, MazeCell> _grid = new Dictionary<Vector2Int, MazeCell>();


    public static Maze Instance => _instance;
    public Vector2Int StartPos => _start;
    public Vector2Int EndPos => _end;
    public int Width => _width;
    public int Height => _height;
    public Dictionary<Vector2Int, MazeCell> Grid { get => _grid; set => _grid = value; }
    public MazeCell this[Vector2Int pos] => Grid[pos];

    private Maze() { }

    /// <summary>
    /// Initializes the singleton if the object didn't exist before
    /// </summary>
    public static void Initialize()
    {
        _instance = _instance ?? new Maze();
    }

    /// <summary>
    /// Specifies the initial maze dimensions
    /// </summary>
    /// <param name="width">The width (X) of the maze</param>
    /// <param name="height">The height (Z) of the maze</param>
    public void SetDimensions(int width, int height)
    {
        Dispose();

        _width = width;
        _height = height;

        _start = new Vector2Int(0, height - 1);
        _end = new Vector2Int(width - 1, 0);
    }

    /// <summary>
    /// Restores maze state before level start
    /// </summary>
    public void Restore()
    {
        MazeState state = JsonUtility.FromJson<MazeState>(_beforeStart);
        state.Load();
    }

    /// <summary>
    /// Saves the state of the maze before the level starts
    /// </summary>
    public void SaveState()
    {
        MazeState state = new MazeState(this);
        _beforeStart = JsonUtility.ToJson(state);
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
                Grid[pos] = new MazeCell(pos, wallState, wallState, wallState, wallState);
            }
        }
    }

    /// <summary>
    /// Places the specified items randomly on this maze
    /// </summary>
    /// <param name="items">The items that should appear on the maze</param>
    public void GenerateItems(List<Item>items)
    {
        List<MazeCell> cells = Grid.Values.ToList();
        List<int> randInd = new List<int>();
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].Position != _start && 
                cells[i].Position != _end)
            {
                randInd.Add(i);
            }
        }
        randInd.Shuffle();
        for (int i = 0; i < Mathf.Min(randInd.Count, items.Count); i++)
        {
            MazeCell cell = cells[randInd[i]];
            cell.Item = items[i];
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
        incomingDirection = Grid[position].GetCorridorOpening(incomingDirection * -1);
        while (incomingDirection != Vector2Int.zero)
        {
            sequence.Add(incomingDirection);
            position += incomingDirection;
            incomingDirection = Grid[position].GetCorridorOpening(incomingDirection * -1);
        }
        return sequence;
    }

    /// <summary>
    /// Display each MazeCell of this maze
    /// </summary>
    public void Display()
    {
        foreach (var kvPair in Grid)
        {
            kvPair.Value.Display();            
        }
    }

    public void Dispose()
    {
        foreach (var kvPair in Grid)
        {
            kvPair.Value.Dispose();
        }
        Grid.Clear();
    }
}
