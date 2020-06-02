using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : Menu<SettingsMenu>
{
    public void OnMainMenuButtonPressed()
    {
        MainMenu.Open();
    }
}
