using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Item : MonoBehaviour
{
    public abstract ItemType Type { get; }

    public abstract void Activate();
    public abstract void Deactivate();
}

public class ItemFactory
{
    public static GameObject SpawnItem(ItemType itemType, Vector3 pos)
    {
        GameObject itemObject;
        switch (itemType)
        {
            case ItemType.Key:
                itemObject = Object.Instantiate(Resources.Load<GameObject>("Key"), pos, Quaternion.identity);
                break;
            case ItemType.Oil:
                itemObject = Object.Instantiate(Resources.Load<GameObject>("Oil"), pos, Quaternion.identity);
                break;
            default:
                return null;
        }
        SceneManager.MoveGameObjectToScene(itemObject, SceneManager.GetSceneByName("Maze"));
        return itemObject;
    }

    public static List<ItemType> GetItems(ItemType itemType, int quantity)
    {
        List<ItemType> res = new List<ItemType>();
        for (int i = 0; i < quantity; i++)
        {
            res.Add(itemType);
        }
        return res;
    }
}