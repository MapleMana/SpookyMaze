using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectMenu : Menu<LevelSelectMenu>
{
    public GameObject ButtonsPanel;
    public Button ButtonTemplate;
    public TMP_Text ModeName;

    private List<Button> buttonList;

    private void Start()
    {
        ModeName.text = GameManager.Instance.CurrentSettings.GetReadableGameMode();
        LoadLevels();
    }

    /// <summary>
    /// Invoked when the game starts and loads level buttons to the Level Select screen
    /// </summary>
    public void LoadLevels()
    {
        buttonList = new List<Button>();
        int levelReached = GetLevelProgress();

        List<int> possibleLevels = LevelIO.GetPossibleIds(GameManager.Instance.CurrentSettings);
        possibleLevels.Sort();

        foreach (int level in possibleLevels)
        {
            Button newButton = CreateLevelButton(levelReached, level);
            buttonList.Add(newButton);
        }
    }

    private Button CreateLevelButton(int levelReached, int level)
    {
        Button newButton = Instantiate(ButtonTemplate);
        newButton.GetComponentInChildren<Text>().text = level.ToString();
        newButton.onClick.AddListener(OnLevelOptionClick(level));
        newButton.interactable = (level <= levelReached);
        newButton.transform.SetParent(ButtonsPanel.transform, false);
        return newButton;
    }

    private static int GetLevelProgress()
    {
        LevelSettings currentLevelSettings = GameManager.Instance.CurrentSettings;
        string modeDimension = currentLevelSettings.ModeDimensions;
        return PlayerPrefs.GetInt(modeDimension, 1);
    }

    /// <summary>
    /// Executed when one of the level select buttons is pressed
    /// </summary>
    /// <param name="levelNumber">The level number to load</param>
    /// <returns></returns>
    public UnityAction OnLevelOptionClick(int levelNumber)
    {
        return () =>
        {
            GameManager.Instance.CurrentSettings.id = levelNumber;
            UIManager.Instance.StartGame();
        };
    }
}
