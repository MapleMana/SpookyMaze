using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : Menu<MainMenu>
{
    protected override void Awake()
    {
        base.Awake();
    }

    public void OnAboutPressed()
    {
        AboutMenu.Open();
    }

    public void OnModePressed(string modeName)
    {
        GameManager.Instance.CurrentSettings.isDaily = false;
        GameManager.Instance.CurrentSettings.gameMode = modeName;
        DimensionsMenu.Open();
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