using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player _instance;

    private Vector2Int _mazePosition;
    private List<PlayerCommand> _playerLevelCommands;
    private Light _playerLight;
    private bool _canMove = false;

    public float replayTime;
    public float reversedReplayTime;
    public float playerSpeed;

    [Range(0f, 180f)]
    public float maxLightAngle;
    [Range(0f, 180f)]
    public float minLightAngle;

    public static Player Instance { get => _instance; }
    public Light PlayerLight { get => _playerLight; }
    public bool CanMove { get => _canMove; set => _canMove = value; }

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
        _playerLevelCommands = new List<PlayerCommand>();
    }

    private void Start()
    {
        _playerLight = GetComponentInChildren<Light>();
    }

    /// <summary>
    /// Places the player at the start of the maze and inits player's state
    /// </summary>
    public void ResetState()
    {
        _mazePosition = Maze.Instance.Start;
        SyncRealPosition();
        _playerLevelCommands.Clear();
        _canMove = true;
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
        // when the player reaches the end (not from replay)
        if (_canMove && _mazePosition == Maze.Instance.End)
        {
            GameManager.Instance.EndLevel(mazeComplete: true);
        }

        PlayerCommand _command = PlayerActionDetector.DetectDesktop();
        if (_canMove && _command.Execute(this))
        {
            _playerLevelCommands.Add(_command);
            SyncRealPosition();
            MoveToDecisionPoint();
        }
    }

    /// <summary>
    /// Moves the player to the next decision point in the maze (intersection or dead end)
    /// </summary>
    private void MoveToDecisionPoint()
    {
        PlayerCommand lastCommand = _playerLevelCommands.Last();
        List<Vector2Int> movementSequence = Maze.Instance.GetSequenceToDicisionPoint(
                _mazePosition,
                PlayerCommand.ComDirTranslator(lastCommand)
            );
        List<PlayerCommand> commandSequence = new List<PlayerCommand>();
        for (int i = 0; i < movementSequence.Count; i++)
        {
            Vector2Int direction = movementSequence[i];
            PlayerCommand newMovement = PlayerCommand.ComDirTranslator(direction);
            commandSequence.Add(newMovement);
            _playerLevelCommands.Add(newMovement);
        }
        _canMove = false;
        StartCoroutine(PlayCommands(
            playerCommands: commandSequence,
            pauseBetween: 1 / playerSpeed,
            onComplete: () => _canMove = true
       ));
    }

    /// <summary>
    /// Move player in the chosen direction. If there is a wall on the way, player idles
    /// </summary>
    /// <param name="direction">THe direction of movement</param>
    public bool Move(Vector2Int direction)
    {
        if (!Maze.Instance.Grid[_mazePosition].WallExists(direction))
        {
            _mazePosition += direction;
            SyncRealPosition();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Performs a sequence of commands on the player
    /// </summary>
    /// <param name="playerCommands">The sequence of commands to execute</param>
    /// <param name="reversed">Whether the execution should be reversed (both order and individual commands)</param>
    /// <param name="initialPosition">The starting position of the player. If null, current position is taken.</param>
    /// <param name="playTime">The time the coroutine will take. If null, replay time is taken. This parameter is overriden by pauseBetween</param>
    /// <param name="pauseBetween">Pause between each command. If null, play time is considered.</param>
    /// <param name="onComplete">Action to perform after the replay is comlete</param>
    /// <returns>A coroutine to execute</returns>
    public IEnumerator PlayCommands(
        List<PlayerCommand> playerCommands = null,
        bool reversed = false,
        Vector2Int? initialPosition = null,
        float? playTime = null,
        float? pauseBetween = null,
        Action onComplete = null)
    {
        playerCommands = playerCommands ?? _playerLevelCommands;
        if (reversed)
        {
            playerCommands.Reverse();
        }
        _mazePosition = initialPosition ?? _mazePosition;

        float pauseBetweenCommands = pauseBetween ?? ((playTime ?? replayTime) / playerCommands.Count);

        foreach (PlayerCommand command in playerCommands)
        {
            yield return new WaitForSeconds(pauseBetweenCommands);
            PlayerCommand execution = reversed ? PlayerCommand.reverseCommand[command] : command;
            execution.Execute(this);
        }
        onComplete?.Invoke();
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

