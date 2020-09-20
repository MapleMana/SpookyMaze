using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class LevelGenerator
{
    public const int NUM_OF_LEVELS = 10;
    public const int NUM_OF_DAILY_LEVELS = 4;
    const int MAZE_WIDTH_INCREMENT = 8;
    const int MAZE_HEIGHT_INCREMENT = 8;
    const int INITIAL_MAZE_WIDTH = 8;
    const int INITIAL_MAZE_HEIGHT = 8;
    const int DIMENTIONS_COUNT = 3;
    const int SEED = 145;
    
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
        return pathLength * 3;
    }

    private static int GetMobQuantity(Dimensions mazeDimensions)
    {
        return (mazeDimensions.Width + mazeDimensions.Height) / 16; 
    }

    private static int GetLevelPoints(Dimensions dimensions, int id)
    {
        return (dimensions.Width + dimensions.Height) * id;
    }

    public static void GenerateLevels()
    {
        UnityEngine.Random.InitState(SEED);
        LevelIO.ClearAll();

        foreach (CombinedGM combinedGM in gameModes)
        {
            Dimensions mazeDimentions = new Dimensions(INITIAL_MAZE_WIDTH, INITIAL_MAZE_HEIGHT);

            for (int i = 0; i < DIMENTIONS_COUNT; i++)
            {
                for (int id = 1; id <= NUM_OF_LEVELS; id++)
                {
                    Maze.Instance.Dimensions = mazeDimentions;
                    new BranchedDFSGeneration(Maze.Instance).Generate();
                    combinedGM.PlaceItems(Maze.Instance);
                    LevelIO.SaveLevel(
                        new LevelSettings(combinedGM.Name, mazeDimentions, id),
                        new LevelData(maze: Maze.Instance,
                                      levelTime: GetLevelTime(Maze.Instance.GetPathLength()),
                                      modeNames: combinedGM.GameModes.Select(gm => gm.GetType().Name).ToArray(),
                                      mobs: combinedGM.GetMovables(GetMobQuantity(mazeDimentions)),
                                      levelPoints: GetLevelPoints(mazeDimentions, id))
                    );

                    Maze.Instance.Clear();
                }

                mazeDimentions.Width += MAZE_WIDTH_INCREMENT;
                mazeDimentions.Height += MAZE_HEIGHT_INCREMENT;
            }
        }

    }
    internal static void GenerateDailyLevels()
    {
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
                                    levelTime: GetLevelTime(Maze.Instance.GetPathLength()),
                                    modeNames: combinedGM.GameModes.Select(gm => gm.GetType().Name).ToArray(),
                                    mobs: combinedGM.GetMovables(GetMobQuantity(mazeDimentions)),
                                    levelPoints: GetLevelPoints(mazeDimentions, id))
                );

                Maze.Instance.Clear();
            }
        }

    }
}
