using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyMenu : Menu<DailyMenu>
{
    InterstitialAd interstitial;

    public void OnModePressed(string modeName)
    {
        GameManager.Instance.CurrentSettings.gameMode = modeName;
        //StartAd();
        HandleAdWatched();
    }

    private void StartAd()
    {
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
        AdRequest request = new AdRequest.Builder().Build();
        interstitial = new InterstitialAd(adUnitId);
        interstitial.LoadAd(request);
        if (interstitial.IsLoaded())
        {
            interstitial.Show();
            //interstitial.OnAdClosed += HandleAdWatched;
        }
    }

    //private void HandleAdWatched(object sender, EventArgs args)
    private void HandleAdWatched()
    {
        int openedDailyLevels = PlayerPrefs.GetInt("OpenedDailyLevels");
        openedDailyLevels += 4;
        PlayerPrefs.SetInt("OpenedDailyLevels", openedDailyLevels);
        DailyLevelSelectMenu.Open();
        interstitial.Destroy();
    }
}
