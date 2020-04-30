using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
class MazeState
{
    // TODO: make serializable
    private Dictionary<Vector2Int, MazeCell> _grid = new Dictionary<Vector2Int, MazeCell>();

    public Dictionary<Vector2Int, MazeCell> Grid { get => _grid; set => _grid = value; }

    public MazeState()
    {
        _grid = new Dictionary<Vector2Int, MazeCell>();
    }

    /// <summary>
    /// Saves the state to a local file
    /// </summary>
    /// <param name="filePath">The file path to save to</param>
    public void SaveTo(string filePath)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + filePath;
        using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate))
        {
            formatter.Serialize(stream, this);
        }
    }

    /// <summary>
    /// Reads the state from a local file
    /// </summary>
    /// <returns></returns>
    public MazeState LoadFrom(string filePath)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + filePath;
        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            return formatter.Deserialize(stream) as MazeState;
        }
    }
}

public class Maze : MonoBehaviour
{
    private static Maze _instance;

    private int _width = 10;
    private int _height = 10;
    private Vector2Int _start;
    private Vector2Int _end;
    private MazeState _state;
    private string  _beforeStart;
    private static Random _generator = new Random();

    public GameObject wallTemplate;
    public GameObject keyTemplate;

    public Vector2Int StartPos => _start;
    public Vector2Int EndPos => _end;

    public int Width => _width;
    public int Height => _height;
    public static Maze Instance => _instance;
    public MazeCell this[Vector2Int pos] => _state.Grid[pos];

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

        _start = new Vector2Int(0, height - 1);
        _end = new Vector2Int(width - 1, 0);


        // clear all walls from previous generation
        if (_state != null) {
            foreach (var kvPair in _state.Grid)
            {
                kvPair.Value.Dispose();
            }
        }

        _state = new MazeState();
    }

    public void Restore()
    {
        _state = JsonUtility.FromJson<MazeState>(_beforeStart);
        // TODO: redraw
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
                _state.Grid[pos] = new MazeCell(pos, wallState, wallState, wallState, wallState);
            }
        }
    }

    /// <summary>
    /// Generates a new maze and places the specified items randomly
    /// </summary>
    /// <param name="algo">The algorithm to use for maze generation</param>
    /// <param name="items">The items that should appear on the maze</param>
    public void Generate(GenerationStrategy algo, List<ItemType>items)
    {
        algo.Generate();
        List<MazeCell> cells = _state.Grid.Values.ToList();
        List<int> randInd = new List<int>();
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].Position != _start && cells[i].Position != _end)
            {
                randInd.Add(i);
            }
        }
        randInd.Shuffle();
        int cellInd = 0;
        foreach (ItemType itemType in items)
        {
            while (cellInd < randInd.Count && !cells[randInd[cellInd]].IsEmpty)
            {
                cellInd++;
            }
            if (cellInd < randInd.Count)
            {
                MazeCell cell = cells[randInd[cellInd]];
                cell.Item = new Item(itemType);
            }
        }
        _beforeStart = JsonUtility.ToJson(_state);
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
        incomingDirection = _state.Grid[position].GetCorridorOpening(incomingDirection * -1);
        while (incomingDirection != Vector2Int.zero)
        {
            sequence.Add(incomingDirection);
            position += incomingDirection;
            incomingDirection = _state.Grid[position].GetCorridorOpening(incomingDirection * -1);
        }
        return sequence;
    }

    /// <summary>
    /// Display each MazeCell of this maze
    /// </summary>
    public void Display()
    {
        foreach (var kvPair in _state.Grid)
        {
            kvPair.Value.Display();            
        }
    }
}
