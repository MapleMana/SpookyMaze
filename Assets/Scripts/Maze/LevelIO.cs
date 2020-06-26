/// This file contains serializables clones of some objects
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable()]
public class SerCell
{
    public int[] pos;
    public bool left;
    public bool up;
    public bool right;
    public bool down;
    public int itemType;

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
            pos.ToVector2Int(), 
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
public class SerMovable
{
    string type;
    int[] startingMazePosition;

    public SerMovable(string movableType, Vector2Int startingPosition)
    {
        type = movableType;
        startingMazePosition = startingPosition.ToArray();
    }

    public Movable Spawn()
    {
        GameObject template = Resources.Load<GameObject>(type);
        GameObject movableObject = UnityEngine.Object.Instantiate(template, Vector3.zero, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(movableObject, SceneManager.GetSceneByName("Maze"));

        Movable movableComponent = movableObject.GetComponent<Movable>();
        // TODO: figure out null reference thing:
        movableComponent.StartingPosition = startingMazePosition.ToVector2Int();
        return movableComponent;
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
public class LevelData
{
    public MazeState mazeState;
    public float time;
    public string gameMode;
    public List<SerMovable> movables;

    public LevelData(Maze maze, float levelTime, string mode, List<SerMovable> mobs)
    {
        mazeState = new MazeState(maze);
        time = levelTime;
        gameMode = mode;
        movables = mobs;
    }

    public List<Movable> SpawnMovables()
    {
        return movables.Select(serMovable => serMovable.Spawn()).ToList();
    }

    public GameMode GetGameMode()
    {
        Type GMType = Type.GetType(gameMode);
        return (GameMode)Activator.CreateInstance(GMType);
    }
}

public class LevelSettings
{
    public string gameMode;
    public Dimensions dimensions;
    public int id;
    public string ModeDimensions => $"{this.gameMode}{this.dimensions}";

    public LevelSettings() { }

    public LevelSettings(string gameMode, Dimensions dimensions, int id)
    {
        this.gameMode = gameMode;
        this.dimensions = dimensions;
        this.id = id;
    }

    public string GetReadableGameMode()
    {
        string modeName = gameMode.Replace("GM", "");
        MatchCollection matches = Regex.Matches(modeName, "[A-Z][a-z]+");
        List<string> matchWords = matches.Cast<Match>().Select(match => match.Value).ToList();
        return String.Join(" ", matchWords);
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

    public static void SaveLevel(LevelSettings levelSettings, LevelData levelStatus)
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

    public static LevelData LoadLevel(LevelSettings levelSettings)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = GetFilePath(levelSettings);
        using (FileStream stream = new FileStream(path, FileMode.Open)) 
        {
            return formatter.Deserialize(stream) as LevelData;
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