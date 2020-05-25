using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Movable
{
    private const float EFFECTIVENESS = 0.3f; // percentage of the total time to remove

    public float ghostSpeed;
    public static bool CanBeMoved { get; set; } = false;

    void Update()
    {
        if (CanBeMoved && !Moving)
        {
            StartCoroutine(PlayCommandsInRealTime(
                playerCommands: new List<PlayerCommand> { PlayerMovementCommand.FromVector(GetRandomDirection()) },
                pauseBetween: 1 / ghostSpeed
            ));
        }
    }
    
    public override bool Move(Vector2Int direction)
    {
        MazePosition += direction;
        return true;
    }

    private Vector2Int GetRandomDirection()
    {
        MazeCell.neighbours.Shuffle();
        foreach (Vector2Int possibleDirection in MazeCell.neighbours)
        {
            if (Maze.Instance.InBounds(MazePosition + possibleDirection))
            {
                return possibleDirection;
            }
        }
        
        return MazeCell.neighbours[0];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player" && 
            LevelManager.Instance.LevelIs(LevelState.InProgress))
        {
            PlayerCommand playerEncounter = PlayerCommand.EncounterPlayer;
            if (playerEncounter.Execute(this).Succeeded)
            {
                AddToHistory(this, playerEncounter);
            }
        }
    }

    /// <summary>
    /// Takes place when the ghost encounters the player
    /// </summary>
    /// <returns></returns>
    public bool EncounterPlayer()
    {
        LevelManager.Instance.AddTime(ratio: -EFFECTIVENESS);
        return true;
    }

    /// <summary>
    /// Reverts ghost reduction of the time
    /// </summary>
    /// <returns></returns>
    public bool LeavePlayer()
    {
        LevelManager.Instance.AddTime(ratio: EFFECTIVENESS);
        return true;
    }
}
