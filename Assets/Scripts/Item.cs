using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : System.IDisposable
{
    internal GameObject _template;
    private GameObject gameObject;

    public abstract ItemType Type { get; }
    public abstract void Activate();
    public abstract void Deactivate();

    public void Display(Vector3 pos)
    {
        if (_template != null)
        {
            gameObject = Object.Instantiate(_template, pos, Quaternion.identity);
        }
    }

    public void Dispose()
    {
        Object.Destroy(gameObject);
    }
}

public class Key : Item
{
    public override ItemType Type => ItemType.Key;

    public Key()
    {
        _template = Resources.Load<GameObject>("Key");
    }

    public override void Activate() {}
    public override void Deactivate() {}
}

public class Oil : Item
{
    const float EFFECTIVENESS = 0.3f; // percentage of the total time to add
    public override ItemType Type => ItemType.Oil;

    public Oil()
    {
        _template = Resources.Load<GameObject>("Oil");
    }

    public override void Activate()
    {
        GameManager.Instance.AddTime(ratio: EFFECTIVENESS);
    }

    public override void Deactivate()
    {
        GameManager.Instance.AddTime(ratio: -EFFECTIVENESS);
    }
}

/// <summary>
/// Creates Item instances based on types
/// </summary>
public class ItemFactory
{
    public static Item GetItem(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Key:
                return new Key();
            case ItemType.Oil:
                return new Oil();
            default:
                return null;
        }
    }

    public static List<Item> GetItems(ItemType itemType, int quantity)
    {
        List<Item> res = new List<Item>();
        for (int i = 0; i < quantity; i++)
        {
            res.Add(GetItem(itemType));
        }
        return res;
    }
}