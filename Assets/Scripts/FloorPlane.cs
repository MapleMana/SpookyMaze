using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorPlane : MonoBehaviour
{
    public Material brickFloor;
    public Material houseFloor;
    public Material wheatFloor;

    public Renderer rend;

    public void ChangeFloorMaterial(string mode, int dim)
    {
        switch (mode)
        {
            case "Classic":
            default:
                rend.material = wheatFloor;                
                break;
            case "Dungeon":
                rend.material = brickFloor;
                break;
            case "Cursed House":
                rend.material = houseFloor;
                break;
        }
        rend.material.mainTextureScale = new Vector2(dim, dim);
    }
}
