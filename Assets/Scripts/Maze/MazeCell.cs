using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MazeCell : System.IDisposable
{
    public const float CELL_WIDTH = 10;
    public const float WALL_WIDTH = 1.5f;
    public const float WALL_HEIGHT_INCREMENT = 0.01f;

    private Dictionary<Vector2Int, WallState> _wallState = new Dictionary<Vector2Int, WallState>();
    private Vector2Int _position;
    private List<GameObject> _walls;
    private static GameObject _wallTemplate = Resources.Load<GameObject>("Wall");
    private static float _wallHeight = WALL_WIDTH;

    public static List<Vector2Int> neighbours = new List<Vector2Int> { Vector2Int.up,
                                                                       Vector2Int.left,
                                                                       Vector2Int.down,
                                                                       Vector2Int.right };

    public ItemType ItemType { get; set; }
    public GameObject Item { get; set; }
    public bool IsEmpty => ItemType == ItemType.None;
    public Vector2Int Position => _position;

    public static float WallHeight { 
        get {
            // this is required for the walls to properly render
            // no 2 walls should have the same height and overlap
            _wallHeight += WALL_HEIGHT_INCREMENT;
            return _wallHeight; 
        } 
    }

    public Vector3 CellCenter(float y) => new Vector3(CELL_WIDTH * (_position.x + 0.5f), y, CELL_WIDTH * (_position.y + 0.5f));

    public MazeCell(Vector2Int pos, WallState up, WallState left, WallState down, WallState right)
    {
        _position = pos;
        _wallState[Vector2Int.up] = up;
        _wallState[Vector2Int.left] = left;
        _wallState[Vector2Int.down] = down;
        _wallState[Vector2Int.right] = right;
        _walls = new List<GameObject>();
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
    public bool WallExists(Vector2Int direction)
    {
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
        float wallY = horizontal ? WALL_WIDTH              : CELL_WIDTH + WALL_WIDTH;

        wall.transform.localScale = new Vector3(wallX, WallHeight, wallY);
        SceneManager.MoveGameObjectToScene(wall, SceneManager.GetSceneByName("Maze"));
        _walls.Add(wall);
    }

    /// <summary>
    /// Instantiates walls according to _wallState
    /// </summary>
    public void Instantiate()
    {
        Item = ItemFactory.SpawnItem(ItemType, CellCenter(y: 0));
        if (WallExists(Vector2Int.right))
        {
            PutWall(new Vector3(CELL_WIDTH * (_position.x + 1), 0, CELL_WIDTH * (_position.y + 0.5f)), false);
        }
        if (WallExists(Vector2Int.up))
        {
            PutWall(new Vector3(CELL_WIDTH * (_position.x + 0.5f), 0, CELL_WIDTH * (_position.y + 1)), true);
        }

        if (WallExists(Vector2Int.down) && _position.y == 0)
        {
            PutWall(new Vector3(CELL_WIDTH * (_position.x + 0.5f), 0, CELL_WIDTH * _position.y), true);
        }
        if (WallExists(Vector2Int.left) && _position.x == 0)
        {
            PutWall(new Vector3(CELL_WIDTH * _position.x, 0, CELL_WIDTH * (_position.y + 0.5f)), false);
        }
    }

    public void Dispose()
    {
        foreach (GameObject child in _walls)
        {
            Object.Destroy(child);
        }
        _walls.Clear();
        Object.Destroy(Item);
    }
}
