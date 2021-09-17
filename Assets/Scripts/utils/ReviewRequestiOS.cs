using UnityEngine;

// Attached to MainMenu Panel
public class ReviewRequestiOS : MonoBehaviour
{
    private void OnEnable()
    {
        if (StatsManager.Instance != null && Application.platform == RuntimePlatform.IPhonePlayer)
        {
            int total = StatsManager.Instance.GetTotalGameModeCompletedLevels("Classic") +
            StatsManager.Instance.GetTotalGameModeCompletedLevels("Dungeon") +
            StatsManager.Instance.GetTotalGameModeCompletedLevels("Cursed House");
            if (total > 5)
            {
                UnityEngine.iOS.Device.RequestStoreReview();
            }
        }        
    }
}