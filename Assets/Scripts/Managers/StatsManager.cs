using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public class StatsManager : Singleton<StatsManager>
{
    [SerializeField]
    public Dictionary<string, int> classicStats = new Dictionary<string, int>();
    [SerializeField]
    public Dictionary<string, int> dungeonStats = new Dictionary<string, int>();
    [SerializeField]
    public Dictionary<string, int> cursedHouseStats = new Dictionary<string, int>();

    public StatsMenu statsMenu;

    private string pathClassic;
    private string pathDungeon;
    private string pathCursedHouse;

    private void Start()
    {
        pathClassic = $"{Application.persistentDataPath}/classicStats.maze";
        pathDungeon = $"{Application.persistentDataPath}/dungeonStats.maze";
        pathCursedHouse = $"{Application.persistentDataPath}/cursedHouseStats.maze";

        if (PlayerPrefs.GetInt("StatsGenerated", 0) == 0)
        {
            GenerateStats();
            PlayerPrefs.SetInt("StatsGenerated", 1);
        }
        else
        {
            LoadStats();
        }
        statsMenu.UpdateStatsText();
    }

    private void GenerateStats()
    {
        // doesn't matter what game more, generating initial stats for all modes
        GameManager.Instance.CurrentSettings.gameMode = "Classic";
        List<Dimensions> possibleDimensions = LevelIO.GetPossibleDimensions(GameManager.Instance.CurrentSettings);
        possibleDimensions.Sort(delegate (Dimensions d1, Dimensions d2)
        {
            return d1.Width.CompareTo(d2.Width);
        });
        foreach (Dimensions dimensions in possibleDimensions)
        {
            classicStats.Add(dimensions.ToString(), 0);
            dungeonStats.Add(dimensions.ToString(), 0);
            cursedHouseStats.Add(dimensions.ToString(), 0);
        }
        SaveStats(pathClassic, classicStats);
        SaveStats(pathDungeon, dungeonStats);
        SaveStats(pathCursedHouse, cursedHouseStats);
    }

    public void AddCompletedLevel(string gameMode, string dimensions)
    {
        switch (gameMode)
        {
            case "Classic":
                if (classicStats.ContainsKey(dimensions))
                {
                    classicStats[dimensions] += 1;
                    SaveStats(pathClassic, classicStats);
                }
                break;
            case "Dungeon":
                if (dungeonStats.ContainsKey(dimensions))
                {
                    dungeonStats[dimensions] += 1;
                    SaveStats(pathDungeon, dungeonStats);
                }
                break;
            case "Cursed House":
                if (cursedHouseStats.ContainsKey(dimensions))
                {
                    cursedHouseStats[dimensions] += 1;
                    SaveStats(pathCursedHouse, cursedHouseStats);
                }
                break;
            default:
                break;
        }
        statsMenu.UpdateStatsText();
    }

    public void SaveStats(string path, Dictionary<string, int> dictionary)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate))
        {
            formatter.Serialize(stream, dictionary);
        }
    }

    public void LoadStats()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(pathClassic, FileMode.Open))
        {
            classicStats = formatter.Deserialize(stream) as Dictionary<string, int>;
            stream.Close();
        }
        using (FileStream stream = new FileStream(pathDungeon, FileMode.Open))
        {
            dungeonStats = formatter.Deserialize(stream) as Dictionary<string, int>;
            stream.Close();
        }
        using (FileStream stream = new FileStream(pathCursedHouse, FileMode.Open))
        {
            cursedHouseStats = formatter.Deserialize(stream) as Dictionary<string, int>;
            stream.Close();
        }
    }
}
