using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectMenu : MonoBehaviour
{
    public GameObject levelSizePanel;
    public Button levelSizeButtonTemplate;
    public GameObject levelSelectButtonsPanel;
    public Button levelSelectButtonTemplate;
    public TMP_Text ModeName;

    private List<Button> buttonList;
    private List<GameObject> panelList;
    private List<Button> levelButtonList;

    public void LoadDimensions()
    {
        ModeName.text = GameManager.Instance.CurrentSettings.GetReadableGameMode();

        buttonList = new List<Button>();
        panelList = new List<GameObject>();
        List<Dimensions> possibleDimensions = LevelIO.GetPossibleDimensions(GameManager.Instance.CurrentSettings);
        possibleDimensions.Sort(delegate (Dimensions d1, Dimensions d2)
        {
            return d1.Width.CompareTo(d2.Width);
        });

        foreach (Dimensions dimensions in possibleDimensions)
        {
            int width = dimensions.Width;
            int height = dimensions.Height;
            Button newButton = Instantiate(levelSizeButtonTemplate);
            newButton.GetComponentInChildren<Text>().text = dimensions.ToString();
            newButton.onClick.AddListener(OnDimensionsOptionClick(width, height, dimensions.ToString()));
            newButton.transform.SetParent(levelSizePanel.transform, false);
            buttonList.Add(newButton);

            GameObject newPanel = Instantiate(levelSelectButtonsPanel);
            newPanel.transform.SetParent(levelSizePanel.transform, false);
            newPanel.name = dimensions.ToString();
            panelList.Add(newPanel);

            GameManager.Instance.CurrentSettings.dimensions = new Dimensions(width, height);
            LoadLevels(newPanel);
            newPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Invoked when the game starts and loads level buttons to the Level Select screen
    /// </summary>
    public void LoadLevels(GameObject panel)
    {
        levelButtonList = new List<Button>();
        int levelReached = GetLevelProgress();

        List<int> possibleLevels = LevelIO.GetPossibleIds(GameManager.Instance.CurrentSettings);
        possibleLevels.Sort();

        foreach (int level in possibleLevels)
        {
            Button newButton = CreateLevelButton(levelReached, level, panel);
            levelButtonList.Add(newButton);
        }
    }

    private Button CreateLevelButton(int levelReached, int level, GameObject panel)
    {
        Button newButton = Instantiate(levelSelectButtonTemplate);
        newButton.GetComponentInChildren<Text>().text = level.ToString();
        newButton.onClick.AddListener(OnLevelOptionClick(level));
        newButton.interactable = (level <= levelReached);
        newButton.transform.SetParent(panel.transform, false);
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

    /// <summary>
    /// Executed when one of the level select buttons is pressed
    /// </summary>
    /// <param name="panelName">The name of the panel to open</param>
    /// <returns></returns>
    public UnityEngine.Events.UnityAction OnDimensionsOptionClick(int dimensionWidth, int dimensionHeight, string panelName)
    {
        return () =>
        {
            GameManager.Instance.CurrentSettings.dimensions = new Dimensions(dimensionWidth, dimensionHeight);
            foreach (GameObject panel in panelList)
            {
                panel.SetActive(panel.name == panelName);
            }
        };
    }

    public void ClearPanel()
    {
        foreach (Transform child in levelSizePanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
