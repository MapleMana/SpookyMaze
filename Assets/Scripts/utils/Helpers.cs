using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtension
{
    public static void Shuffle<T>(this List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(0, list.Count);
            int k = Random.Range(0, list.Count);
            T value = list[k];
            list[k] = list[j];
            list[j] = value;
        }
    }
}

public enum WallState
{
    Destroyed,
    Exists
}

public enum ItemType
{
    None = 0,
    Key = 1,
    Oil = 2
}

[System.Flags]
public enum LevelState
{
    None = 0,
    InProgress = 1,
    Completed = 2,
    Failed = 4,
    InReplay = 8,
    InReplayReversed = 16
}

[System.Serializable()]
public struct Dimensions
{
    public int Width;
    public int Height;

    public Dimensions(int Width, int Height)
    {
        this.Width = Width;
        this.Height = Height;
    }

    public Dimensions(string dimensionName)
    {
        string[] dimensionNameParts = dimensionName.Split('x');
        int.TryParse(dimensionNameParts[0], out Width);
        int.TryParse(dimensionNameParts[1], out Height);
    }

    public override string ToString()
    {
        return $"{Width}x{Height}";
    }
}
