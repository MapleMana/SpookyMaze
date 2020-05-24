/// This file contains serializables clones of some objects

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable()]
public class SerCell
{
    public int[] pos;
    public bool left;
    public bool up;
    public bool right;
    public bool down;
    public int item;

    public Vector2Int Pos => new Vector2Int(pos[0], pos[1]);

    public SerCell(MazeCell cell)
    {
        pos = new int[2] { cell.Position.x, cell.Position.y };
        left = cell.WallExists(Vector2Int.left);
        up = cell.WallExists(Vector2Int.up);
        right = cell.WallExists(Vector2Int.right);
        down = cell.WallExists(Vector2Int.down);
        item = (int)(cell.Item?.Type ?? ItemType.None);
    }

    public MazeCell ToMazeCell()
    {
        MazeCell cell = new MazeCell(
            Pos, 
            up ? WallState.Exists : WallState.Destroyed,
            left ? WallState.Exists : WallState.Destroyed,
            down ? WallState.Exists : WallState.Destroyed,
            right ? WallState.Exists : WallState.Destroyed
        );
        cell.Item = ItemFactory.GetItem((ItemType)item);
        return cell;
    }
}

[System.Serializable()]
public class MazeState
{
    public int width;
    public int height;
    public List<SerCell> cells = new List<SerCell>();

    public MazeState(Maze maze)
    {
        // TODO: save level details (time)
        width = maze.Width;
        height = maze.Height;
        cells.Clear();
        foreach (MazeCell cell in maze.Grid.Values)
        {
            cells.Add(new SerCell(cell));
        }
    }

    /// <summary>
    /// Saves the state to a local file
    /// </summary>
    /// <param name="filePath">The file path to save to</param>
    public void SaveTo(string filePath)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + filePath;
        using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate))
        {
            formatter.Serialize(stream, this);
        }
    }

    /// <summary>
    /// Reads the state from a local file
    /// </summary>
    /// <returns></returns>
    public static MazeState LoadFrom(string filePath)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + filePath;
        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            return formatter.Deserialize(stream) as MazeState;
        }
    }
}
