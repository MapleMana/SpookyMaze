using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class GameMode
{
    abstract public bool GameEnded();
    abstract public void Initialize();
    abstract public void Reset();
    abstract public List<ItemType> GetItems();

    public void DefaultInitialize()
    {
        Player.Instance.MazePosition = Maze.Instance.StartPos;
    }
    public void DefaultReset()
    {
        Player.Instance.MazePosition = Maze.Instance.StartPos;
    }
}

public class ClassicGM : GameMode
{
    public override bool GameEnded()
    {
        return Player.Instance.AtMazeEnd;
    }

    public override  void Initialize()
    {
        DefaultInitialize();
    }

    public override List<ItemType> GetItems()
    {
        return new List<ItemType>();
    }

    public override void Reset()
    {
        DefaultReset();
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

    public override void Initialize()
    {
        DefaultInitialize();
    }

    public override void Reset()
    {
        DefaultReset();
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

    public override void Initialize()
    {
        DefaultInitialize();
    }

    public override void Reset()
    {
        DefaultReset();
    }
}

public class GhostGM : GameMode
{
    List<Ghost> ghosts;
    Vector2Int StartPosition => new Vector2Int(Maze.Instance.EndPos.x, Maze.Instance.EndPos.y);

    public override bool GameEnded()
    {
        return Player.Instance.AtMazeEnd;
    }

    public override List<ItemType> GetItems()
    {
        return new List<ItemType>();
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
        SceneManager.MoveGameObjectToScene(ghostObject, SceneManager.GetSceneByName("Maze"));
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
