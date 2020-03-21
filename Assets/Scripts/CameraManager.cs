using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
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
        Vector3 mazeCenter = new Vector3(Maze.CELL_WIDTH * maze.Width / 2, 0, Maze.CELL_WIDTH * maze.Height / 2);
        transform.position = new Vector3(mazeCenter.x, transform.position.y, mazeCenter.z);
    }
}