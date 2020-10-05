using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;
using UnityEngine.UI;


public class DailyLevelSelectMenu : MonoBehaviour
{
    public GameObject ClassicPanel;
    public GameObject DungeonPanel;
    public GameObject CursedHousePanel;
    public Button ButtonTemplate;

    private List<Button> classicButtonList = new List<Button>();
    private List<Button> dungeonButtonList = new List<Button>();
    private List<Button> cursedHouseButtonList = new List<Button>();

    private string modeToUnlock;

    private void Start()
    {
        LoadLevels(ClassicPanel, classicButtonList, "Classic");
        LoadLevels(DungeonPanel, dungeonButtonList, "Dungeon");
        LoadLevels(CursedHousePanel, cursedHouseButtonList, "Cursed House");
    }

    public void LoadLevels(GameObject panel, List<Button> list, string modeName)
    {
        GameManager.Instance.CurrentSettings.gameMode = modeName;
        int openedLevels = PlayerPrefs.GetInt($"OpenedDailyLevels{GameManager.Instance.CurrentSettings.gameMode}");

        int possibleLevels = LevelGenerator.NUM_OF_DAILY_LEVELS;

        for (int i = 1; i <= possibleLevels; i++)
        {
            Button newButton = CreateLevelButton(panel, openedLevels, i);
            list.Add(newButton);
        }
    }

    public UnityAction OnLevelOptionClick(int levelNumber)
    {
        return () =>
        {
            GameManager.Instance.CurrentSettings.id = levelNumber;
            GameManager.Instance.CurrentSettings.isDaily = true;
            UIManager.Instance.StartGame();
        };
    }

    private Button CreateLevelButton(GameObject panel, int openedLevels, int level)
    {
        Button newButton = Instantiate(ButtonTemplate);
        newButton.GetComponentInChildren<Text>().text = level.ToString();
        newButton.onClick.AddListener(OnLevelOptionClick(level));
        newButton.interactable = false;
        newButton.transform.SetParent(panel.transform, false);
        return newButton;
    }

    public void UnlockLevels(string modeName)
    {
        Advertisement.Show();
        modeToUnlock = modeName;
    }

    public void HandleAdWatched()
    {
        int openedDailyLevels = PlayerPrefs.GetInt($"OpenedDailyLevels{GameManager.Instance.CurrentSettings.gameMode}");
        openedDailyLevels += 4;
        PlayerPrefs.SetInt($"OpenedDailyLevels{GameManager.Instance.CurrentSettings.gameMode}", openedDailyLevels);
        switch (modeToUnlock)
        {
            case "Classic":
                foreach (Button button in classicButtonList)
                {
                    button.interactable = true;
                }
                break;
            case "Dungeon":
                foreach (Button button in dungeonButtonList)
                {
                    button.interactable = true;
                }
                break;
            case "Cursed House":
                foreach (Button button in cursedHouseButtonList)
                {
                    button.interactable = true;
                }
                break;
            default:
                Debug.Log("buttons didn't unlock :(");
                break;
        }
    }

    private void ClearButtonsPanel(GameObject panel)
    {
        foreach (Transform child in panel.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
