using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameMode
{
    abstract public bool GameEnded();
    abstract public void Initialize();
    abstract public List<Item> GetItems();

    public void DefaultInitialize() { }
}

public class ClassicGameMode : GameMode
{
    override public bool GameEnded()
    {
        return Player.Instance.AtMazeEnd;
    }

    override public void Initialize()
    {
        DefaultInitialize();
    }

    override public List<Item> GetItems()
    {
        return new List<Item>();
    }
}

public class DoorKeyGameMode : GameMode
{
    override public bool GameEnded()
    {
        return Player.Instance.AtMazeEnd &&
                Player.Instance.Inventory.Contains(ItemType.Key);
    }

    override public List<Item> GetItems()
    {
        return ItemFactory.GetItems(ItemType.Key, 1);
    }

    override public void Initialize()
    {
        DefaultInitialize();
    }
}

public class OilGameMode : GameMode
{
    override public bool GameEnded()
    {
        return Player.Instance.AtMazeEnd;
    }

    override public List<Item> GetItems()
    {
        int itemQuantity = (Maze.Instance.Height + Maze.Instance.Width) / 8; // magic formula - subject to change in the future
        return ItemFactory.GetItems(ItemType.Oil, itemQuantity);
    }

    override public void Initialize()
    {
        DefaultInitialize();
    }
}

public class GhostGameMode : GameMode
{
    override public bool GameEnded()
    {
        return Player.Instance.AtMazeEnd;
    }

    override public void Initialize()
    {
        Ghost.CanBeMoved = true;
        GameObject _template = Resources.Load<GameObject>("Ghost");
        Vector2Int _mazePosition = new Vector2Int(Maze.Instance.EndPos.x, Maze.Instance.EndPos.y);
        MazeCell currentCell = Maze.Instance[_mazePosition];
        Vector3 pos = currentCell.CellCenter(y: 0);
        Object.Instantiate(_template, pos, Quaternion.identity);
    }

    override public List<Item> GetItems()
    {
        return new List<Item>();
    }
}
