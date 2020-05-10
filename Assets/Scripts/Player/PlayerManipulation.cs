using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Verdict
{
    private bool _wasSuccessfull;
    private float _executionTime;
    public bool Succeeded => _wasSuccessfull;
    public float Time => _executionTime;

    public Verdict(bool succeeded, float time=0)
    {
        _wasSuccessfull = succeeded;
        _executionTime = time;
    }
}

public class PlayerCommand
{
    //public static readonly PlayerCommand Idle = new PlayerCommand((Player player) => new Verdict(false), (Player player) => new Verdict(false));
    public static readonly PlayerCommand PickUpItem = new PlayerCommand(
        (Player player) => new Verdict(player.PickUpItem()),
        (Player player) => new Verdict(player.PlaceItem())
    );

    /// <summary>
    /// Actions to be performed on the player
    /// </summary>
    /// <param name="player">The target player</param>
    /// <returns>true if the execution was successfull</returns>
    public delegate Verdict ExecuteCallback(Player player);

    public ExecuteCallback Execute { get; internal set; }
    public ExecuteCallback ExecuteReversed { get; internal set; }

    public PlayerCommand(ExecuteCallback executeMethod, ExecuteCallback executeReversedMethod)
    {
        Execute = executeMethod;
        ExecuteReversed = executeReversedMethod;
    }

    public static PlayerCommand CreateIdle(float time)
    {
        return new PlayerCommand((Player player) => new Verdict(false, time), (Player player) => new Verdict(false, time));
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

    public PlayerMovementCommand(Vector2Int direction) : base(player => new Verdict(player.Move(direction)), player => new Verdict(player.Move(-1 * direction)))
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

