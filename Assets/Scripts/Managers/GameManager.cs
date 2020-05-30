using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public float replayMultiplier;
    public float reversedReplayMultiplier;
    public int CurrentLevel { get; set; } = 1;
    public GameMode GameMode { get; set; }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnFullLoad;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnFullLoad;
    }

    private void Start()
    {
        Maze.Initialize();
        GameMode = new ClassicGM();
        // FIXME: restore after testing
        int levelsGenerated = 0; // PlayerPrefs.GetInt("generated", 0);
        if (levelsGenerated == 0)
        {
            LevelGenerator.GenerateLevels();
            PlayerPrefs.SetInt("generated", 1);
        }
    }

    private void OnFullLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Maze")
        {
            LoadLevel(CurrentLevel);
        }
    }

    /// <summary>
    /// Loads the appropriate level from the file
    /// </summary>
    /// <param name="levelNumber">The level number to load</param>
    public void LoadLevel(int levelNumber)
    {
        //MazeState state = MazeState.LoadFrom($"/{levelNumber}.maze");
        string gameModeName = GameMode.GetType().Name;
        LevelStatus levelStatus = LevelIO.LoadLevel(new LevelSettings(gameModeName, new Dimensions(8, 8), levelNumber));
        Type GMType = Type.GetType(levelStatus.gameMode);
        GameMode gameMode = (GameMode)Activator.CreateInstance(GMType);
        LevelManager.Instance.Initialize(levelNumber, levelStatus, GameMode);
    }
}
