using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class DailyAdHandler : MonoBehaviour, IUnityAdsListener
{
    private static string gameId = "0";
    public static string placementId = "daily_unlock";
    public static bool testMode = true;
    public DailyLevelSelectMenu DailyLevelSelectMenu;
    public static bool dailyUnlockAd;

    public List<GameObject> purchaseBtns = new List<GameObject>();

    void Start()
    {
#if UNITY_EDITOR
        gameId = "";
#elif UNITY_ANDROID
        gameId = "";
#elif UNITY_IOS
        gameId = "";
#endif
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, testMode);
    }

    public void OnUnityAdsDidError(string message)
    {
        Debug.Log("Daily Ad failed: " + message);
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        switch (showResult)
        {
            case ShowResult.Failed:
            case ShowResult.Skipped:
            default:
                break;
            case ShowResult.Finished:
                if (dailyUnlockAd)
                {
                    DailyLevelSelectMenu.HandleAdWatched();
                }
                else
                {
                    // earn 10 coins
                    int previousScore = PlayerPrefs.GetInt("PlayersCoins", 0);
                    int newScore = previousScore + 10;
                    PlayerPrefs.SetInt("PlayersCoins", newScore);
                    PlayerPrefs.SetInt("TimeAdPlayed", (int)DateTimeOffset.Now.ToUnixTimeSeconds());
                    PlayerPrefs.Save();
                    foreach(GameObject btn in purchaseBtns)
                    {
                        btn.GetComponent<BlinkBtn>().CheckAdBtn();
                    }
                }                
                break;
        }
    }

    public void OnUnityAdsDidStart(string placementId)
    {}

    public void OnUnityAdsReady(string placementId)
    {}
}
