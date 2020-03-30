using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player _instance;

    private Vector2Int _mazePosition;

    public static Player Instance { get => _instance; set => _instance = value; }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void PlaceOnMaze()
    {
        _mazePosition = Maze.Instance.start;
        SyncRealPosition();
    }

    static private Vector2Int GetInput()
    {
        if (Input.GetAxis("Horizontal") > Mathf.Epsilon)
        {
            return Vector2Int.right;
        }
        else if (Input.GetAxis("Horizontal") < -Mathf.Epsilon)
        {
            return Vector2Int.left;
        }
        else if (Input.GetAxis("Vertical") > Mathf.Epsilon)
        {
            return Vector2Int.up;
        }
        else if (Input.GetAxis("Vertical") < -Mathf.Epsilon)
        {
            return Vector2Int.down;
        }
        return Vector2Int.zero;
    }

    void Update()
    {
        Vector2Int movement = InputDetector.DetectDesktop();
        if (movement != Vector2Int.zero)
        {
            if (!Maze.Instance.Grid[_mazePosition].WallExists(movement))
            {
                _mazePosition += movement;
                SyncRealPosition();
            }
        }
    }

    /// <summary>
    /// Synchronizes maze position and physical player position
    /// </summary>
    void SyncRealPosition()
    {
        MazeCell currentCell = Maze.Instance.Grid[_mazePosition];
        transform.position = new Vector3(currentCell.cellCenter.x, transform.position.y, currentCell.cellCenter.y);
    }
}

/// <summary>
/// Detects input for different platforms. Methods to be called on Update.
/// </summary>
static class InputDetector
{
    static private Vector3 touchStart;
    const double minSwipeDistance = 0.1;  //minimum distance for a swipe to be registered (fraction of screen height)

    /// <summary>
    /// Detects swipes on mobile platforms
    /// </summary>
    /// <returns>Direction of movement</returns>
    public static Vector2Int DetectMobile()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchStart = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                Vector3 touchEnd = touch.position;

                if (Vector3.Distance(touchStart, touchEnd) > minSwipeDistance * Screen.height)
                {
                    // check which axis is more significant
                    if (Mathf.Abs(touchEnd.x - touchStart.x) > Mathf.Abs(touchEnd.y - touchStart.y))
                    {
                        return (touchEnd.x > touchStart.x) ? Vector2Int.right : Vector2Int.left;
                    }
                    else
                    {
                        return (touchEnd.y > touchStart.y) ? Vector2Int.up : Vector2Int.down;
                    }
                }
            }
        }
        return Vector2Int.zero;
    }

    /// <summary>
    /// Detects arrow key presses on desktop
    /// </summary>
    /// <returns>Direction of movement</returns>
    public static Vector2Int DetectDesktop()
    {
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            return Vector2Int.up;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            return Vector2Int.down;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            return Vector2Int.left;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            return Vector2Int.right;
        }
        return Vector2Int.zero;
    }
}