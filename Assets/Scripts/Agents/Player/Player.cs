using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class Player : Movable
{
    private static Player _instance;

    private Light _playerLight;
    private bool _controllable = false;
    private float _lightIntensity;
    private Stack<ItemType> _inventory;

    public float playerSpeed;
    [Range(0f, 180f)]
    public float maxLightAngle;
    [Range(0f, 180f)]
    public float minLightAngle;

    public static Player Instance => _instance;
    public Light PlayerLight => _playerLight;
    public float LightIntensity => _lightIntensity;
    public Stack<ItemType> Inventory => _inventory;
    public bool Controllable { get => _controllable; set => _controllable = value; }

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
        Movable._commandHistory = new List<KeyValuePair<Movable, PlayerCommand>>();
        _inventory = new Stack<ItemType>();
    }

    private void Start()
    {
        _playerLight = GetComponentInChildren<Light>();
        _lightIntensity = _playerLight.intensity;
    }
        
    /// <summary>
    /// Sets the angle of the light cone above the player
    /// </summary>
    /// <param name="coef">Completeness (max -> min) coefficient</param>
    /// <param name="min">Minimal value to interpolate from</param>
    /// <param name="max">Maximal value to interpolate to</param>
    public void LerpLightAngle(float? min = null, float? max = null, float coef = 0)
    {
        _playerLight.spotAngle = Mathf.Lerp(min ?? minLightAngle, max ?? maxLightAngle, coef);
    }

    void Update()
    {
        if (_controllable)
        {
            PlayerCommand picking = PlayerCommand.PickUpItem;
            if (picking.Execute(this).Succeeded)
            {
                AddToHistory(this, picking);
            }

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

    /// <summary>
    /// Picks any item from the cell the player is currently standing on 
    /// and places the item in the player's inventory
    /// </summary>
    /// <returns>true if the cell was not empty</returns>
    public bool PickUpItem()
    {
        MazeCell currentCell = Maze.Instance[MazePosition];
        if (!currentCell.IsEmpty)
        {
            currentCell.Item.Activate();
            _inventory.Push(currentCell.ClearItem());
            return true;
        }
        return false;
    }

    /// <summary>
    /// Places the last item that the player picked up on the ground
    /// </summary>
    /// <returns>true if the cell was empty and the inventory wasn't</returns>
    public bool PlaceItem()
    {
        MazeCell currentCell = Maze.Instance[MazePosition];
        if (currentCell.IsEmpty && _inventory.Count > 0)
        {
            currentCell.Item = ItemFactory.GetItem(_inventory.Pop());
            currentCell.Item.Deactivate();
            currentCell.Item.Display(currentCell.CellCenter(y: 0));
            return true;
        }
        return false;
    }
}

