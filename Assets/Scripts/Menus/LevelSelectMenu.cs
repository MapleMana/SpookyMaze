﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    private Color orange = new Color(248f/255f, 148f/255f, 6f/255f);
    private Slider currentSlider;

    private const int COST_PER_PACK = 0; //200;

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
            GameManager.Instance.CurrentSettings.dimensions = new Dimensions(width, height);

            List<string> possiblePacks = LevelIO.GetPossiblePackIds(GameManager.Instance.CurrentSettings);

            foreach(string pack in possiblePacks)
            {
                Button newButton = Instantiate(levelSizeButtonTemplate);
                newButton.GetComponentInChildren<Text>().text = dimensions.ToString() + pack;
                newButton.onClick.AddListener(OnDimensionsOptionClick(width, height, pack, dimensions.ToString() + pack));
                newButton.transform.SetParent(levelSizePanel.transform, false);                
                buttonList.Add(newButton);
                currentSlider = newButton.transform.GetChild(0).GetComponent<Slider>();

                GameObject newPanel = Instantiate(levelSelectButtonsPanel);
                newPanel.transform.SetParent(levelSizePanel.transform, false);
                newPanel.name = dimensions.ToString() + pack;
                panelList.Add(newPanel);

                GameObject subNewPanel = newPanel.transform.GetChild(0).gameObject;

                GameManager.Instance.CurrentSettings.packId = pack;
                if (GetLevelUnlocked(1))
                {
                    newButton.GetComponentsInChildren<Image>()[2].gameObject.SetActive(false);
                    newPanel.transform.GetChild(1).gameObject.SetActive(false);                    
                }
                else
                {
                    Button purchaseButton = newPanel.transform.GetChild(1).GetChild(0).GetComponent<Button>();
                    purchaseButton.onClick.AddListener(OnPurchasePackOptionClick(width, height, pack, newPanel, newButton));
                }
                LoadLevels(subNewPanel);
                newPanel.SetActive(false);
            }            
        }
    }

    /// <summary>
    /// Invoked when the game starts and loads level buttons to the Level Select screen
    /// </summary>
    public void LoadLevels(GameObject panel)
    {
        levelButtonList = new List<Button>();
        List<int> possibleLevels = LevelIO.GetPossibleIds(GameManager.Instance.CurrentSettings);
        possibleLevels.Sort();

        foreach (int level in possibleLevels)
        {
            Button newButton = CreateLevelButton(level, panel);
            levelButtonList.Add(newButton);
        }
    }

    private Button CreateLevelButton(int level, GameObject panel)
    {
        Button newButton = Instantiate(levelSelectButtonTemplate);
        newButton.GetComponentInChildren<Text>().text = level.ToString();
        if (GetLevelCompete(level))
        {
            currentSlider.value += 0.05f;
            newButton.GetComponent<Image>().color = orange;
        }
        newButton.onClick.AddListener(OnLevelOptionClick(level));
        newButton.transform.SetParent(panel.transform, false);
        return newButton;
    }

    private static bool GetLevelUnlocked(int level)
    {
        GameManager.Instance.CurrentSettings.id = level;
        LevelSettings currentLevelSettings = GameManager.Instance.CurrentSettings;
        return LevelIO.LoadLevel(currentLevelSettings).unlocked;
    }

    private static bool GetLevelCompete(int level)
    {        
        GameManager.Instance.CurrentSettings.id = level;
        LevelSettings currentLevelSettings = GameManager.Instance.CurrentSettings;
        return LevelIO.LoadLevel(currentLevelSettings).complete;        
    }

    /// <summary>
    /// Executed when one of the level select buttons is pressed
    /// </summary>
    /// <param name="levelNumber">The level number to load</param>
    /// <returns></returns>
    public UnityEngine.Events.UnityAction OnLevelOptionClick(int levelNumber)
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
    public UnityEngine.Events.UnityAction OnDimensionsOptionClick(int dimensionWidth, int dimensionHeight, string packId, string panelName)
    {
        return () =>
        {
            GameManager.Instance.CurrentSettings.dimensions = new Dimensions(dimensionWidth, dimensionHeight);
            GameManager.Instance.CurrentSettings.packId = packId;
            foreach (GameObject panel in panelList)
            {
                panel.SetActive(panel.name == panelName);
            }
        };
    }

    /// <summary>
    /// Executed when one of the purchase level pack buttons is pressed
    /// unlocks pack if player has at least 200 coins
    /// </summary>
    /// <returns></returns>
    public UnityEngine.Events.UnityAction OnPurchasePackOptionClick(int dimensionWidth, int dimensionHeight, string packId, GameObject panel, Button levelPackButton)
    {
        return () =>
        {
            int currentAmount = PlayerPrefs.GetInt("PlayersCoins", 0);
            if (currentAmount >= COST_PER_PACK)
            {
                PlayerPrefs.SetInt("PlayerCoins", currentAmount - COST_PER_PACK);
                GameManager.Instance.CurrentSettings.dimensions = new Dimensions(dimensionWidth, dimensionHeight);
                GameManager.Instance.CurrentSettings.packId = packId;
                LevelSettings currentLevelSettings = GameManager.Instance.CurrentSettings;

                List<int> possibleLevels = LevelIO.GetPossibleIds(GameManager.Instance.CurrentSettings);
                possibleLevels.Sort();
                foreach (int level in possibleLevels)
                {
                    currentLevelSettings.id = level;
                    LevelData currentLevelData = LevelIO.LoadLevel(currentLevelSettings);
                    currentLevelData.unlocked = true;
                    LevelIO.SaveLevel(currentLevelSettings, currentLevelData);
                }
                panel.transform.GetChild(1).gameObject.SetActive(false);
                levelPackButton.GetComponentsInChildren<Image>()[2].gameObject.SetActive(false);
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
