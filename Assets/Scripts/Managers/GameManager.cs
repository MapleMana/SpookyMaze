using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Advertisement.IosSupport.Samples;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public float replayMultiplier;
    public float reversedReplayMultiplier;
    public LevelSettings CurrentSettings { get; set; } = new LevelSettings();

    public GameObject loadingPanel;
    public Text loadingPanelText;
    public Slider loadingPanelSlider;
    public GameObject contextScreenManager;

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
        int levelsGenerated = PlayerPrefs.GetInt("Generated", 0); // 0 no levels, 1 all levels generated
        if (levelsGenerated == 0 || !Directory.Exists(Path.GetDirectoryName($"{Application.persistentDataPath}/Cursed House/8x8/E/1.maze")))
        {
            StartCoroutine(GenerateLevelsOverTime());
        }
    }

    IEnumerator GenerateLevelsOverTime()
    {
        loadingPanel.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        foreach(CombinedGM combined in LevelGenerator.gameModes)
        {
            LevelGenerator.GenerateLevels(combined);
            yield return new WaitForSeconds(0.1f);
            loadingPanelText.text += ".";
            loadingPanelSlider.value += 0.33f;
            yield return new WaitForSeconds(0.1f);
        }

        loadingPanel.SetActive(false);
        PlayerPrefs.SetInt("Generated", 1);
        yield return new WaitForSeconds(0.1f);
        contextScreenManager.GetComponent<ContextScreenManager>().CheckStatus();
        StopAllCoroutines();
    }

    private void OnFullLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Maze")
        {
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

    /// <summary>
    /// Called in purchase menu
    /// Adds coins to player's total after purchase
    /// </summary>
    public void PurchaseCoins(int amount)
    {
        int currentAmount = PlayerPrefs.GetInt("PlayersCoins", 0);
        int newAmount = currentAmount + amount;
        PlayerPrefs.SetInt("PlayersCoins", newAmount);
    }

    public bool IsLastLevel()
    {
        if (CurrentSettings.isDaily)
        {
            return CurrentSettings.id == LevelGenerator.NUM_OF_DAILY_LEVELS;
        }
        return CurrentSettings.id == LevelGenerator.NUM_OF_LEVELS;
    }
}