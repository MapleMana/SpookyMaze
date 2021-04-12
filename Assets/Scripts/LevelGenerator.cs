using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class LevelGenerator
{
    public const int NUM_OF_DAILY_LEVELS = 4;
    public const int NUM_OF_LEVELS = 20;
    const int MAZE_WIDTH_INCREMENT = 2;
    const int MAZE_HEIGHT_INCREMENT = 2;
    const int INITIAL_MAZE_WIDTH = 8;
    const int INITIAL_MAZE_HEIGHT = 8;
    const int DIMENTIONS_COUNT = 10;
    const int NUM_OF_PACKS_PER_SIZE = 5;
    const int SEED = 145;
    const int LEVEL_REWARD = 4;
    
    private static readonly List<CombinedGM> gameModes = new List<CombinedGM>()
    {
        new CombinedGM("Classic", new ClassicGM()),
        new CombinedGM("Dungeon", new DoorKeyGM()),
        new CombinedGM("Cursed House", new OilGM(), new GhostGM()),
    };

    private static Dimensions GetMazeDimensions(int id)
    {
        // TODO: figure out a better formula
        return new Dimensions(8, 8);
    }

    private static int GetLevelTime(int pathLength)
    {
        return (int)(pathLength * 1);
    }

    private static int GetMobQuantity(Dimensions mazeDimensions)
    {
        return (mazeDimensions.Width + mazeDimensions.Height) / 16; 
    }

    public static void GenerateLevels()
    {
        UnityEngine.Random.InitState(SEED);
        LevelIO.ClearAll();
        string packId;
        bool unlocked;
        foreach (CombinedGM combinedGM in gameModes)
        {
            Dimensions mazeDimentions = new Dimensions(INITIAL_MAZE_WIDTH, INITIAL_MAZE_HEIGHT);
            string gameModeName = combinedGM.Name;

            for (int i = 0; i < DIMENTIONS_COUNT; i++)
            {
                for (int ip = 0; ip < NUM_OF_PACKS_PER_SIZE; ip++)
                {
                    for (int id = 1; id <= NUM_OF_LEVELS; id++)
                    {
                        switch (ip)
                        {
                            case 0:
                            default:
                                packId = "A";
                                unlocked = true;
                                break;
                            case 1:
                                packId = "B";
                                unlocked = false;
                                break;
                            case 2:
                                packId = "C";
                                unlocked = false;
                                break;
                            case 3:
                                packId = "D";
                                unlocked = false;
                                break;
                            case 4:
                                packId = "E";
                                unlocked = false;
                                break;
                        }
                        Maze.Instance.Dimensions = mazeDimentions;
                        new BranchedDFSGeneration(Maze.Instance).Generate();
                        combinedGM.PlaceItems(Maze.Instance);
                        LevelIO.SaveLevel(
                            new LevelSettings(gameModeName, mazeDimentions, id, packId),
                            new LevelData(maze: Maze.Instance,
                                          levelTime: GetLevelTime(gameModeName == "Dungeon" ? Maze.Instance.GetPathLengthWithKey() : Maze.Instance.GetPathLength()),
                                          modeNames: combinedGM.GameModes.Select(gm => gm.GetType().Name).ToArray(),
                                          mobs: combinedGM.GetMovables(GetMobQuantity(mazeDimentions)),
                                          levelPoints: LEVEL_REWARD,
                                          levelUnlocked: unlocked,
                                          levelComplete: false)
                        );
                        LevelIO.SaveLevelPackData(
                            new LevelSettings(gameModeName, mazeDimentions, id, packId),
                            new LevelPackData(complete: 0)
                        );
                        Maze.Instance.Clear();
                    }
                }              
                mazeDimentions.Width += MAZE_WIDTH_INCREMENT;
                mazeDimentions.Height += MAZE_HEIGHT_INCREMENT;
            }
        }

    }
    internal static void GenerateDailyLevels()
    {        
        string currentDate = (DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString());
        if (currentDate == PlayerPrefs.GetString("currentDate",""))
        {
            return;
        }
        PlayerPrefs.SetInt("Classic", 0);
        PlayerPrefs.SetInt("Dungeon", 0);
        PlayerPrefs.SetInt("Cursed House", 0);
        PlayerPrefs.SetString("currentDate", currentDate);
        PlayerPrefs.Save();

        int dailySeed = (int)(DateTimeOffset.Now.ToUnixTimeSeconds() / (3600 * 24));
        UnityEngine.Random.InitState(dailySeed);

        foreach (CombinedGM combinedGM in gameModes)
        {
            for (int id = 1; id <= NUM_OF_DAILY_LEVELS; id++)
            {
                Dimensions mazeDimentions = GetMazeDimensions(id);
                GameManager.Instance.CurrentSettings.dimensions = mazeDimentions;
                GameManager.Instance.CurrentSettings.id = id;

                Maze.Instance.Dimensions = mazeDimentions;
                new BranchedDFSGeneration(Maze.Instance).Generate();
                combinedGM.PlaceItems(Maze.Instance);
                LevelIO.SaveLevel(
                    new LevelSettings(combinedGM.Name, mazeDimentions, id, isDaily: true),
                    new LevelData(maze: Maze.Instance,
                                    levelTime: GetLevelTime(combinedGM.Name == "Dungeon" ? Maze.Instance.GetPathLengthWithKey() : Maze.Instance.GetPathLength()),
                                    modeNames: combinedGM.GameModes.Select(gm => gm.GetType().Name).ToArray(),
                                    mobs: combinedGM.GetMovables(GetMobQuantity(mazeDimentions)),
                                    levelPoints: LEVEL_REWARD,
                                    levelUnlocked: true,
                                    levelComplete: false)
                );

                Maze.Instance.Clear();
            }
        }

    }
}