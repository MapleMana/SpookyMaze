﻿/// This file contains serializables clones of some objects
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditorInternal;
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
    public Dimensions dimensions;
    public List<SerCell> cells = new List<SerCell>();

    public MazeState(Maze maze)
    {
        dimensions = maze.Dimensions;
        cells = maze.Grid.Values
            .Select(cell => new SerCell(cell))
            .ToList();
    }
}

[System.Serializable()]
public class LevelStatus
{
    public MazeState mazeState;
    public float time;
    public string gameMode;
    public List<int[]> ghostPositions;

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
    public readonly string gameMode;
    public readonly Dimensions dimensions;
    public readonly int id;

    public LevelSettings(string gameMode, Dimensions dimensions, int id)
    {
        this.gameMode = gameMode;
        this.dimensions = dimensions;
        this.id = id;
    }

    public override string ToString()
    {
        return $"{gameMode}/{dimensions}/{id}";
    }
}

public static class LevelIO
{
    private static readonly string Root = Application.persistentDataPath;

    private static string GetFilePath(LevelSettings levelSettings)
    {
        return $"{Root}/{levelSettings}.maze";
    }

    public static void ClearAll()
    {
        foreach (string subDir in Directory.GetDirectories(Root))
        {
            Directory.Delete(subDir, recursive: true);
        }
    }

    public static void SaveLevel(LevelSettings levelSettings, LevelStatus levelStatus)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = GetFilePath(levelSettings);
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate))
        {
            formatter.Serialize(stream, levelStatus);
        }
    }

    public static LevelStatus LoadLevel(LevelSettings levelSettings)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = GetFilePath(levelSettings);
        using (FileStream stream = new FileStream(path, FileMode.Open)) 
        {
            return formatter.Deserialize(stream) as LevelStatus;
        }
    }

    private static List<string> GetSubdirectoryNames(string dir)
    {
        return Directory.GetFileSystemEntries(dir)
            .Select(path => Path.GetFileNameWithoutExtension(path))
            .ToList();
    }

    public static List<Dimensions> GetPossibleDimensions(LevelSettings levelSettings)
    {
        List<string> dimensionNames = GetSubdirectoryNames($"{Root}/{levelSettings.gameMode}");
        return dimensionNames
            .Select(dimensionName => new Dimensions(dimensionName))
            .ToList();
    }

    public static List<int> GetPossibleIds(LevelSettings levelSettings)
    {
        List<string> mazeFiles = GetSubdirectoryNames($"{Root}/{levelSettings.gameMode}/{levelSettings.dimensions}");
        return mazeFiles
            .Select(mazeFile => int.Parse(mazeFile))
            .ToList();
    }
}