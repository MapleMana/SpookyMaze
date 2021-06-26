using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : Singleton<StatsManager>
{
    public List<String> statsList = new List<string>();

    public StatsMenu statsMenu;

    private void Start()
    {
        GameManager.Instance.CurrentSettings.gameMode = "Classic";
        List<Dimensions> possibleDimensions = LevelIO.GetPossibleDimensions(GameManager.Instance.CurrentSettings);
        possibleDimensions.Sort(delegate (Dimensions d1, Dimensions d2)
        {
            return d1.Width.CompareTo(d2.Width);
        });
        foreach (Dimensions dimensions in possibleDimensions)
        {
            statsList.Add(dimensions.ToString());
        }
        statsMenu.UpdateStatsText();
    }

    public void AddCompletedLevel(string gameMode, string dimensions)
    {
        PlayerPrefs.SetInt($"{gameMode}{dimensions}", PlayerPrefs.GetInt($"{gameMode}{dimensions}", 0) + 1);
        PlayerPrefs.Save();
        statsMenu.UpdateStatsText();
    }
}
