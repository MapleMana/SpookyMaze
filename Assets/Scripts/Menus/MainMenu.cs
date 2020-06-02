using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Menu<MainMenu>
{
    public void OnAboutPressed()
    {
        AboutMenu.Open();
    }

    public void OnModePressed()
    {
        DimensionsMenu.Open();
    }

    public void OnSettingsPressed()
    {
        SettingsMenu.Open();
    }

    public override void OnBackPressed()
    {
        Application.Quit();
    }
}