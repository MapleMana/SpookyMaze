using UnityEngine;

public class PurchaseMenu : MonoBehaviour
{
    public void PurchasedCoins(int num)
    {
        int previousScore = PlayerPrefs.GetInt("PlayersCoins", 0);
        int newScore = previousScore + num;
        PlayerPrefs.SetInt("PlayersCoins", newScore);
        PlayerPrefs.Save();
    }
}
