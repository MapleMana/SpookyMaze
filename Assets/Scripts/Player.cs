using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player _instance;

    private Vector2Int? _movement;

    public static Player Instance { get => _instance; set => _instance = value; }

    void Awake()
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

    private void Start()
    {
        Vector2 mazeStartCenter = Maze.Instance.start.CellCenter();
        transform.position = new Vector3(mazeStartCenter.x, 0, mazeStartCenter.y);
    }

    private Vector2Int? GetInput()
    {
        if (Input.GetAxis("Horizontal") > Mathf.Epsilon)
        {
            return Vector2Int.right;
        }
        else if (Input.GetAxis("Horizontal") < -Mathf.Epsilon)
        {
            return Vector2Int.left;
        }
        else if (Input.GetAxis("Vertical") > Mathf.Epsilon)
        {
            return Vector2Int.up;
        }
        else if (Input.GetAxis("Vertical") < -Mathf.Epsilon)
        {
            return Vector2Int.down;
        }
        return null;
    }

    void Update()
    {
        if (_movement == null)
        {
            _movement = GetInput();
            transform.position += new Vector3((_movement?.x ?? 0) * MazeCell.CELL_WIDTH, 0, (_movement?.y ?? 0) * MazeCell.CELL_WIDTH);
        }
        _movement = GetInput();
    }
}
