using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Maze: Singleton<Maze>
{
    private Vector2Int _start;
    private Vector2Int _end;
    private string  _beforeStart;
    private static readonly Random _generator = new Random();

    public Vector2Int StartPos => _start;
    public Vector2Int EndPos => _end;
    public int Width { get; private set; } = 10;
    public int Height { get; private set; } = 10;
    public Dictionary<Vector2Int, MazeCell> Grid { get; private set; } = new Dictionary<Vector2Int, MazeCell>();
    public MazeCell this[Vector2Int pos] => Grid[pos];

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// Checks if the position is in bounds of the maze
    /// </summary>
    /// <param name="pos">The position to check</param>
    /// <returns></returns>
    public bool InBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < Width &&
               pos.y >= 0 && pos.y < Height;
    }

    /// <summary>
    /// Specifies the initial maze dimensions
    /// </summary>
    /// <param name="width">The width (X) of the maze</param>
    /// <param name="height">The height (Z) of the maze</param>
    public void SetDimensions(int width, int height)
    {
        Width = width;
        Height = height;

        _start = new Vector2Int(0, height - 1);
        _end = new Vector2Int(width - 1, 0);
    }

    // FIXME: move back to MazeIO when Maze is no longer a singleton
    /// <summary>
    /// Synchronize the Maze with this state
    /// </summary>
    public void Load(MazeState state)
    {
        SetDimensions(state.width, state.height);
        foreach (SerCell cell in state.cells)
        {
            Grid[cell.Pos] = cell.ToMazeCell();
        }
    }
    /// <summary>
    /// Restores maze state before level start
    /// </summary>
    public void Restore()
    {
        MazeState state = JsonUtility.FromJson<MazeState>(_beforeStart);
        Load(state);
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
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
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
            kvPair.Value.Display(gameObject);            
        }
    }

    protected override void OnDestroy()
    {
        Clear();
        base.OnDestroy();
    }

    public void Clear()
    {
        foreach (var kvPair in Grid)
        {
            kvPair.Value.Dispose();
        }
    }
}
