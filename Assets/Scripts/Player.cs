using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player _instance;

    private Vector2Int _mazePosition;
    private Vector2Int _movement;

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
        if (_movement == Vector2Int.zero)
        {
            _movement = GetInput();
            if (_movement != Vector2Int.zero)
            {
                if (!Maze.Instance.Grid[_mazePosition].WallExists(_movement))
                {
                    _mazePosition += _movement;
                }
            }
        }

        MazeCell currentCell = Maze.Instance.Grid[_mazePosition];
        transform.position = new Vector3(currentCell.cellCenter.x, transform.position.y, currentCell.cellCenter.y);

        _movement = GetInput();
    }
}

class InputDetector : MonoBehaviour
{
    static private Vector3 touchStart;
    static private float minSwipeDistance;  //minimum distance for a swipe to be registered

    void Start()
    {
        minSwipeDistance = Screen.height * 15 / 100; //dragDistance is 15% height of the screen
    }

    static Vector2Int DetectMobile()
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

                if (Vector3.Distance(touchStart, touchEnd) > minSwipeDistance)
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
        return null;
    }
}