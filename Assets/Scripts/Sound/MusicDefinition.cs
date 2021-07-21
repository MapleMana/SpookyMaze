using UnityEngine;

[System.Serializable]
public struct MusicDefinition
{
    public Music music;
    public AudioClip Clip;
}

[System.Serializable]
public enum Music
{
    MenuMusic,
    ClassicMusic,
    DungeonMusic,
    CursedHouseMusic
}