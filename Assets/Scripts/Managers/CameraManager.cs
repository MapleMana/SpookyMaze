using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Vector3 menuPosition = new Vector3(0, 200, 0);

    private static CameraManager _instance;
    private Camera _camera;

    public float mazeMargin;

    public static CameraManager Instance { get => _instance; }

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }

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
        float screenRatio = 1f * Screen.width / Screen.height;

        float mazeHeight = MazeCell.CELL_WIDTH * maze.Height;
        float mazeWidth = MazeCell.CELL_WIDTH * maze.Width;
        float significantSide = (mazeWidth > mazeHeight * screenRatio) ? mazeWidth : mazeHeight;

        float heightFOV = 2 * Mathf.Tan(_camera.fieldOfView * Mathf.Deg2Rad / 2);
        float widthFOV = heightFOV * screenRatio;
        float FOV = (mazeWidth > mazeHeight * screenRatio) ? widthFOV : heightFOV;

        float cameraHeight = (significantSide + 2 * mazeMargin) / FOV;
        transform.position = new Vector3(mazeWidth / 2, Mathf.Abs(cameraHeight), mazeHeight / 2);
    }

    public void FocusOnMenu()
    {
        transform.position = menuPosition;
    }
}