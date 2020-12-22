using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Player : Movable
{
    private float _time;

    [Range(0f, 180f)]
    public float maxLightAngle;
    [Range(0f, 180f)]
    public float minLightAngle;

    public static Player Instance { get; private set; }
    public Light Light { get; private set; }
    public float DefaultLightIntensity { get; private set; }
    public Stack<ItemType> Inventory { get; private set; }
    public float TimeLeft { get => _time; set => _time = value; }

    public ParticleSystem torchParticleSystem;
    private ParticleSystem.EmissionModule torchParticleSystemEmission;
    private const float EMISSION_CONSTANT = 1.25f;
    private const float MAX_EMISSION = 80f;

    protected override void Awake()
    {
        base.Awake();
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        Inventory = new Stack<ItemType>();
    }

    public void PlaceOn(Maze maze)
    {
        StartingPosition = maze.StartPos;
        Reset();
    }

    private void Start()
    {
        Light = GetComponentInChildren<Light>();
        DefaultLightIntensity = Light.intensity;

        // set new emission for torch particle system
        torchParticleSystemEmission = torchParticleSystem.emission;
    }

    public void SubtractTime(float power=1)
    {
        if (LevelManager.Instance.LevelIs(LevelState.InProgress | LevelState.InReplay | LevelState.InReplayReversed))
        {
            float dt = LevelManager.Instance.LevelIs(LevelState.InReplayReversed) ? -1 : 1;
            TimeLeft = Mathf.Clamp(TimeLeft - power * Speed / 30 * dt * Time.deltaTime, 0, LevelManager.Instance.LevelData.time);
            Light.spotAngle = Mathf.Lerp(minLightAngle, maxLightAngle, TimeLeft / LevelManager.Instance.LevelData.time);

            // diminish torch particle system as light diminishes
            torchParticleSystemEmission.rateOverTime = Mathf.Clamp(TimeLeft * EMISSION_CONSTANT, 0, MAX_EMISSION);
        }
    }

    protected override void Update()
    {
        SubtractTime();
        base.Update();
    }

    public override void PerformMovement()
    {
        MovableMovementCommand command = PlayerActionDetector.Detect();
        if (command != null && command.Execute(this).Succeeded)
        {
            if (command == MovableMovementCommand.Stop)
            {
                // remove last command
                // add new commend that moves player to their current position
                transform.position = transform.position;
            }
            AddToHistory(this, command);
            MoveToDecisionPoint(incomingDirection: command.Direction);
        }
    }

    /// <summary>
    /// Moves the player to the next decision point in the maze (intersection or dead end)
    /// </summary>
    private void MoveToDecisionPoint(Vector2Int incomingDirection)
    {
        List<Vector2Int> movementSequence = incomingDirection != Vector2Int.zero ? Maze.Instance.GetSequenceToDicisionPoint(
            position: MazePosition,
            incomingDirection: incomingDirection
        ) : null;
        List<MovableCommand> commandSequence = new List<MovableCommand>();
        for (int i = 0; i < movementSequence.Count; i++)
        {
            Vector2Int direction = movementSequence[i];
            MovableMovementCommand newMovement = MovableMovementCommand.FromVector(direction);
            commandSequence.Add(newMovement);
        }

        StartCoroutine(PlayCommandsInRealTime(
            playerCommands: commandSequence,
            waitBefore: true
        ));
    }

    public override bool Move(Vector2Int direction)
    {
        if (!Maze.Instance[MazePosition].WallExists(direction))
        {
            MazePosition += direction;
            return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Item") && 
            LevelManager.Instance.LevelIs(LevelState.InProgress))
        {
            MovableCommand keyPicking = MovableCommand.PickUpItem;
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
        item.Deactivate();
        Maze.Instance[MazePosition].Item = itemObject;
        return true;
    }
}