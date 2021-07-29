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

    [Header("Music & SFX")]
    public Slider musicVol;
    public Slider sfxVol;

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
        musicVol.value = PlayerPrefs.GetFloat("musicVol", 0.5f);
        sfxVol.value = PlayerPrefs.GetFloat("sfxVol", 0.5f);
    }

    public void SetMusicVol()
    {
        PlayerPrefs.SetFloat("musicVol", musicVol.value);
        PlayerPrefs.Save();
    }

    public void SetSFXVol()
    {
        PlayerPrefs.SetFloat("sfxVol", sfxVol.value);
        PlayerPrefs.Save();
    }
}
