using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkBtn : MonoBehaviour
{
    public Image image;
    public Color imageColourMax;
    public Color imageColourMin;

    private bool _isBlinking = false;
    private bool _blinkToMin = true;
    private float _timeStartedLerping;
    private float _timeTakenDuringLerp = 0.8f;

    private void Start()
    {
        ToggleBlinking(true);
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
    }

    public void ToggleBlinking(bool toggle)
    {
        _isBlinking = toggle;
        _timeStartedLerping = Time.time;
        if (!toggle)
        {
            image.color = imageColourMax;
        }
    }
}
