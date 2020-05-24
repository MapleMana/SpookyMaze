using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Player : Movable
{
    private const float GHOST_EFFECTIVENESS = 0.3f; // percentage of the total time to add

    public float playerSpeed;
    [Range(0f, 180f)]
    public float maxLightAngle;
    [Range(0f, 180f)]
    public float minLightAngle;

    public static Player Instance { get; private set; }
    public Light Light { get; private set; }
    public float DefaultLightIntensity { get; private set; }
    public Stack<ItemType> Inventory { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        _commandHistory = new List<KeyValuePair<Movable, PlayerCommand>>();
        Inventory = new Stack<ItemType>();
    }

    private void Start()
    {
        Light = GetComponentInChildren<Light>();
        DefaultLightIntensity = Light.intensity;
    }
        
    /// <summary>
    /// Sets the angle of the light cone above the player
    /// </summary>
    /// <param name="coef">Completeness (max -> min) coefficient</param>
    /// <param name="min">Minimal value to interpolate from</param>
    /// <param name="max">Maximal value to interpolate to</param>
    public void LerpLightAngle(float? min = null, float? max = null, float coef = 0)
    {
        Light.spotAngle = Mathf.Lerp(min ?? minLightAngle, max ?? maxLightAngle, coef);
    }

    void Update()
    {
        if (LevelManager.Instance.LevelIs(LevelState.InProgress))
        {
            PlayerCommand command = PlayerActionDetector.DetectDesktop();
            if (!_moving && command != null && command.Execute(this).Succeeded)
            {
                AddToHistory(this, command);
                MoveToDecisionPoint(incomingDirection: ((PlayerMovementCommand)command).Direction);
            }
        }
    }

    /// <summary>
    /// Moves the player to the next decision point in the maze (intersection or dead end)
    /// </summary>
    private void MoveToDecisionPoint(Vector2Int incomingDirection)
    {
        List<Vector2Int> movementSequence = Maze.Instance.GetSequenceToDicisionPoint(
            position: MazePosition,
            incomingDirection: incomingDirection
        );
        List<PlayerCommand> commandSequence = new List<PlayerCommand>();
        for (int i = 0; i < movementSequence.Count; i++)
        {
            Vector2Int direction = movementSequence[i];
            PlayerMovementCommand newMovement = PlayerMovementCommand.FromVector(direction);
            commandSequence.Add(newMovement);
        }

        StartCoroutine(PlayCommandsInRealTime(
            playerCommands: commandSequence,
            pauseBetween: 1 / playerSpeed
       ));
    }

    /// <summary>
    /// Move player in the chosen direction. If there is a wall on the way, player idles
    /// </summary>
    /// <param name="direction">The direction of movement</param>
    /// <returns>true if the movement completed</returns>
    public override bool Move(Vector2Int direction)
    {
        if (!Maze.Instance[MazePosition].WallExists(direction))
        {
            MazePosition += direction;
            return true;
        }
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Item"
            && LevelManager.Instance.LevelIs(LevelState.InProgress))
        {
            PlayerCommand keyPicking = PlayerCommand.PickUpItem;
            if (keyPicking.Execute(this).Succeeded)
            {
                AddToHistory(this, keyPicking);
            }
        }
    }

    /// <summary>
    /// Picks any item from the cell the player is currently standing on 
    /// and places the item in the player's inventory
    /// </summary>
    /// <returns>true if the cell was not empty</returns>
    public bool PickUpItem()
    {
        GameObject itemObject = Maze.Instance[MazePosition].Item;
        Item item = itemObject.GetComponent<Item>();
        item.Activate();
        Inventory.Push(item.Type);
        Destroy(itemObject);
        return true;
    }

    /// <summary>
    /// Places the last item that the player picked up on the ground
    /// </summary>
    /// <returns>true if the cell was empty and the inventory wasn't</returns>
    public bool PlaceItem()
    {
        GameObject itemObject = ItemFactory.SpawnItem(Inventory.Pop(), transform.position);
        Item item = itemObject.GetComponent<Item>();
        Maze.Instance[MazePosition].Item = itemObject;
        item.Deactivate();
        return true;
    }

    /// <summary>
    /// Takes place when player incounters a ghost
    /// </summary>
    /// <returns></returns>
    public bool EncounterGhost()
    {
        LevelManager.Instance.AddTime(ratio: -GHOST_EFFECTIVENESS);
        return true;
    }

    /// <summary>
    /// Reverts ghost reduction of the time
    /// </summary>
    /// <returns></returns>
    public bool LeaveGhost()
    {
        LevelManager.Instance.AddTime(ratio: GHOST_EFFECTIVENESS);
        return true;
    }
}