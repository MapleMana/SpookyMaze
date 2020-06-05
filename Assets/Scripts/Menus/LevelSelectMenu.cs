using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectMenu : Menu<LevelSelectMenu>
{
    public GameObject ButtonsPanel;
    public Button ButtonTemplate;

    private List<Button> buttonList;

    private void Start()
    {
        LoadLevels();
    }

    /// <summary>
    /// Invoked when the game starts and loads level buttons to the Level Select screen
    /// </summary>
    public void LoadLevels()
    {
        buttonList = new List<Button>();
        int levelReached = PlayerPrefs.GetInt("levelReached", 1);

        for (int i = 1; i <= LevelGenerator.NUM_OF_LEVELS; i++)
        {
            Button newButton = Instantiate(ButtonTemplate);
            newButton.GetComponentInChildren<Text>().text = i.ToString();
            newButton.onClick.AddListener(OnLevelOptionClick(i));
            newButton.interactable = (i <= levelReached);
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
            GameManager.Instance.CurrentLevel = levelNumber;
            UIManager.Instance.StartGame();
        };
    }
}
