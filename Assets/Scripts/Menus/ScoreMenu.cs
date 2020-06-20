using TMPro;
using UnityEngine.UI;

public class ScoreMenu : Menu<ScoreMenu>
{
    public TextMeshProUGUI PlayerScore;

    /// <summary>
    /// Sets the text on the button for showing the next available action
    /// </summary>
    public void SetScoreText(int score)
    {
        PlayerScore.text = $"Current Score: {score}";
    }
}
