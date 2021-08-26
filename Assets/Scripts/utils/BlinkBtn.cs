using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkBtn : MonoBehaviour
{
    public Image image;
    public Color imageColourMax;
    public Color imageColourMin;

    public Text timerText;
    public GameObject timerPanel;
    public Button watchAdBtn;

    private bool _isBlinking = false;
    private bool _blinkToMin = true;
    private float _timeStartedLerping;
    private float _timeTakenDuringLerp = 0.8f;

    private const int WAIT_TIME = 3600;

    private void Start()
    {
        CheckAdBtn();
    }

    void Update()
    {
        if (_isBlinking)
        {
            if (_blinkToMin)
            {
                float timeSinceStarted = Time.time - _timeStartedLerping;
                float percentComplete = timeSinceStarted / _timeTakenDuringLerp;
                image.color = Color.Lerp(imageColourMax, imageColourMin, percentComplete);
                if (percentComplete >= 1.0)
                {
                    _blinkToMin = false;
                    _timeStartedLerping = Time.time;
                    image.color = imageColourMin;
                }
            }
            else
            {
                float timeSinceStarted = Time.time - _timeStartedLerping;
                float percentComplete = timeSinceStarted / _timeTakenDuringLerp;
                image.color = Color.Lerp(imageColourMin, imageColourMax, percentComplete);
                if (percentComplete >= 1.0)
                {
                    _blinkToMin = true;
                    _timeStartedLerping = Time.time;
                    image.color = imageColourMax;
                }
            }
        }
        else
        {
            int last = PlayerPrefs.GetInt("TimeAdPlayed", 0);
            int current = (int)DateTimeOffset.Now.ToUnixTimeSeconds();

            float timeRemaining = WAIT_TIME - (current - last);

            float minutes = Mathf.FloorToInt(timeRemaining / 60);
            float seconds = Mathf.FloorToInt(timeRemaining % 60);

            if (current - last >= WAIT_TIME)
            {
                ToggleBlinking(true);
            }

            if (timerPanel != null & timerText != null)
            {
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
        }
    }

    public void CheckAdBtn()
    {
        int last = PlayerPrefs.GetInt("TimeAdPlayed", 0);
        int current = (int)DateTimeOffset.Now.ToUnixTimeSeconds();

        if (current - last >= WAIT_TIME)
        {
            ToggleBlinking(true);
        }
        else
        {
            ToggleBlinking(false);
        }
    }

    public void ToggleBlinking(bool toggle)
    {
        _isBlinking = toggle;
        _timeStartedLerping = Time.time;
        if (!toggle)
        {
            image.color = imageColourMax;
        }
        if (timerPanel != null && watchAdBtn != null)
        {
            timerPanel.SetActive(!toggle);
            watchAdBtn.interactable = toggle;
        }
    }
}
