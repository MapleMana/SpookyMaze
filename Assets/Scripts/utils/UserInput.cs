using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
// https://stackoverflow.com/questions/63219332/unity-define-ui-buttons-in-input-manager

/// <summary>
/// Detects input for different platforms. Methods to be called on Update.
/// </summary>
static class PlayerActionDetector
{
    static private Vector3 touchStart;
    static private Vector3 touchEnd;
    const double minSwipeDistance = 0.1;  //minimum distance for a swipe to be registered (fraction of screen height)

    public static MovableMovementCommand Detect()
    {
        if (Application.platform == RuntimePlatform.Android || 
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (PlayerPrefs.GetInt("isTouch") == 0) // 0 means button controls
            {
                return DetectButtons();
            }
            return DetectTouch();
        }
        return DetectButtons();
    }

    /// <summary>
    /// Detects swipes on mobile platforms
    /// </summary>
    /// <returns>Direction of movement</returns>
    public static MovableMovementCommand DetectTouch()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchStart = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended && (touchStart != Vector3.zero))
            {
                touchEnd = touch.position;

                if (Vector3.Distance(touchStart, touchEnd) > minSwipeDistance * Screen.height)
                {
                    // check which axis is more significant
                    if (Mathf.Abs(touchEnd.x - touchStart.x) > Mathf.Abs(touchEnd.y - touchStart.y))
                    {
                        return (touchEnd.x > touchStart.x) ? MovableMovementCommand.MoveRight : MovableMovementCommand.MoveLeft;
                    }
                    else if (Mathf.Abs(touchEnd.y - touchStart.y) > Mathf.Abs(touchEnd.x - touchStart.x))
                    {
                        return (touchEnd.y > touchStart.y) ? MovableMovementCommand.MoveUp : MovableMovementCommand.MoveDown;
                    }
                }
                /*else if (touchStart == touchEnd)
                {
                    return MovableMovementCommand.Stop;
                }*/
            }
        }        
        return null;
    }

    public static MovableMovementCommand DetectMobileButtons()
    {

        return null;
    }

    public static void ResetTouches()
    {
        touchStart = Vector3.zero;
        touchEnd = Vector3.zero;
    }

    /// <summary>
    /// Detects arrow key presses on desktop
    /// </summary>
    /// <returns>Direction of movement</returns>
    public static MovableMovementCommand DetectButtons()
    {
        if (CrossPlatformInputManager.GetButtonDown("up"))
        {
            return MovableMovementCommand.MoveUp;
        }
        if (CrossPlatformInputManager.GetButtonDown("down"))
        {
            return MovableMovementCommand.MoveDown;
        }
        if (CrossPlatformInputManager.GetButtonDown("left"))
        {
            return MovableMovementCommand.MoveLeft;
        }
        if (CrossPlatformInputManager.GetButtonDown("right"))
        {
            return MovableMovementCommand.MoveRight;
        }
        /*if (Input.GetKeyUp(KeyCode.Space))
        {
            return MovableMovementCommand.Stop;
        }*/
        return null;
    }
}
