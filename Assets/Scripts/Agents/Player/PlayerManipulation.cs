using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Verdict
{
    public bool Succeeded { get; }
    public float Time { get; }

    public Verdict(bool succeeded, float time=0)
    {
        Succeeded = succeeded;
        Time = time;
    }
}

public class PlayerCommand
{
    public static readonly PlayerCommand PickUpItem = new PlayerCommand(
        (Movable movable) => new Verdict(((Player)movable).PickUpItem()),
        (Movable movable) => new Verdict(((Player)movable).PlaceItem())
    );

    public static readonly PlayerCommand EncounterPlayer = new PlayerCommand(
        (Movable movable) => new Verdict(((Ghost)movable).EncounterPlayer()),
        (Movable movable) => new Verdict(((Ghost)movable).LeavePlayer())
    );

    /// <summary>
    /// Actions to be performed on the player
    /// </summary>
    /// <param name="player">The target player</param>
    /// <returns>true if the execution was successfull</returns>
    public delegate Verdict ExecuteCallback(Movable movable);

    public ExecuteCallback Execute { get; internal set; }
    public ExecuteCallback ExecuteReversed { get; internal set; }

    public PlayerCommand(ExecuteCallback executeMethod, ExecuteCallback executeReversedMethod)
    {
        Execute = executeMethod;
        ExecuteReversed = executeReversedMethod;
    }

    public static PlayerCommand CreateIdle(float time)
    {
        return new PlayerCommand((Movable movable) => new Verdict(false, time), (Movable movable) => new Verdict(false, time));
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