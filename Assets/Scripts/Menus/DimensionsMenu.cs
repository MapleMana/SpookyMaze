using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DimensionsMenu : Menu<DimensionsMenu>
{
    public GameObject ButtonsPanel;
    public Button ButtonTemplate;
    public TMP_Text ModeName;

    private List<Button> buttonList;

    private void Start()
    {
        ModeName.text = GameManager.Instance.CurrentSettings.gameMode;
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
        List<Dimensions> possibleDimensions = LevelIO.GetPossibleDimensions(GameManager.Instance.CurrentSettings);
        possibleDimensions.Sort(delegate (Dimensions d1, Dimensions d2)
        {
            return d1.Width.CompareTo(d2.Width);
        });

        foreach (Dimensions dimensions in possibleDimensions)
        {
            int width = dimensions.Width;
            int height = dimensions.Height;
            Button newButton = Instantiate(ButtonTemplate);
            newButton.GetComponentInChildren<TMP_Text>().text = dimensions.ToString();
            newButton.onClick.AddListener(OnDimensionsOptionClick(width, height));
            newButton.transform.SetParent(ButtonsPanel.transform, false);

            buttonList.Add(newButton);
        }
    }

    /// <summary>
    /// Executed when one of the level select buttons is pressed
    /// </summary>
    /// <param name="dimensionNumber">The dimensions to load</param>
    /// <returns></returns>
    public UnityEngine.Events.UnityAction OnDimensionsOptionClick(int dimensionWidth, int dimensionHeight)
    {
        return () =>
        {
            GameManager.Instance.CurrentSettings.dimensions = new Dimensions(dimensionWidth, dimensionHeight);
            LevelSelectMenu.Open();
        };
    }
}
