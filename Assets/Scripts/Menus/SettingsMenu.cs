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
                UIManager.Instance.ChangeLocal(0);
                break;
            case "fr":
                UIManager.Instance.ChangeLocal(1);
                break;
            case "de":
                UIManager.Instance.ChangeLocal(2);
                break;
            case "it":
                UIManager.Instance.ChangeLocal(3);
                break;
            case "es":
                UIManager.Instance.ChangeLocal(4);
                break;
        }
    }
}
