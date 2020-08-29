using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyLevelSelectMenu : Menu<DailyLevelSelectMenu>
{
    public UnityEngine.Events.UnityAction OnLevelPressed(int levelNumber)
    {
        return () =>
        {
            GameManager.Instance.CurrentSettings.id = levelNumber;
            UIManager.Instance.StartGame();
        };
    }
}
