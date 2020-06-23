using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : Menu<MainMenu>
{
    public TMP_Text Score;

    protected override void Awake()
    {
        base.Awake();
        InstantiateScore();
    }

    private void InstantiateScore()
    {
        TMP_Text playerScore = Instantiate(Score, MainMenu.Instance.transform, false);
        playerScore.text = $"Score: {PlayerPrefs.GetInt("PlayerScore", 0)}";
    }

    public void OnAboutPressed()
    {
        AboutMenu.Open();
    }

    public void OnModePressed(string modeName)
    {
        GameManager.Instance.CurrentSettings.gameMode = modeName;
        DimensionsMenu.Open();
    }

    public void OnSettingsPressed()
    {
        SettingsMenu.Open();
    }

    public void OnDimensionPressed()
    {
        LevelSelectMenu.Open();
    }

    public override void OnBackPressed()
    {
        Application.Quit();
    }
}