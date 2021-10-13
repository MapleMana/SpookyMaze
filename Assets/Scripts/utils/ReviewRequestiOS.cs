using UnityEngine;

// Attached to MainMenu Panel
public class ReviewRequestiOS : MonoBehaviour
{
    private void OnEnable()
    {
    #if UNITY_IOS
        if (StatsManager.Instance != null && Application.platform == RuntimePlatform.IPhonePlayer)
        {
            int total = StatsManager.Instance.GetTotalGameModeCompletedLevels("Classic") +
            StatsManager.Instance.GetTotalGameModeCompletedLevels("Dungeon") +
            StatsManager.Instance.GetTotalGameModeCompletedLevels("Cursed House");
            int reviewRate = PlayerPrefs.GetInt("ReviewRate", 5);
            if (total > reviewRate)
            {
                UnityEngine.iOS.Device.RequestStoreReview();
                PlayerPrefs.SetInt("ReviewRate", reviewRate + 50);
                PlayerPrefs.Save();
            }
        }
    #endif
    }
}