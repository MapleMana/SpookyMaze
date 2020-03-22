using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    void Start()
    {
        
    }

    private Vector2Int? GetInput()
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
        return null;
    }

    void Update()
    {
        Vector2Int? movement = GetInput();
        transform.position += new Vector3(movement?.x ?? 0, 0, movement?.y ?? 0);
    }
}
