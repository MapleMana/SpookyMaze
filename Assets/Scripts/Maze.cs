using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

public enum ItemType
{
    None,
    Key
}

public class Item
{
    private ItemType _type;
    private static GameObject _keyTemplate = Resources.Load<GameObject>("Key");

    public Item(ItemType type = ItemType.None)
    {
        _type = type;
    }

    public void Display(Vector3 pos)
    {
        switch (_type)
        {
            case ItemType.None:
                break;
            case ItemType.Key:
                Object.Instantiate(_keyTemplate, pos, Quaternion.identity);
                break;
            default:
                break;
        }
    }

}

public class MazeCell : System.IDisposable
{
    public const float CELL_WIDTH = 10;
    public const float WALL_WIDTH = 1.5f;

    private Dictionary<Vector2Int, WallState> _wallState = new Dictionary<Vector2Int, WallState>();
    private Vector2Int _position;
    private Item _item;
    private List<GameObject> _walls;
    private static GameObject _wallTemplate = Resources.Load<GameObject>("Wall");

    public readonly Vector2 cellCenter;
    public static List<Vector2Int> neighbours = new List<Vector2Int> { Vector2Int.up, 
                                                                       Vector2Int.left, 
                                                                       Vector2Int.down, 
                                                                       Vector2Int.right };

    public Item Item { get => _item; set => _item = value; }
    public Vector2Int Position => _position;

    public MazeCell(Vector2Int pos, WallState up, WallState left, WallState down, WallState right)
    {
        _position = pos;
        _wallState[Vector2Int.up] = up;
        _wallState[Vector2Int.left] = left;
        _wallState[Vector2Int.down] = down;
        _wallState[Vector2Int.right] = right;
        _walls = new List<GameObject>();
        _item = new Item();
        cellCenter = new Vector2(CELL_WIDTH * (_position.x + 0.5f), CELL_WIDTH * (_position.y + 0.5f));
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

    /// <summary>
    /// Checks if this cell is a corridor (only 2 entrances) and 
    /// returns an entrance (different from the input one)
    /// </summary>
    /// <param name="incomingDirection">The direction of the entrance that should not be considered</param>
    /// <returns>An opposite corridor entrance or Vector2Int.zero if this is not a corridor</returns>
    public Vector2Int GetCorridorOpening(Vector2Int incomingDirection)
    {
        List<Vector2Int> otherOpenings = new List<Vector2Int>();
        foreach (Vector2Int direction in neighbours)
        {
            if (direction != incomingDirection && _wallState[direction] == WallState.Destroyed)
            {
                otherOpenings.Add(direction);
            }
        }
        return otherOpenings.Count == 1 ? otherOpenings[0] : Vector2Int.zero;
    }

    /// <summary>
    /// Creates a wall GameObject at the specified position
    /// </summary>
    /// <param name="pos">The position to place the wall at</param>
    /// <param name="horizontal">Determines whether the wall is horizontal or vertical</param>
    private void PutWall(Vector3 pos, bool horizontal = true)
    {
        GameObject wall = Object.Instantiate(_wallTemplate, pos, Quaternion.identity);
        float wallX = horizontal ? CELL_WIDTH + WALL_WIDTH : WALL_WIDTH;
        float wallY = horizontal ? WALL_WIDTH : CELL_WIDTH + WALL_WIDTH;

        // a random value is added, so that the overlap is better rendered
        float wallHeight = CELL_WIDTH + Random.value * 0.1f;

        wall.transform.localScale = new Vector3(wallX, wallHeight, wallY);
        _walls.Add(wall);
    }

    /// <summary>
    /// Instantiates walls according to _wallState
    /// </summary>
    public void Display()
    {
        _item.Display(new Vector3(cellCenter.x, 0, cellCenter.y));
        if (WallExists(Vector2Int.right))
        {
            PutWall(new Vector3(CELL_WIDTH * (_position.x + 1), 0, CELL_WIDTH * (_position.y + 0.5f)), false);
        }
        if (WallExists(Vector2Int.up))
        {
            PutWall(new Vector3(CELL_WIDTH * (_position.x + 0.5f), 0, CELL_WIDTH * (_position.y + 1)), true);
        }
        if (WallExists(Vector2Int.left))
        {
            PutWall(new Vector3(CELL_WIDTH * _position.x, 0, CELL_WIDTH * (_position.y + 0.5f)), false);
        }
        if (WallExists(Vector2Int.down))
        {
            PutWall(new Vector3(CELL_WIDTH * (_position.x + 0.5f), 0, CELL_WIDTH * _position.y), true);
        }
    }

    public void Dispose()
    {
        foreach (GameObject child in _walls)
        {
            Object.Destroy(child);
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
            ind.Add(i);
        }
        ind.Shuffle();
        for (int i = 0; i < items.Count; i++)
        {
            MazeCell cur = cells[ind[i]];
            if (cur.Position != _start && cur.Position != _end)
            {
                cur.Item = new Item(items[i]);
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
