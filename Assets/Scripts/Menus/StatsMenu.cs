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

    private void OnEnable()
    {
        UpdateStatsText();
    }

    public void UpdateStatsText()
    {
        int total = 0;
        classicText.text = "\nCLASSIC\n\n";
        foreach(string dim in StatsManager.Instance.statsList)
        {
            int value = PlayerPrefs.GetInt($"Classic{dim}", 0);
            classicText.text += $"{dim} : {value}\n";
            total += value;
        }

        dungeonText.text = "\nDUNGEON\n\n";
        foreach (string dim in StatsManager.Instance.statsList)
        {
            int value = PlayerPrefs.GetInt($"Dungeon{dim}", 0);
            dungeonText.text += $"{dim} : {value}\n";
            total += value;
        }

        cursedHouseText.text = "CURSED HOUSE\n\n";
        foreach (string dim in StatsManager.Instance.statsList)
        {
            int value = PlayerPrefs.GetInt($"Cursed House{dim}", 0);
            cursedHouseText.text += $"{dim} : {value}\n";
            total += value;
        }

        totalText.text = $"TOTAL LEVELS COMPLETED: {total}";
        statsBtnText.text = $"{total}";
        statsEndGameBtnText.text = $"{total}";
    }
}
