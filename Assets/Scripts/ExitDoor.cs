using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExitDoor : MonoBehaviour
{
    public GameObject door;
    public Transform rotatePoint;
    public GameObject lockImage;

    private bool openDoor;

    private void Update()
    {
        if (lockImage.activeInHierarchy)
        {
            // checked if key is in player inventory
            if (Player.Instance.Inventory.Contains(ItemType.Key))
            {
                lockImage.SetActive(false);
                openDoor = true;
                UIManager.Instance.PickedUpKey();
            }
            MoveLockImageOverDoor();
        }        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 8f, LayerMask.GetMask("Player"));
        if (hitColliders.Length > 0)
        {
            if (openDoor)
            {
                door.transform.RotateAround(rotatePoint.transform.position, Vector3.back, 700 * Time.deltaTime);
                if (door.transform.rotation.eulerAngles.x < 274f)
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
        if (GameManager.Instance.CurrentSettings.GetReadableGameMode() != "Dungeon")
        {
            lockImage.SetActive(false);
            openDoor = true;
        }
        else
        {
            // in Dungeon mode, door won't open until key is in player inventory
            lockImage.SetActive(true);
            openDoor = false;
        }
        MazeCell currentCell = Maze.Instance[maze.EndPos];
        transform.position = currentCell.CellCenter(y: transform.position.y);
    }

    public void CloseDoor()
    {
        door.transform.localPosition = new Vector3(0f, 0.1f, 0f);
        door.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
    }

    private void MoveLockImageOverDoor()
    {
        lockImage.transform.position = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z));
    }
}