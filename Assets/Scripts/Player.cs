using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player _instance;

    private Vector2Int _mazePosition;
    private PlayerCommand _command;
    
    private List<PlayerCommand> playerCommands = new List<PlayerCommand>();

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
        SyncRealPosition();
    }
    
    void Update()
    {
        if (_command == null)
        {
            _command = PlayerInputHandler.HandleInput();
            playerCommands.Add(_command);
            ExecuteCommandsRoutine();
        }
        _command = PlayerInputHandler.HandleInput();
    }

    /// <summary>
    /// Synchronizes maze position and physical player position
    /// </summary>
    void SyncRealPosition()
    {
        MazeCell currentCell = Maze.Instance.Grid[_mazePosition];
        transform.position = new Vector3(currentCell.cellCenter.x, transform.position.y, currentCell.cellCenter.y);
    }

    private void ExecuteCommandsRoutine()
    {
        PlayerCommand last = playerCommands.Last();
        if (last != null)
        {
            last.Execute(this);
            SyncRealPosition();
        }
    }

    public void Move(Vector2Int direction)
    {
        if (!Maze.Instance.Grid[_mazePosition].WallExists(direction))
        {
            _mazePosition += direction;
            SyncRealPosition();
        }
    }
}

public class PlayerCommand
{
    public delegate void ExecuteCallback(Player player);

    public ExecuteCallback Execute { get; private set; }

    public PlayerCommand(ExecuteCallback executeMethod)
    {
        Execute = executeMethod;
    }
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

