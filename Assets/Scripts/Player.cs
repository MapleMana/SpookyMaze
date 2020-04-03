using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player _instance;

    private Vector2Int _mazePosition;
    private List<PlayerCommand> _playerLevelCommands;
    private Light _playerLight;
    private bool _canMove = false;

    public float replayTime;
    public float reversedReplayTime;

    [Range(0f, 180f)]
    public float maxLightAngle;
    [Range(0f, 180f)]
    public float minLightAngle;

    public static Player Instance { get => _instance; }
    public Light PlayerLight { get => _playerLight; }
    public bool CanMove { get => _canMove; set => _canMove = value; }

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
        _playerLevelCommands = new List<PlayerCommand>();
    }

    private void Start()
    {
        _playerLight = GetComponentInChildren<Light>();
    }

    /// <summary>
    /// Places the player at the start of the maze and inits player's state
    /// </summary>
    public void ResetState()
    {
        _mazePosition = Maze.Instance.Start;
        SyncRealPosition();
        _playerLevelCommands.Clear();
        _canMove = true;
    }
    
    /// <summary>
    /// Sets the angle of the light cone above the player
    /// </summary>
    /// <param name="coef">Completeness (max -> min) coefficient</param>
    public void SetLightAngle(float coef)
    {
        _playerLight.spotAngle = Mathf.Lerp(minLightAngle, maxLightAngle, coef);
    }

    void Update()
    {
        PlayerMovementCommand _command = (PlayerMovementCommand)PlayerActionDetector.DetectDesktop();
        if (_canMove && _command.Execute(this))
        {
            _playerLevelCommands.Add(_command);
            SyncRealPosition();
            Vector2Int previousMoveDirection = _command.Direction;
            //while (Maze.Instance.Grid[_mazePosition].GetCorridorOpening(previousMoveDirection) != Vector2Int.zero)
            //{
            //    MazeCell curCell
            //}
        }
        
        // when the player reaches the end (not from replay)
        if (_canMove && _mazePosition == Maze.Instance.End)
        {
            GameManager.Instance.EndLevel(mazeComplete: true);
        }
    }

    private void ExecuteLastCommand()
    {
        PlayerCommand last = _playerLevelCommands.Last();
        last.Execute(this);
        SyncRealPosition();
    }

    /// <summary>
    /// Move player in the chosen direction, but if there is a wall on the way,
    /// it doesn't do anything and remove this command from the commands list 
    /// </summary>
    /// <param name="direction"></param>
    public bool Move(Vector2Int direction)
    {
        if (!Maze.Instance.Grid[_mazePosition].WallExists(direction))
        {
            _mazePosition += direction;
            SyncRealPosition();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Player is returned to the inital spot, all movements from there are replayed
    /// </summary>
    /// <param name="onComplete">Action to perform after the replay is comlete</param>
    /// <returns>A couroutine to execute</returns>
    public IEnumerator PlayCommands(
        List<PlayerCommand> playerCommands = null,
        bool reversed = false,
        Vector2Int? initialPosition = null,
        float? playTime = null,
        Action onComplete = null)
    {
        playerCommands = playerCommands ?? _playerLevelCommands;
        if (reversed)
        {
            playerCommands.Reverse();
        }
        _mazePosition = initialPosition ?? _mazePosition;

        float pauseBetweenCommands = (playTime ?? replayTime) / playerCommands.Count;

        foreach (PlayerCommand command in playerCommands)
        {
            yield return new WaitForSeconds(pauseBetweenCommands);
            PlayerCommand execution = reversed ? PlayerCommand.reverseCommand[command] : command;
            execution.Execute(this);
        }
        onComplete?.Invoke();
    }

    /// <summary>
    /// Synchronizes maze position and physical player position
    /// </summary>
    void SyncRealPosition()
    {
        MazeCell currentCell = Maze.Instance.Grid[_mazePosition];
        transform.position = new Vector3(currentCell.cellCenter.x, transform.position.y, currentCell.cellCenter.y);
    }
}

public class PlayerCommand
{
    public static readonly PlayerCommand MoveUp =
        new PlayerMovementCommand(Vector2Int.up);

    public static readonly PlayerCommand MoveDown =
        new PlayerMovementCommand(Vector2Int.down);

    public static readonly PlayerCommand MoveLeft =
        new PlayerMovementCommand(Vector2Int.left);

    public static readonly PlayerCommand MoveRight =
        new PlayerMovementCommand(Vector2Int.right);

    public static readonly PlayerCommand Idle =
        new PlayerCommand((Player player) => false);

    public static Dictionary<PlayerCommand, PlayerCommand> reverseCommand = new Dictionary<PlayerCommand, PlayerCommand>
    {
        [MoveUp] = MoveDown,
        [MoveDown] = MoveUp,
        [MoveRight] = MoveLeft,
        [MoveLeft] = MoveRight,
        [Idle] = Idle
    };

    public delegate bool ExecuteCallback(Player player);

    public PlayerCommand(ExecuteCallback executeMethod)
    {
        Execute = executeMethod;
    }

    public ExecuteCallback Execute { get; internal set; }
}

public class PlayerMovementCommand : PlayerCommand
{
    private Vector2Int _direction;

    public PlayerMovementCommand(Vector2Int direction) : base(player => false)
    {
        _direction = direction;
        Execute = (Player player) => player.Move(direction);
    }

    public Vector2Int Direction { get => _direction; }
}

/// <summary>
/// Detects input for different platforms. Methods to be called on Update.
/// </summary>
static class PlayerActionDetector
{
    static private Vector3 touchStart;
    const double minSwipeDistance = 0.1;  //minimum distance for a swipe to be registered (fraction of screen height)

    /// <summary>
    /// Detects swipes on mobile platforms
    /// </summary>
    /// <returns>Direction of movement</returns>
    public static PlayerCommand DetectMobile()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchStart = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                Vector3 touchEnd = touch.position;

                if (Vector3.Distance(touchStart, touchEnd) > minSwipeDistance * Screen.height)
                {
                    // check which axis is more significant
                    if (Mathf.Abs(touchEnd.x - touchStart.x) > Mathf.Abs(touchEnd.y - touchStart.y))
                    {
                        return (touchEnd.x > touchStart.x) ? PlayerCommand.MoveRight : PlayerCommand.MoveLeft;
                    }
                    else
                    {
                        return (touchEnd.y > touchStart.y) ? PlayerCommand.MoveUp : PlayerCommand.MoveDown;
                    }
                }
            }
        }
        return PlayerCommand.Idle;
    }

    /// <summary>
    /// Detects arrow key presses on desktop
    /// </summary>
    /// <returns>Direction of movement</returns>
    public static PlayerCommand DetectDesktop()
    {
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            return PlayerCommand.MoveUp;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            return PlayerCommand.MoveDown;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            return PlayerCommand.MoveLeft;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            return PlayerCommand.MoveRight;
        }
        return PlayerCommand.Idle;
    }
}
