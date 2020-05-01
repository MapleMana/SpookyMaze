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
    None,
    Key
}

//[System.Flags]
//public enum GameMode
//{
//    Classic = 0,
//    Key = 1,
//    Oil = 2,
//    Ghost = 3
//}

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

