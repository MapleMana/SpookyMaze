﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player _instance;

    private Vector2Int _mazePosition;
    private List<PlayerCommand> _playerLevelCommands;
    private Light _playerLight;
    private bool _moving = false;
    private bool _canBeMoved= false;
    private float _lightIntensity;

    public float playerSpeed;
    [Range(0f, 180f)]
    public float maxLightAngle;
    [Range(0f, 180f)]
    public float minLightAngle;

    public static Player Instance => _instance;
    public Light PlayerLight => _playerLight;
    public float LightIntensity => _lightIntensity;
    public bool Moving { get => _moving; set => _moving = value; }
    public bool CanBeMoved { get => _canBeMoved && !_moving; set => _canBeMoved = value; }

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
        _lightIntensity = _playerLight.intensity;
    }

    /// <summary>
    /// Places the player at the start of the maze and inits player's state
    /// </summary>
    public void ResetState()
    {
        _mazePosition = Maze.Instance.StartPos;
        SyncRealPosition();
        _playerLevelCommands.Clear();
        _canBeMoved = true;
        _moving = false;
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
        // when the player reaches the end (not from replay)
        if (_canBeMoved && _mazePosition == Maze.Instance.EndPos)
        {
            GameManager.Instance.EndLevel(mazeCompleted: true);
        }

        PlayerCommand _command = PlayerActionDetector.DetectDesktop();
        if (CanBeMoved && _command.Execute(this))
        {
            _playerLevelCommands.Add(_command);
            SyncRealPosition();
            MoveToDecisionPoint(incomingDirection: _command.Direction);
        }
    }

    /// <summary>
    /// Moves the player to the next decision point in the maze (intersection or dead end)
    /// </summary>
    private void MoveToDecisionPoint(Vector2Int incomingDirection)
    {
        List<Vector2Int> movementSequence = Maze.Instance.GetSequenceToDicisionPoint(
            position: _mazePosition,
            incomingDirection: incomingDirection
        );
        List<PlayerCommand> commandSequence = new List<PlayerCommand>();
        for (int i = 0; i < movementSequence.Count; i++)
        {
            Vector2Int direction = movementSequence[i];
            PlayerCommand newMovement = PlayerCommand.FromVector(direction);
            commandSequence.Add(newMovement);
            _playerLevelCommands.Add(newMovement);
        }

        StartCoroutine(PlayCommands(
            playerCommands: commandSequence,
            pauseBetween: 1 / playerSpeed
       ));
    }

    /// <summary>
    /// Move player in the chosen direction. If there is a wall on the way, player idles
    /// </summary>
    /// <param name="direction">The direction of movement</param>
    public bool Move(Vector2Int direction)
    {
        if (!Maze.Instance[_mazePosition].WallExists(direction))
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
        float pauseBetweenCommands = pauseBetween ?? ((playTime ?? 0) / playerCommands.Count);

        SyncRealPosition();
        Moving = true;

        foreach (PlayerCommand command in playerCommands)
        {
            if (Moving)
            {
                yield return new WaitForSeconds(pauseBetweenCommands);
                PlayerCommand execution = reversed ? PlayerCommand.reverseCommand[command] : command;
                execution.Execute(this);
            }
        }
        Moving = false;
        onComplete?.Invoke();
    }

    /// <summary>
    /// Synchronizes maze position and physical player position
    /// </summary>
    void SyncRealPosition()
    {
        MazeCell currentCell = Maze.Instance[_mazePosition];
        transform.position = new Vector3(currentCell.cellCenter.x, transform.position.y, currentCell.cellCenter.y);
    }
}

