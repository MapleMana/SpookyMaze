using UnityEngine;

public class PurchaseMenu : MonoBehaviour
{
    public void PurchasedCoins(int num)
    {
        int previousScore = PlayerPrefs.GetInt("PlayersCoins", 0);
        int newScore = previousScore + num;
        SoundManager.Instance.PlaySoundEffect(SoundEffect.CoinsEarned);
        PlayerPrefs.SetInt("PlayersCoins", newScore);
        PlayerPrefs.Save();
    }
}
