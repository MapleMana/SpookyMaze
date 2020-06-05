using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    private Camera _camera;
    private const int MAIN_MENU_CAMERA_HEIGHT = 200;

    public float speed;
    public Vector3 playerOffset;
    public float mazeMargin;

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }

    public void FocusOnMaze(Maze maze)
    {
        float screenRatio = 1f * Screen.width / Screen.height;

        float mazeHeight = MazeCell.CELL_WIDTH * maze.Dimensions.Height;
        float mazeWidth = MazeCell.CELL_WIDTH * maze.Dimensions.Width;
        float significantSide = (mazeWidth > mazeHeight * screenRatio) ? mazeWidth : mazeHeight;

        float heightFOV = 2 * Mathf.Tan(_camera.fieldOfView * Mathf.Deg2Rad / 2);
        float widthFOV = heightFOV * screenRatio;
        float FOV = (mazeWidth > mazeHeight * screenRatio) ? widthFOV : heightFOV;

        float cameraHeight = (significantSide + 2 * mazeMargin) / FOV;
        transform.position = new Vector3(mazeWidth / 2, Mathf.Abs(cameraHeight), mazeHeight / 2);
    }

    public void FocusOnMenu(Vector3 menuPosition)
    {
        transform.position = new Vector3(menuPosition.x, menuPosition.y + MAIN_MENU_CAMERA_HEIGHT, menuPosition.z);
    }

    public void FocusOnPlayer()
    {
        transform.position = Vector3.Lerp(transform.position, Player.Instance.transform.position + playerOffset, speed * Time.deltaTime);
    }
}