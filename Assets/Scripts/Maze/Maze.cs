using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Maze
{
    private Dimensions _dimensions;
    private static readonly Random _generator = new Random();

    public static Maze Instance { get; private set; }
    public Vector2Int StartPos { get; private set; }
    public Vector2Int EndPos { get; private set; }
    public Dimensions Dimensions
    {
        get => _dimensions;
        set {
            _dimensions = value;
            StartPos = new Vector2Int(0, value.Height - 1);
            EndPos = new Vector2Int(value.Width - 1, 0);
        } 
    }
    public Dictionary<Vector2Int, MazeCell> Grid { get; private set; } = new Dictionary<Vector2Int, MazeCell>();
    public MazeCell this[Vector2Int pos] => Grid[pos];

    /// <summary>
    /// Initializes the singleton if the object didn't exist before
    /// </summary>
    public static void Initialize()
    {
        Instance = Instance ?? new Maze();
    }

    /// <summary>
    /// Checks if the position is in bounds of the maze
    /// </summary>
    /// <param name="pos">The position to check</param>
    /// <returns></returns>
    public bool InBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < Dimensions.Width &&
               pos.y >= 0 && pos.y < Dimensions.Height;
    }

    /// <summary>
    /// Synchronize the Maze with this state
    /// </summary>
    public void Load(MazeState state)
    {
        Clear();
        Dimensions = state.dimensions;
        foreach (SerCell cell in state.cells)
        {
            Grid[cell.Pos] = cell.ToMazeCell();
        }
    }

    /// <summary>
    /// Fills all the walls of the maze with a specific state
    /// </summary>
    /// <param name="wallState">The state of the walls to fill with</param>
    public void Fill(WallState wallState=WallState.Exists)
    {
        for (int x = 0; x < Dimensions.Width; x++)
        {
            for (int y = 0; y < Dimensions.Height; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                Grid[pos] = new MazeCell(pos, wallState, wallState, wallState, wallState);
            }
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

    public List<Vector2Int> GetRandomPositions(int quantity)
    {
        List<MazeCell> emptyCells = Grid.Values.Where(cell => cell.IsEmpty && cell.Position != StartPos).ToList();
        emptyCells.Shuffle();
        IEnumerable<Vector2Int> randomPositions = emptyCells.Select(cell => cell.Position);
        return randomPositions.Take(quantity).ToList();
    }

    public void PlaceOnMaze(List<ItemType> itemTypes)
    {
        List<Vector2Int> itemPositions = GetRandomPositions(itemTypes.Count);
        for (int i = 0; i < Mathf.Min(itemPositions.Count, itemTypes.Count); i++)
        {
            Grid[itemPositions[i]].ItemType = itemTypes[i];
        }
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

    public void Clear()
    {
        foreach (var kvPair in Grid)
        {
            kvPair.Value.Dispose();
        }
        Grid.Clear();
    }
}
