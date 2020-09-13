using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyMenu : Menu<DailyMenu>
{
    public void OnModePressed(string modeName)
    {
        GameManager.Instance.CurrentSettings.gameMode = modeName;
        // TODO: start an ad
        HandleAdWatched();
        DailyLevelSelectMenu.Open();
    }

    private void HandleAdWatched()
    {
        int openedDailyLevels = PlayerPrefs.GetInt("OpenedDailyLevels");
        openedDailyLevels += 4;
        PlayerPrefs.SetInt("OpenedDailyLevels", openedDailyLevels);
    }
}
