using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private const Vector3 menuPosition = new Vector3(25, 200, 20);

    private static CameraManager _instance;

    public static CameraManager Instance { get => _instance; }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void FocusOn(Maze maze)
    {
        Vector3 mazeCenter = new Vector3(MazeCell.CELL_WIDTH * maze.Width / 2, 0, MazeCell.CELL_WIDTH * maze.Height / 2);
        transform.position = new Vector3(mazeCenter.x, transform.position.y, mazeCenter.z);
    }

    public void FocusOnMenu()
    {
        transform.position = menuPosition;
    }
}