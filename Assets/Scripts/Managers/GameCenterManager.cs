using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://gamegorillaz.com/blog/game-center-setup-in-unity/

public class GameCenterManager : Singleton<GameCenterManager>
{
    public bool loginSuccessful;
    public GameObject gameCenterBtn;

    string leaderboardID = "WRITE YOUR LEADERBOARD ID HERE";

    void Start()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            AuthenticateUser();
            gameCenterBtn.SetActive(true);
        }        
        else
        {
            gameCenterBtn.SetActive(false);
        }
    }    

    void AuthenticateUser()
    {
        Social.localUser.Authenticate((bool success) => {
            if (success)
            {
                loginSuccessful = true;
                Debug.Log("success");
            }
            else
            {
                Debug.Log("unsuccessful");
            }
            // handle success or failure
        });
    }

    public void PostScoreOnLeaderBoard(int myScore)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (loginSuccessful)
            {
                Social.ReportScore(myScore, leaderboardID, (bool success) => {
                    if (success)
                    {
                        Debug.Log("Successfully uploaded");
                    }
                    // handle success or failure
                });
            }
            else
            {
                Social.localUser.Authenticate((bool success) => {
                    if (success)
                    {
                        loginSuccessful = true;
                        Social.ReportScore(myScore, leaderboardID, (bool successful) => {
                            // handle success or failure

                        });
                    }
                    else
                    {
                        Debug.Log("unsuccessful");
                    }
                    // handle success or failure
                });
            }
        }
    }
    
    public void ShowLeaderboard()
    {
        Social.ShowLeaderboardUI();
    }
}
