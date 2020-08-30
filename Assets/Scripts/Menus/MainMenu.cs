using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : Menu<MainMenu>
{
    public TMP_Text Coins;

    protected override void Awake()
    {
        base.Awake();
        InstantiateScore();
    }

    private void InstantiateScore()
    {
        TMP_Text playerCoins = Instantiate(Coins, MainMenu.Instance.transform, false);
        playerCoins.text = $"Coins: {PlayerPrefs.GetInt("PlayersCoins", 0)}";
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

    public void OnDailyPressed()
    {
        GameManager.Instance.CurrentSettings.isDaily = true;
        DailyMenu.Open();
    }

    public override void OnBackPressed()
    {
        Application.Quit();
    }
}