using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public float replayMultiplier;
    public float reversedReplayMultiplier;
    public LevelSettings CurrentSettings { get; set; } = new LevelSettings();
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnFullLoad;
        //ScoreMenu.Open();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnFullLoad;
    }

    private void Start()
    {
        Maze.Initialize();
        int levelsGenerated = PlayerPrefs.GetInt("generated", 0);
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
            MobileAds.Initialize(initStatus => { });
            LoadLevel();
        }
    }

    /// <summary>
    /// Loads the appropriate level from the file
    /// </summary>
    public void LoadLevel()
    {
        LevelData levelData = LevelIO.LoadLevel(CurrentSettings);
        LevelManager.Instance.Initialize(levelData);
    }
}