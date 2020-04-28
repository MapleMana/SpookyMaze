using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : System.IDisposable
{
    private static GameObject _keyTemplate = Resources.Load<GameObject>("Key");
    private GameObject _template;
    private GameObject gameObject;
    private ItemType _type;

    public ItemType Type => _type;

    public Item(ItemType type = ItemType.None)
    {
        _type = type;
        switch (type)
        {
            case ItemType.None:
                break;
            case ItemType.Key:
                _template = _keyTemplate;
                break;
            default:
                break;
        }
    }

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
