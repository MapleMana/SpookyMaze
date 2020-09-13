using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DailyLevelSelectMenu : Menu<DailyLevelSelectMenu>
{
    public TMP_Text ModeName;
    
    private void Start()
    {
        ModeName.text = GameManager.Instance.CurrentSettings.GetReadableGameMode();
    }
    public UnityAction OnLevelPressed(int levelNumber)
    {
        return () =>
        {
            GameManager.Instance.CurrentSettings.id = levelNumber;
            UIManager.Instance.StartGame();
        };
    }
}
