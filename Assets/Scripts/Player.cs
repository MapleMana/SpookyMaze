using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player _instance;

    private Vector2Int _mazePosition;
    private PlayerCommand _command;
    private List<PlayerCommand> playerCommands;
    private Light _playerLight;
    private bool _canMove = false;
    
    private bool replayInProgress = false;
    public static float PAUSE_IN_REPLAY = 0.15f;

    [Range(0f, 180f)]
    public float maxLightAngle;
    [Range(0f, 180f)]
    public float minLightAngle;

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
        _playerLight = GetComponentInChildren<Light>();
        playerCommands = new List<PlayerCommand>();
    }

    /// <summary>
    /// Places the player at the start of the maze and inits player's state
    /// </summary>
    public void Reset()
    {
        _mazePosition = Maze.Instance.Start;
        SyncRealPosition();
        playerCommands.Clear();
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
        _command = PlayerActionDetector.DetectDesktop();
        if (_command != PlayerCommand.Idle && !replayInProgress)
        {
            playerCommands.Add(_command);
            ExecuteLastCommand();
        }
        
        if (_mazePosition == Maze.Instance.End && !replayInProgress)
        {
            replayInProgress = true;
            GameManager.Instance.EndLevel();
        }
    }

    private void ExecuteLastCommand()
    {
        PlayerCommand last = playerCommands.Last();
        last.Execute(this);
        SyncRealPosition();
    }

    public void Move(Vector2Int direction)
    {
        if (!Maze.Instance.Grid[_mazePosition].WallExists(direction))
        {
            _mazePosition += direction;
            SyncRealPosition();
        }
    }

    /// <summary>
    /// Player is returned to the inital spot, all movements from there are replayed
    /// </summary>
    /// <returns></returns>
    public IEnumerator ReplayMovementsFromStart()
    {
        yield return new WaitForSeconds(PAUSE_IN_REPLAY);
        this.Reset();
        yield return new WaitForSeconds(PAUSE_IN_REPLAY);
        for (int i = 0; i < playerCommands.Count; i++)
        {
            PlayerCommand command = playerCommands[i];
            command.Execute(this);

            yield return new WaitForSeconds(PAUSE_IN_REPLAY);
        }

        replayInProgress = false;
    }

    /// <summary>
    /// All player movements are replayed from the finish spot to the start
    /// </summary>
    /// <returns></returns>
    public IEnumerator ReplayMovementsFromFinish()
    {
        yield return new WaitForSeconds(PAUSE_IN_REPLAY);
        for (int i = playerCommands.Count - 1; i >= 0; i--)
        {
            PlayerCommand command = PlayerCommand.reverseCommand[playerCommands[i]];
            command.Execute(this);

            yield return new WaitForSeconds(PAUSE_IN_REPLAY);
        }

        playerCommands.Clear();
        LightManager.Instance.TurnOff();
        GameManager.Instance.LoadLevel("Maze");
        replayInProgress = false;
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
        new PlayerCommand((Player player) => player.Move(Vector2Int.up));

    public static readonly PlayerCommand MoveDown =
        new PlayerCommand((Player player) => player.Move(Vector2Int.down));

    public static readonly PlayerCommand MoveLeft =
        new PlayerCommand((Player player) => player.Move(Vector2Int.left));

    public static readonly PlayerCommand MoveRight =
        new PlayerCommand((Player player) => player.Move(Vector2Int.right));

    public static readonly PlayerCommand Idle =
        new PlayerCommand((Player player) => { });

    public static Dictionary<PlayerCommand, PlayerCommand> reverseCommand = new Dictionary<PlayerCommand, PlayerCommand>
    {
        [MoveUp] = MoveDown,
        [MoveDown] = MoveUp,
        [MoveRight] = MoveLeft,
        [MoveLeft] = MoveRight
    };

    public delegate void ExecuteCallback(Player player);

    public PlayerCommand(ExecuteCallback executeMethod)
    {
        Execute = executeMethod;
    }

    public ExecuteCallback Execute { get; private set; }
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
