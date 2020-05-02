using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameMode
{
    bool GameEnded();
    List<ItemType> GetItems();

}

public class ClassicGameMode : IGameMode
{
    public bool GameEnded()
    {
        return Player.Instance.AtMazeEnd;
    }

    public List<ItemType> GetItems()
    {
        return new List<ItemType>();
    }
}

public class DoorKeyGameMode : IGameMode
{
    public bool GameEnded()
    {
        return Player.Instance.AtMazeEnd && 
               Player.Instance.Inventory.Contains(ItemType.Key);
    }

    public List<ItemType> GetItems()
    {
        return new List<ItemType> { ItemType.Key };
    }
}

public class OilGameMode : IGameMode
{
    public bool GameEnded()
    {
        return Player.Instance.AtMazeEnd;
    }

    public List<ItemType> GetItems()
    {
        return new List<ItemType> { /* oil item(s) */ };
    }
}
