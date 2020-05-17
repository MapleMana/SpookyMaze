using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameMode
{
    abstract public bool GameEnded();
    abstract public void Initialize();
    abstract public void Reset();
    abstract public List<Item> GetItems();

    public void DefaultInitialize()
    {
        Player.Instance.MazePosition = Maze.Instance.StartPos;
    }
    public void DefaultReset()
    {
        Player.Instance.MazePosition = Maze.Instance.StartPos;
    }
}

public class ClassicGameMode : GameMode
{
    public override bool GameEnded()
    {
        return Player.Instance.AtMazeEnd;
    }

    public override  void Initialize()
    {
        DefaultInitialize();
    }

    public override List<Item> GetItems()
    {
        return new List<Item>();
    }

    public override void Reset()
    {
        DefaultReset();
    }
}

public class DoorKeyGameMode : GameMode
{
    public override bool GameEnded()
    {
        return Player.Instance.AtMazeEnd &&
                Player.Instance.Inventory.Contains(ItemType.Key);
    }

    public override List<Item> GetItems()
    {
        return ItemFactory.GetItems(ItemType.Key, 1);
    }

    public override void Initialize()
    {
        DefaultInitialize();
    }

    public override void Reset()
    {
        DefaultReset();
    }
}

public class OilGameMode : GameMode
{
    public override bool GameEnded()
    {
        return Player.Instance.AtMazeEnd;
    }

    public override List<Item> GetItems()
    {
        int itemQuantity = (Maze.Instance.Height + Maze.Instance.Width) / 8; // magic formula - subject to change in the future
        return ItemFactory.GetItems(ItemType.Oil, itemQuantity);
    }

    public override void Initialize()
    {
        DefaultInitialize();
    }

    public override void Reset()
    {
        DefaultReset();
    }
}

public class GhostGameMode : GameMode
{
    List<Ghost> ghosts;
    Vector2Int StartPosition => new Vector2Int(Maze.Instance.EndPos.x, Maze.Instance.EndPos.y);

    public override bool GameEnded()
    {
        return Player.Instance.AtMazeEnd;
    }

    public override List<Item> GetItems()
    {
        return new List<Item>();
    }

    public override void Initialize()
    {
        DefaultInitialize();
        ghosts = ghosts ?? new List<Ghost>();
        foreach (Ghost ghost in ghosts)
        {
            GameObject.Destroy(ghost.gameObject);
        }
        ghosts.Clear();
        GameObject _template = Resources.Load<GameObject>("Ghost");
        Ghost.CanBeMoved = true;
        Vector2Int ghostPosition = StartPosition;
        MazeCell currentCell = Maze.Instance[ghostPosition];
        Vector3 pos = currentCell.CellCenter(y: 0);
        GameObject ghostObject = Object.Instantiate(_template, pos, Quaternion.identity);
        ghostObject.GetComponent<Ghost>().MazePosition = StartPosition;
        ghosts.Add(ghostObject.GetComponent<Ghost>());
    }

    public override void Reset()
    {
        DefaultReset();
        foreach (Ghost ghost in ghosts)
        {
            ghost.MazePosition = StartPosition;
        }
    }
}
