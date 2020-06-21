using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnReplayMenu : Menu<OnReplayMenu>
{    
    /// <summary>
    /// Skips replay of player's movements and loads the next level
    /// </summary>
    public void SkipReplay()
    {
        LevelManager.Instance.StopAllCoroutines();
        LevelManager.Instance.Clear();
        GameManager.Instance.LoadLevel();
    }
}
