using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DimensionsMenu : Menu<DimensionsMenu>
{
    public GameObject ButtonsPanel;
    public Button ButtonTemplate;

    private List<Button> buttonList;

    private void Start()
    {
        LoadDimensions();
    }

    public void OnDimensionPressed()
    {
        LevelSelectMenu.Open();
    }

    /// <summary>
    /// Invoked when the game starts and loads dimensions buttons to the Dimension Select screen
    /// </summary>
    public void LoadDimensions()
    {
        buttonList = new List<Button>();

        for (int i = 1; i <= 3; i++)
        {
            Button newButton = Instantiate(ButtonTemplate);
            newButton.GetComponentInChildren<TMP_Text>().text = $"{i}x{i}";
            newButton.onClick.AddListener(OnLevelOptionClick(i));
            newButton.transform.SetParent(ButtonsPanel.transform, false);

            buttonList.Add(newButton);
        }
    }

    /// <summary>
    /// Executed when one of the level select buttons is pressed
    /// </summary>
    /// <param name="dimensionNumber">The dimensions to load</param>
    /// <returns></returns>
    public UnityEngine.Events.UnityAction OnLevelOptionClick(int dimensionNumber)
    {
        return () =>
        {
            LevelSelectMenu.Open();
        };
    }
}
