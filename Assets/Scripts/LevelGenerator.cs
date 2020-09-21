using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class LevelGenerator
{
    public const int NUM_OF_LEVELS = 20;
    const int MAZE_WIDTH_INCREMENT = 2;
    const int MAZE_HEIGHT_INCREMENT = 2;
    const int INITIAL_MAZE_WIDTH = 8;
    const int INITIAL_MAZE_HEIGHT = 8;
    const int DIMENTIONS_COUNT = 10;
    const int NUM_OF_PACKS_PER_SIZE = 5;
    const int SEED = 145;
    
    private static readonly List<CombinedGM> gameModes = new List<CombinedGM>()
    {
        new CombinedGM("Classic", new ClassicGM()),
        new CombinedGM("Dungeon", new DoorKeyGM()),
        new CombinedGM("Cursed House", new OilGM(), new GhostGM()),
    };

    private static int GetLevelTime(int pathLength)
    {
        return pathLength * 3;
    }

    private static int GetMobQuantity(Dimensions mazeDimensions)
    {
        return (mazeDimensions.Width + mazeDimensions.Height) / 16; 
    }

    private static int GetLevelPoints()
    {
        // same number of points / coins for each level 
        return 4;
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
                            new LevelSettings(gameModeName, mazeDimentions, id, packId, unlocked),
                            new LevelData(maze: Maze.Instance,
                                          levelTime: GetLevelTime(Maze.Instance.GetPathLength()),
                                          modeNames: combinedGM.GameModes.Select(gm => gm.GetType().Name).ToArray(),
                                          mobs: combinedGM.GetMovables(GetMobQuantity(mazeDimentions)),
                                          levelPoints: GetLevelPoints())
                        );
                        Maze.Instance.Clear();
                    }
                }              
                mazeDimentions.Width += MAZE_WIDTH_INCREMENT;
                mazeDimentions.Height += MAZE_HEIGHT_INCREMENT;
            }
        }
    }
}
