using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    private LevelState _levelState;
    private LevelData _levelData;
    private List<Movable> _mobs;

    public GameMode GameMode { get; set; }
    public LevelData LevelData { get => _levelData; }

    public GameObject exitDoor;

    public bool LevelIs(LevelState state) => (_levelState & state) != 0;

    public void Initialize(LevelData levelData)
    {
        _levelData = levelData;
        _levelState = LevelState.InProgress;
        Player.Instance.TimeLeft = levelData.time;
        GameMode = levelData.GetGameMode();

        Maze.Instance.Load(levelData.mazeState);

        Player.Instance.PlaceOn(Maze.Instance);
        exitDoor.GetComponent<ExitDoor>().MoveToExit(Maze.Instance);

        _mobs = levelData.SpawnMovables();
        Movable.ClearHistory();
    }

    void ResetState()
    {
        Maze.Instance.Load(_levelData.mazeState);

        Player.Instance.Reset();
        foreach (Movable mob in _mobs)
        {
            mob.Reset();
        }
    }

    public float GetSpeedMultiplier()
    {
        if (LevelIs(LevelState.InReplay))
        {
            return 1 / GameManager.Instance.replayMultiplier;
        }
        else if (LevelIs(LevelState.InReplayReversed))
        {
            return 1 / GameManager.Instance.reversedReplayMultiplier;
        }
        return 1;
    }

    /// <summary>
    /// Replays player movements from the start.
    /// </summary>
    /// <param name="onComplete">Action to perform when the replay is complete</param>
    public void WatchReplay(Action onComplete)
    {   
        _levelState |= LevelState.InReplay;
        Player.Instance.TimeLeft = LevelData.time;
        ResetState();
        StartCoroutine(Movable.ReplayCommands(
            timeMultiplier: GameManager.Instance.replayMultiplier,
            onComplete: () => {
                _levelState ^= LevelState.InReplay;
                onComplete();
            }
        ));
    }

    /// <summary>
    /// Replays the player movements reversely.
    /// Either renders the same level or the next one.
    /// </summary>
    public void LoadCurrentLevel()
    {
        _levelState |= LevelState.InReplayReversed;
        LightManager.Instance.TurnOff();

        StartCoroutine(Movable.ReplayCommands(
            reversed: true,
            timeMultiplier: GameManager.Instance.reversedReplayMultiplier,
            onComplete: () =>
            {
                Clear();
                GameManager.Instance.LoadLevel();
            }
        ));
    }

    /// <summary>
    /// Called when the level ends (player wins/loses)
    /// </summary>
    public void EndLevel(bool mazeCompleted)
    {
        _levelState = mazeCompleted ? LevelState.Completed : LevelState.Failed;

        if (mazeCompleted)
        {
            LightManager.Instance.TurnOn();
            CameraManager.Instance.FocusOnMaze(Maze.Instance);
            SaveLevelProgress();
            if (!GameManager.Instance.IsLastLevel())
            {
                GameManager.Instance.CurrentSettings.id++;
            }
        }
        UIManager.Instance.ShowFinishMenu(mazeCompleted);
    }

    private void SaveLevelProgress()
    {
        LevelSettings currentLevelSettings = GameManager.Instance.CurrentSettings;
        LevelData currentLevelData = LevelIO.LoadLevel(currentLevelSettings);
        if (!currentLevelData.complete)
        {
            IncreasePlayerScore();
            currentLevelData.complete = true;            
            LevelIO.SaveLevel(currentLevelSettings, currentLevelData);            
        }       
    }

    private void IncreasePlayerScore()
    {
        int previousScore = PlayerPrefs.GetInt("PlayersCoins", 0);
        int newScore = previousScore + _levelData.points;
        PlayerPrefs.SetInt("PlayersCoins", newScore);
    }

    void Update()
    {
        if (LevelIs(LevelState.InProgress) ||
           (!LevelIs(LevelState.Completed) && LevelIs(LevelState.InReplay | LevelState.InReplayReversed)))
        {
            CameraManager.Instance.FocusOnPlayer();
        }

        if (LevelIs(LevelState.InProgress))
        {
            if (Player.Instance.TimeLeft < Mathf.Epsilon)
            {
                EndLevel(mazeCompleted: false);
            }

            if (GameMode.GameEnded())
            {
                EndLevel(mazeCompleted: true);
            }
        }
    }

    public void Clear()
    {
        Maze.Instance.Clear();
        foreach (Movable mob in _mobs)
        {
            Destroy(mob.gameObject);
        }
        _mobs.Clear();
        UIManager.Instance.onReplayMenu.SetActive(false);
    }

    protected override void OnDestroy()
    {
        Clear();
        base.OnDestroy();
    }
}
