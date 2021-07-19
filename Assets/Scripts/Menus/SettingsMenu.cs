using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Language Buttons")]
    public Button ENBtn;
    public Button FRBtn;
    public Button DEBtn;
    public Button ITBtn;
    public Button ESBtn;

    private void OnEnable()
    {
        switch (PlayerPrefs.GetString("selected-locale", "en"))
        {
            case "en":
            default:
                ENBtn.Select();
                break;
            case "fr":
                FRBtn.Select();
                break;
            case "de":
                DEBtn.Select();
                break;
            case "it":
                ITBtn.Select();
                break;
            case "es":
                ENBtn.Select();
                break;
        }
    }
}
