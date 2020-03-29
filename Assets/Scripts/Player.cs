using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player _instance;

    private Vector2Int _mazePosition;
    private PlayerCommand _command;
    
    private List<PlayerCommand> playerCommands = new List<PlayerCommand>();
    private Coroutine executeRoutine;
    const float PAUSE_TIME = 0.3f;

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

    public void PlaceOnMaze()
    {
        _mazePosition = Maze.Instance.start;
    }
    
    void Update()
    {
        _command = PlayerInputHandler.HandleInput();
        if (_command != null && executeRoutine == null)
        {
            playerCommands.Add(_command);
            executeRoutine = StartCoroutine(ExecuteCommandsRoutine());
        }
    }

    private IEnumerator ExecuteCommandsRoutine()
    {
        playerCommands[playerCommands.Count - 1].Execute(this);
        MazeCell currentCell = Maze.Instance.Grid[_mazePosition];
        transform.position = new Vector3(currentCell.cellCenter.x, transform.position.y, currentCell.cellCenter.y);
        yield return new WaitForSeconds(PAUSE_TIME);
        executeRoutine = null;
    }

    public void Move(Vector2Int direction)
    {
        if (!Maze.Instance.Grid[_mazePosition].WallExists(direction))
        {
            _mazePosition += direction;
        }
    }
}

public class PlayerCommand
{
    public PlayerCommand(ExecuteCallback executeMethod)
    {
        Execute = executeMethod;
    }

    public delegate void ExecuteCallback(Player player);

    public ExecuteCallback Execute { get; private set; }
}

public class PlayerInputHandler
{
    private static readonly PlayerCommand MoveUp =
        new PlayerCommand(delegate (Player player) { player.Move(Vector2Int.up); });

    private static readonly PlayerCommand MoveDown =
        new PlayerCommand(delegate (Player player) { player.Move(Vector2Int.down); });

    private static readonly PlayerCommand MoveLeft =
        new PlayerCommand(delegate (Player player) { player.Move(Vector2Int.left); });

    private static readonly PlayerCommand MoveRight =
        new PlayerCommand(delegate (Player player) { player.Move(Vector2Int.right); });

    public static PlayerCommand HandleInput()
    {
        if (Input.GetAxis("Horizontal") > Mathf.Epsilon)
        {
            return MoveRight;
        }
        else if (Input.GetAxis("Horizontal") < -Mathf.Epsilon)
        {
            return MoveLeft;
        }
        else if (Input.GetAxis("Vertical") > Mathf.Epsilon)
        {
            return MoveUp;
        }
        else if (Input.GetAxis("Vertical") < -Mathf.Epsilon)
        {
            return MoveDown;
        }
        return null;
    }
}

