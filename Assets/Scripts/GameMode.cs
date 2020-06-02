using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class GameMode
{
    abstract public bool GameEnded();

    public virtual void Reset()
    {
        Player.Instance.MazePosition = Maze.Instance.StartPos;
    }

    abstract public List<ItemType> GetItems();

    public virtual List<SerMovable> GetMovables()
    {
        return new List<SerMovable>();
    }

    public void PlaceItems(Maze maze)
    {
        maze.PlaceOnMaze(GetItems());
    }
}

public class ClassicGM : GameMode
{
    public override bool GameEnded()
    {
        return Player.Instance.AtMazeEnd;
    }

    public override List<ItemType> GetItems() 
    {
        return new List<ItemType>();
    }
}

public class DoorKeyGM : GameMode
{
    public override bool GameEnded()
    {
        return Player.Instance.AtMazeEnd &&
                Player.Instance.Inventory.Contains(ItemType.Key);
    }

    public override List<ItemType> GetItems()
    {
        return ItemFactory.GetItems(ItemType.Key, 1);
    }
}

public class OilGM : GameMode
{
    public override bool GameEnded()
    {
        return Player.Instance.AtMazeEnd;
    }

    public override List<ItemType> GetItems()
    {
        int itemQuantity = (Maze.Instance.Dimensions.Height + Maze.Instance.Dimensions.Width) / 8; // magic formula - subject to change in the future
        return ItemFactory.GetItems(ItemType.Oil, itemQuantity);
    }
}

public class GhostGM : GameMode
{
    public override bool GameEnded()
    {
        return Player.Instance.AtMazeEnd;
    }

    public override List<ItemType> GetItems()
    {
        return new List<ItemType>();
    }

    public override List<SerMovable> GetMovables()
    {
        int ghostQuantity = (Maze.Instance.Dimensions.Height + Maze.Instance.Dimensions.Width) / 16; // magic formula - subject to change in the future
        List<SerMovable> ghosts = new List<SerMovable>();

        foreach (Vector2Int position in Maze.Instance.GetRandomPositions(ghostQuantity))
        {
            ghosts.Add(new SerMovable("Ghost", position));
        }

        return ghosts;
    }
}
