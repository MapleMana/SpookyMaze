using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
            return DetectMobile();
        }
        return DetectDesktop();
    }

    /// <summary>
    /// Detects swipes on mobile platforms
    /// </summary>
    /// <returns>Direction of movement</returns>
    public static MovableMovementCommand DetectMobile()
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
            }
        }        
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
    public static MovableMovementCommand DetectDesktop()
    {
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            return MovableMovementCommand.MoveUp;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            return MovableMovementCommand.MoveDown;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            return MovableMovementCommand.MoveLeft;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            return MovableMovementCommand.MoveRight;
        }
        return null;
    }
}
