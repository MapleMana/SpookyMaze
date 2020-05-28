/// This file contains serializables clones of some objects
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
    public int itemType;

    public Vector2Int Pos => new Vector2Int(pos[0], pos[1]);

    public SerCell(MazeCell cell)
    {
        pos = new int[2] { cell.Position.x, cell.Position.y };
        left = cell.WallExists(Vector2Int.left);
        up = cell.WallExists(Vector2Int.up);
        right = cell.WallExists(Vector2Int.right);
        down = cell.WallExists(Vector2Int.down);
        itemType = (int)cell.ItemType;
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
        cell.ItemType = (ItemType)itemType;
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
        width = maze.Width;
        height = maze.Height;
        cells = maze.Grid.Values
            .Select(cell => new SerCell(cell))
            .ToList();
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

[System.Serializable()]
public class LevelStatus
{
    MazeState mazeState;
    float time;
    string gameMode;
    List<int[]> ghostPositions;

    public LevelStatus(Maze maze, float levelTime, string mode, List<Vector2Int> ghostStartVectors)
    {
        mazeState = new MazeState(maze);
        time = levelTime;
        gameMode = mode;
        ghostPositions = ghostStartVectors
            .Select(pos => new int[2] { pos.x, pos.y })
            .ToList();
    }
}

public struct LevelSettings
{
    public readonly int id;
    public readonly string gameMode;
    public readonly Vector2Int dimensions;

    public LevelSettings(int id, string gameMode, Vector2Int dimensions)
    {
        this.id = id;
        this.gameMode = gameMode;
        this.dimensions = dimensions;
    }

    public override string ToString()
    {
        return $"{gameMode}/{dimensions.x}x{dimensions.y}/{id}";
    }
}

public static class LevelIO
{
    private static string GetFilePath(LevelSettings levelSettings)
    {
        return $"{Application.persistentDataPath}/{levelSettings}";
    }

    public static void SaveLevel(LevelSettings levelSettings, LevelStatus levelStatus)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = GetFilePath(levelSettings);
        using FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
        formatter.Serialize(stream, levelStatus);
    }

    public static LevelStatus LoadLevel(LevelSettings levelSettings)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = GetFilePath(levelSettings);
        using FileStream stream = new FileStream(path, FileMode.Open);
        return formatter.Deserialize(stream) as LevelStatus;
    }
}