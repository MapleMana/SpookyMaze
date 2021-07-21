using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Analytics;

public class LevelManager : Singleton<LevelManager>
{
    public GameObject plane;

    private LevelState _levelState;
    private LevelData _levelData;
    private List<Movable> _mobs;

    private float timeAllowed = 0;

    public GameMode GameMode { get; set; }
    public LevelData LevelData { get => _levelData; }

    public GameObject exitDoor;

    public bool LevelIs(LevelState state) => (_levelState & state) != 0;

    private float frationOfTimeLeft = 0.15f;
    private float endOfLevelSpeed = 1.25f;

    public void Initialize(LevelData levelData)
    {
        UIManager.Instance.ToggleInGameMenu();
        Movable.ClearHistory();
        PlayerActionDetector.ResetTouches();
        _levelData = levelData;
        _levelState = LevelState.InProgress;
        Player.Instance.TimeLeft = levelData.time;
        timeAllowed = levelData.time;
        GameMode = levelData.GetGameMode();

        Maze.Instance.Load(levelData.mazeState);

        Player.Instance.PlaceOn(Maze.Instance);
        Player.Instance.Inventory.Clear();
        exitDoor.GetComponent<ExitDoor>().MoveToExit(Maze.Instance);

        _mobs = levelData.SpawnMovables();

        if (PlayerPrefs.GetInt("isTouch") == 0)
        {
            UIManager.Instance.ToggleInGameControls(true);
        }

        LoadPlaneMaterial(GameManager.Instance.CurrentSettings.gameMode, GameManager.Instance.CurrentSettings.dimensions.Width);

        // select Music
        switch (GameManager.Instance.CurrentSettings.gameMode)
        {
            case "Classic":
            default:
                MusicManager.Instance.PlayMusic(Music.ClassicMusic);
                break;
            case "Dungeon":
                MusicManager.Instance.PlayMusic(Music.DungeonMusic);
                break;
            case "Cursed House":
                MusicManager.Instance.PlayMusic(Music.CursedHouseMusic);
                break;
        }
    }

    private void LoadPlaneMaterial(string mode, int dim)
    {
        plane.GetComponent<FloorPlane>().ChangeFloorMaterial(mode, dim);
        plane.transform.localScale = new Vector3(Maze.Instance.Dimensions.Width, 1f, Maze.Instance.Dimensions.Height);
        plane.transform.position = new Vector3(Maze.Instance.Dimensions.Width * 5f, 0f, Maze.Instance.Dimensions.Height * 5f);
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
        else if (LevelIs(LevelState.InProgress))
        {
            // increase player speed near end of allowed time
            return Player.Instance.TimeLeft < (timeAllowed * frationOfTimeLeft) ? endOfLevelSpeed : 1f;
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
        PlayerActionDetector.ResetTouches();        
        _levelState = mazeCompleted ? LevelState.Completed : LevelState.Failed;
        if (mazeCompleted)
        {
            Analytics.CustomEvent("MazeCompete", new Dictionary<string, object>
            {
                {"Time", $"{GameManager.Instance.CurrentSettings.ToString()}:{timeAllowed - Player.Instance.TimeLeft}/{timeAllowed}"}
            });
            StatsManager.Instance.AddCompletedLevel(GameManager.Instance.CurrentSettings.gameMode, GameManager.Instance.CurrentSettings.dimensions.ToString());
            LightManager.Instance.TurnOn();
            CameraManager.Instance.FocusOnMaze(Maze.Instance);
            SaveLevelProgress();
        }
        else
        {
            UIManager.Instance.FirstTimeCompletingLevel(false);
            Analytics.CustomEvent("MazeNotCompleted - " + GameManager.Instance.CurrentSettings.ToString());
        }
        UIManager.Instance.ToggleInGameMenu();
        UIManager.Instance.ToggleInGameControls(false);
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
            if (!GameManager.Instance.CurrentSettings.isDaily)
            {
                LevelPackData currentlevelPackData = LevelIO.LoadLevelPackData(currentLevelSettings);
                currentlevelPackData.numLevelsComplete += 1;
                LevelIO.SaveLevelPackData(currentLevelSettings, currentlevelPackData);
            }
            UIManager.Instance.FirstTimeCompletingLevel(true);
        }
        else
        {
            UIManager.Instance.FirstTimeCompletingLevel(false);
        }        
    }

    private void IncreasePlayerScore()
    {
        int previousScore = PlayerPrefs.GetInt("PlayersCoins", 0);
        int newScore = previousScore + _levelData.points;
        PlayerPrefs.SetInt("PlayersCoins", newScore);
        PlayerPrefs.Save();
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
            if (mob != null)
            {
                Destroy(mob.gameObject);
            }            
        }
        _mobs.Clear();
        if (UIManager.Instance.onReplayMenu != null)
        {
            UIManager.Instance.onReplayMenu.SetActive(false);
        }        
    }
}
