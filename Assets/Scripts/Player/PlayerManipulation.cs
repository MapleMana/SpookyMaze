using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCommand
{
    private Vector2Int _direction;

    public static readonly PlayerCommand MoveUp = new PlayerCommand(Vector2Int.up);
    public static readonly PlayerCommand MoveDown = new PlayerCommand(Vector2Int.down);
    public static readonly PlayerCommand MoveLeft = new PlayerCommand(Vector2Int.left);
    public static readonly PlayerCommand MoveRight = new PlayerCommand(Vector2Int.right);
    public static readonly PlayerCommand Idle = new PlayerCommand((Player player) => false);

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

    public PlayerCommand(Vector2Int direction)
    {
        Execute = player => player.Move(direction);
        _direction = direction;
    }

    public ExecuteCallback Execute { get; internal set; }
    public Vector2Int Direction => _direction;
    
    /// <summary>
    /// Returns an appropriate command for the given direction
    /// </summary>
    /// <param name="direction">The direction to get the command for</param>
    /// <returns>Desired command or null if the mapping does not exist</returns>
    public static PlayerCommand FromVector(Vector2Int direction)
    {
        if (direction == Vector2Int.up) return MoveUp;
        if (direction == Vector2Int.down) return MoveDown;
        if (direction == Vector2Int.right) return MoveRight;
        if (direction == Vector2Int.left) return MoveLeft;
        return null;
    }
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
