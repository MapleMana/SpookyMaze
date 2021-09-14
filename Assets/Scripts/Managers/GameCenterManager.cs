using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://gamegorillaz.com/blog/game-center-setup-in-unity/

public class GameCenterManager : Singleton<GameCenterManager>
{
    public bool loginSuccessful;
    public GameObject gameCenterBtn;

    private const string _classicLeaderboardID = "com.MapleMana.SpookyMaze.ClassicLeaderboard";
    private const string _dungeonLeaderboardID = "com.MapleMana.SpookyMaze.DungeonLeaderboard";
    private const string _cursedHouseLeaderboardID  = "com.MapleMana.SpookyMaze.CursedHouseLeaderboard";

    private const string _classicStreakLeaderboardID = "com.MapleMana.SpookyMaze.ClassicStreakLeaderboard";
    private const string _dungeonStreakLeaderboardID = "com.MapleMana.SpookyMaze.DungeonStreakLeaderboard";
    private const string _cursedHouseStreakLeaderboardID = "com.MapleMana.SpookyMaze.CursedHouseStreakLeaderboard";

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

    public void PostScoreOnLeaderBoard(int myScore, string gameMode, bool isStreak)
    {
        string leaderboardID = "";
        switch (gameMode)
        {
            case "Classic":
                if (!isStreak)
                {
                    leaderboardID = _classicLeaderboardID;
                }
                else
                {
                    leaderboardID = _classicStreakLeaderboardID;
                }
                break;
            case "Dungeon":
                if (!isStreak)
                {
                    leaderboardID = _dungeonLeaderboardID;
                }
                else
                {
                    leaderboardID = _dungeonStreakLeaderboardID;
                }
                break;
            case "Cursed House":
                if (!isStreak)
                {
                    leaderboardID = _cursedHouseLeaderboardID;
                }
                else
                {
                    leaderboardID = _cursedHouseStreakLeaderboardID;
                }
                break;
            default:
                return;
        }

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
