using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    private Camera _camera;

    public float speed;
    public Vector3 playerOffset;
    public float mazeMargin;

    private Vector3 startPos;
    private Vector3 endPos;
    private float journeyLength;
    private float startTime;
    private float speedPop = 120f;
    private bool zoomOutCamera = false;

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (zoomOutCamera)
        {
            // Distance moved equals elapsed time times speed..
            float distCovered = (Time.time - startTime) * speedPop;

            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = distCovered / journeyLength;

            // Set our position as a fraction of the distance between the markers.
            transform.position = Vector3.Lerp(startPos, endPos, fractionOfJourney);

            if (distCovered >= journeyLength)
            {
                zoomOutCamera = false;
            }
        }
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

        // Keep a note of the time the movement started.
        startTime = Time.time;

        startPos = transform.position;
        endPos = new Vector3(mazeWidth / 2, Mathf.Abs(cameraHeight), mazeHeight / 2);

        // Calculate the journey length.
        journeyLength = Vector3.Distance(startPos, endPos);
        zoomOutCamera = true;
    }

    public void FocusOnPlayer()
    {
        transform.position = Vector3.Lerp(transform.position, Player.Instance.transform.position + playerOffset, speed * Time.deltaTime);
    }
}