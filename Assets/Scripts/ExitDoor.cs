using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExitDoor : MonoBehaviour
{
    public GameObject door;
    public Transform rotatePoint;

    private bool openDoor;

    private void Start()
    {
        openDoor = true;
    }

    private void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 8f, LayerMask.GetMask("Player"));
        if (hitColliders.Length > 0)
        {
            if (openDoor)
            {
                door.transform.RotateAround(rotatePoint.transform.position, Vector3.back, 700 * Time.deltaTime);
                if (Mathf.Abs(TransformUtils.GetInspectorRotation(door.transform).x) > 92.5f)
                {
                    openDoor = false;
                }
            }            
        }
        else
        {
            CloseDoor();
        }
    }

    public void MoveToExit(Maze maze)
    {
        openDoor = true;
        MazeCell currentCell = Maze.Instance[maze.EndPos];
        transform.position = currentCell.CellCenter(y: transform.position.y);
    }

    public void CloseDoor()
    {
        door.transform.localPosition = new Vector3(0f, 0.1f, 0f);
        door.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
    }
}