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
    public GameObject classicPanel;
    public Button classicUnlockBtn;
    public GameObject classicUnlockImage;
    public GameObject dungeonPanel;
    public Button dungeonUnlockBtn;
    public GameObject dungeonUnlockImage;
    public GameObject cursedHousePanel;
    public Button cursedHouseUnlockBtn;
    public GameObject cursedHouseUnlockImage;
    public Button ButtonTemplate;

    private List<Button> classicButtonList = new List<Button>();
    private List<Button> dungeonButtonList = new List<Button>();
    private List<Button> cursedHouseButtonList = new List<Button>();
    private Color orange = new Color(248f / 255f, 148f / 255f, 6f / 255f);

    private string modeToUnlock;

    public void LoadDailyMenu()
    {
        LoadLevels(classicPanel, classicUnlockBtn, classicUnlockImage, classicButtonList, "Classic");
        LoadLevels(dungeonPanel, dungeonUnlockBtn, dungeonUnlockImage, dungeonButtonList, "Dungeon");
        LoadLevels(cursedHousePanel, cursedHouseUnlockBtn, cursedHouseUnlockImage, cursedHouseButtonList, "Cursed House");
    }

    private void LoadLevels(GameObject panel, Button btn, GameObject img, List<Button> list, string modeName)
    {
        GameManager.Instance.CurrentSettings.gameMode = modeName;
        int openedLevels = PlayerPrefs.GetInt(modeName, 0);
        int possibleLevels = LevelGenerator.NUM_OF_DAILY_LEVELS;

        if (openedLevels > 0)
        {
            btn.interactable = false;
            img.SetActive(false);
        }

        for (int i = 1; i <= possibleLevels; i++)
        {
            GameManager.Instance.CurrentSettings.id = i;
            GameManager.Instance.CurrentSettings.dimensions = LevelIO.GetDailyDimension(GameManager.Instance.CurrentSettings)[0];
            string levelDim = LevelIO.GetDailyDimension(GameManager.Instance.CurrentSettings)[0].ToString();
            Button newButton = CreateLevelButton(panel, modeName, openedLevels, i, levelDim);
            list.Add(newButton);
        }
    }

    public UnityAction OnLevelOptionClick(string modeName, int levelNumber)
    {
        return () =>
        {
            GameManager.Instance.CurrentSettings.gameMode = modeName;
            GameManager.Instance.CurrentSettings.id = levelNumber;
            GameManager.Instance.CurrentSettings.isDaily = true;
            GameManager.Instance.CurrentSettings.dimensions = LevelIO.GetDailyDimension(GameManager.Instance.CurrentSettings)[0];
            ClearPanels();
            UIManager.Instance.StartGame();
        };
    }

    private Button CreateLevelButton(GameObject panel, string modeName, int openedLevels, int level, string dim)
    {
        Button newButton = Instantiate(ButtonTemplate);
        newButton.GetComponentInChildren<Text>().text = dim;
        newButton.GetComponentInChildren<Text>().fontSize = 100;
        newButton.onClick.AddListener(OnLevelOptionClick(modeName, level));
        if (openedLevels > 0)
        {
            newButton.interactable = true;
            newButton.transform.SetParent(panel.transform, false);
        }
        else
        {
            newButton.interactable = false;
            newButton.transform.SetParent(panel.transform, false);
        }
        
        if (GetLevelCompete(level))
        {
            newButton.GetComponent<Image>().color = orange;
        }
        return newButton;
    }

    private static bool GetLevelCompete(int level)
    {
        GameManager.Instance.CurrentSettings.id = level;
        GameManager.Instance.CurrentSettings.isDaily = true;
        LevelSettings currentLevelSettings = GameManager.Instance.CurrentSettings;
        return LevelIO.LoadLevel(currentLevelSettings).complete;
    }

    public void UnlockLevels(string modeName)
    {
        Advertisement.Show();
        DailyAdHandler.dailyUnlockAd = true;
        modeToUnlock = modeName;
    }

    public void HandleAdWatched()
    {
        ClearPanels();
        PlayerPrefs.SetInt(modeToUnlock, 1);
        PlayerPrefs.Save();
        LoadDailyMenu();
    }

    public void ResetButtons()
    {
        classicUnlockBtn.interactable = true;
        classicUnlockImage.SetActive(true);
        dungeonUnlockBtn.interactable = true;
        dungeonUnlockImage.SetActive(true);
        cursedHouseUnlockBtn.interactable = true;
        cursedHouseUnlockImage.SetActive(true);
    }

    public void ClearPanels()
    {
        ClearButtonsPanel(classicPanel);
        ClearButtonsPanel(dungeonPanel);
        ClearButtonsPanel(cursedHousePanel);
    }

    private void ClearButtonsPanel(GameObject panel)
    {
        foreach (Transform child in panel.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
