using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DailyMenu : Menu<DailyMenu>
{
    public void OnModePressed(string modeName)
    {
        GameManager.Instance.CurrentSettings.gameMode = modeName;
        DimensionsMenu.Open();
    }
}
