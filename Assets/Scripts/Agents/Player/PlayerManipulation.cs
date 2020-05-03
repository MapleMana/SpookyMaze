using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCommand
{
    public static readonly PlayerCommand Idle = new PlayerCommand((Player player) => false, (Player player) => false);
    public static readonly PlayerCommand PickUpItem = new PlayerCommand(
        (Player player) => player.PickUpItem(), 
        (Player player) => player.PlaceItem()
    );

    /// <summary>
    /// Actions to be performed on the player
    /// </summary>
    /// <param name="player">The target player</param>
    /// <returns>true if the execution was successfull</returns>
    public delegate bool ExecuteCallback(Player player);

    public ExecuteCallback Execute { get; internal set; }
    public ExecuteCallback ExecuteReversed { get; internal set; }

    public PlayerCommand(ExecuteCallback executeMethod, ExecuteCallback executeReversedMethod)
    {
        Execute = executeMethod;
        ExecuteReversed = executeReversedMethod;
    }
}

public class PlayerMovementCommand : PlayerCommand
{
    private Vector2Int _direction;

    public Vector2Int Direction => _direction;

    public static readonly PlayerMovementCommand MoveUp = new PlayerMovementCommand(Vector2Int.up);
    public static readonly PlayerMovementCommand MoveDown = new PlayerMovementCommand(Vector2Int.down);
    public static readonly PlayerMovementCommand MoveLeft = new PlayerMovementCommand(Vector2Int.left);
    public static readonly PlayerMovementCommand MoveRight = new PlayerMovementCommand(Vector2Int.right);

    public PlayerMovementCommand(Vector2Int direction) : base(player => player.Move(direction), player => player.Move(-1 * direction))
    {
        _direction = direction;
    }

    /// <summary>
    /// Returns an appropriate command for the given direction
    /// </summary>
    /// <param name="direction">The direction to get the command for</param>
    /// <returns>Desired command or null if the mapping does not exist</returns>
    public static PlayerMovementCommand FromVector(Vector2Int direction)
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
                        return (touchEnd.x > touchStart.x) ? PlayerMovementCommand.MoveRight : PlayerMovementCommand.MoveLeft;
                    }
                    else
                    {
                        return (touchEnd.y > touchStart.y) ? PlayerMovementCommand.MoveUp : PlayerMovementCommand.MoveDown;
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
            return PlayerMovementCommand.MoveUp;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            return PlayerMovementCommand.MoveDown;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            return PlayerMovementCommand.MoveLeft;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            return PlayerMovementCommand.MoveRight;
        }
        return PlayerCommand.Idle;
    }
}
