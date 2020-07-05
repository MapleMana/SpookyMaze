using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class GameMode
{
    abstract public bool GameEnded();

    abstract public List<ItemType> GetItems();

    public virtual List<SerMovable> GetMovables(int quantity=0)
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

    public override List<SerMovable> GetMovables(int quantity)
    {
        List<Vector2Int> positions = Maze.Instance.GetRandomPositions(quantity);
        return positions
            .Select((position) => new SerMovable("Ghost", position))
            .ToList();
    }
}

public class CombinedGM: GameMode
{
    GameMode[] gameModes;
    string name;

    public CombinedGM(string name, params GameMode[] gameModes)
    {
        this.name = name;
        this.gameModes = gameModes; 
    }

    public string Name { get => name; }
    public GameMode[] GameModes { get => gameModes; }

    public override bool GameEnded()
    {
        return GameModes.All(gm => gm.GameEnded());
    }

    public override List<ItemType> GetItems()
    {
        return GameModes.SelectMany(gm => gm.GetItems()).ToList();
    }

    public override List<SerMovable> GetMovables(int quantity)
    {
        var movable = GameModes.SelectMany(gm => gm.GetMovables(quantity)).ToList();
        return movable;
    }
}
