using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class DailyLevelSelectMenu : Menu<DailyLevelSelectMenu>
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

    public void LoadLevels()
    {
        buttonList = new List<Button>();
        int openedLevels = PlayerPrefs.GetInt($"OpenedDailyLevels{GameManager.Instance.CurrentSettings.gameMode}");

        int possibleLevels = LevelGenerator.NUM_OF_DAILY_LEVELS;

        for (int i = 1; i <= possibleLevels; i++)
        {
            Button newButton = CreateLevelButton(openedLevels, i);
            buttonList.Add(newButton);
        }
    }

    public UnityAction OnLevelOptionClick(int levelNumber)
    {
        return () =>
        {
            GameManager.Instance.CurrentSettings.id = levelNumber;
            UIManager.Instance.StartGame();
        };
    }

    private Button CreateLevelButton(int openedLevels, int level)
    {
        Button newButton = Instantiate(ButtonTemplate);
        newButton.GetComponentInChildren<Text>().text = level.ToString();
        newButton.onClick.AddListener(OnLevelOptionClick(level));
        newButton.interactable = (level <= openedLevels);
        newButton.transform.SetParent(ButtonsPanel.transform, false);
        return newButton;
    }

    public void UnlockLevels()
    {
        HandleAdWatched();
    }

    //private void HandleAdWatched(object sender, EventArgs args)
    private void HandleAdWatched()
    {
        int openedDailyLevels = PlayerPrefs.GetInt($"OpenedDailyLevels{GameManager.Instance.CurrentSettings.gameMode}");
        openedDailyLevels += 4;
        PlayerPrefs.SetInt($"OpenedDailyLevels{GameManager.Instance.CurrentSettings.gameMode}", openedDailyLevels);
        ClearButtonsPanel();
        LoadLevels();
        //interstitial.Destroy();
    }

    private void ClearButtonsPanel()
    {
        foreach (Transform child in ButtonsPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
