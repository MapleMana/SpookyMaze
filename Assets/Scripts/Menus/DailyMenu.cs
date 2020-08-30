using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyMenu : Menu<DailyMenu>
{
    public int levelsToUnlock = 4;

    public void OnModePressed(string modeName)
    {
        GameManager.Instance.CurrentSettings.gameMode = modeName;
        // TODO: start an ad
        HandleAdWatched();
        DailyLevelSelectMenu.Open();
    }

    private void HandleAdWatched()
    {
        for (int i = 0; i < levelsToUnlock; i++)
        {
            LevelGenerator.GenerateDailyLevel(i);
        }
    }
}
