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
        if (itemType == ItemType.None) return null;
        GameObject template = Resources.Load<GameObject>(itemType.ToString());
        GameObject newItem = Object.Instantiate(template, pos, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(newItem, SceneManager.GetSceneByName("Maze"));
        return newItem;
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