using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectMenu : Menu<LevelSelectMenu>
{
    public GameObject ButtonsPanel;
    public Button ButtonTemplate;
    public TMP_Text ModeName;

    private List<Button> buttonList;

    private void Start()
    {
        ModeName.text = GameManager.Instance.GameModeName;
        LoadLevels();
    }

    /// <summary>
    /// Invoked when the game starts and loads level buttons to the Level Select screen
    /// </summary>
    public void LoadLevels()
    {
        buttonList = new List<Button>();
        int levelReached = PlayerPrefs.GetInt("levelReached", 1);

        List<int> possibleLevels = LevelIO.GetPossibleIds(GameManager.Instance.GameModeName, GameManager.Instance.Dimensions);
        possibleLevels.Sort();

        foreach (int level in possibleLevels)
        {
            Button newButton = Instantiate(ButtonTemplate);
            newButton.GetComponentInChildren<Text>().text = level.ToString();
            newButton.onClick.AddListener(OnLevelOptionClick(level));
            newButton.interactable = (level <= levelReached);
            newButton.transform.SetParent(ButtonsPanel.transform, false);

            buttonList.Add(newButton);
        }
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
            GameManager.Instance.LoadLevel(levelNumber);
            UIManager.Instance.StartGame();
        };
    }
}
