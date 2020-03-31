using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance;
    private Camera _camera;

    public float widthMargin;

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
        float mazeHeight = MazeCell.CELL_WIDTH * maze.Height;
        float mazeWidth = MazeCell.CELL_WIDTH * maze.Width;
        float largerSide = Mathf.Max(mazeWidth, mazeHeight);
        float heightFOV = 2 * Mathf.Tan(_camera.fieldOfView * Mathf.Deg2Rad / 2);
        float widthFOV = heightFOV * Screen.width / Screen.height;

        float cameraHeight = (largerSide + 2 * widthMargin) / widthFOV;
        transform.position = new Vector3(mazeWidth / 2, Mathf.Abs(cameraHeight), mazeHeight / 2);
    }
}