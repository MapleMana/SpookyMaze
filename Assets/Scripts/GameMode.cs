﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameMode
{
    bool GameEnded();
    List<Item> GetItems();
}

public class ClassicGameMode : IGameMode
{
    public bool GameEnded()
    {
        return Player.Instance.AtMazeEnd;
    }

    public List<Item> GetItems()
    {
        return new List<Item>();
    }
}

public class DoorKeyGameMode : IGameMode
{
    public bool GameEnded()
    {
        return Player.Instance.AtMazeEnd && 
               Player.Instance.Inventory.Contains(ItemType.Key);
    }

    public List<Item> GetItems()
    {
        return ItemFactory.GetItems(ItemType.Key, 1);
    }
}

public class OilGameMode : IGameMode
{
    public bool GameEnded()
    {
        return Player.Instance.AtMazeEnd;
    }

    public List<Item> GetItems()
    {
        int itemQuantity = (Maze.Instance.Height + Maze.Instance.Width) / 8; // magic formula - subject to change in the future
        return ItemFactory.GetItems(ItemType.Oil, itemQuantity);
    }
}
