using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Movable
{
    public float ghostSpeed;
    public static bool CanBeMoved { get; set; } = false;

    void Update()
    {
        if (CanBeMoved)
        {
            if (!_moving) {
                StartCoroutine(PlayCommandsInRealTime(
                    playerCommands: new List<PlayerCommand> { PlayerMovementCommand.FromVector(getRandomDirection()) },
                    pauseBetween: 1 / ghostSpeed
                    ));
            }
        }
    }
    
    public override bool Move(Vector2Int direction)
    {
        MazePosition += direction;
        return true;
    }

    private Vector2Int getRandomDirection()
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
            PlayerCommand ghostEncounter = PlayerCommand.EncounterGhost;
            Player player = other.gameObject.GetComponent<Player>();
            if (ghostEncounter.Execute(player).Succeeded)
            {
                AddToHistory(player, ghostEncounter);
            }
        }
    }
}
