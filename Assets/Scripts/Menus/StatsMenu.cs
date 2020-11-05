using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsMenu : MonoBehaviour
{
    public Text classicText;
    public Text dungeonText;
    public Text cursedHouseText;
    public Text totalText;
    public Text statsBtnText;
    public Text statsEndGameBtnText;

    public void UpdateStatsText()
    {
        int total = 0;
        classicText.text = "\nCLASSIC\n\n";
        foreach(KeyValuePair<string, int> kvp in StatsManager.Instance.classicStats)
        {
            classicText.text += $"{kvp.Key} : {kvp.Value}\n";
            total += kvp.Value;
        }

        dungeonText.text = "\nDUNGEON\n\n";
        foreach (KeyValuePair<string, int> kvp in StatsManager.Instance.dungeonStats)
        {
            dungeonText.text += $"{kvp.Key} : {kvp.Value}\n";
            total += kvp.Value;
        }

        cursedHouseText.text = "CURSED HOUSE\n\n";
        foreach (KeyValuePair<string, int> kvp in StatsManager.Instance.cursedHouseStats)
        {
            cursedHouseText.text += $"{kvp.Key} : {kvp.Value}\n";
            total += kvp.Value;
        }

        totalText.text = $"TOTAL LEVELS COMPLETED: {total}";
        statsBtnText.text = $"{total}";
        statsEndGameBtnText.text = $"{total}";
    }

    public void OpenStatsMenu()
    {
        UpdateStatsText();
    }
}
