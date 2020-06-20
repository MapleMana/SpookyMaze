using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelGenerator
{
    public const int NUM_OF_LEVELS = 10;
    const int MAZE_WIDTH_INCREMENT = 8;
    const int MAZE_HEIGHT_INCREMENT = 8;
    const int INITIAL_MAZE_WIDTH = 8;
    const int INITIAL_MAZE_HEIGHT = 8;
    const int DIMENTIONS_COUNT = 3;

    private static readonly List<GameMode> gameModes = new List<GameMode>()
    {
        new ClassicGM(),
        new DoorKeyGM(),
        new OilGM(),
        new GhostGM()
    };

    private static int GetLevelTime(Dimensions dimensions, int id)
    {
        return Mathf.FloorToInt(dimensions.Width * dimensions.Height / 2) - 3 * id;
    }

    public static void GenerateLevels()
    {
        LevelIO.ClearAll();
        foreach (GameMode gameMode in gameModes)
        {
            Dimensions mazeDimentions = new Dimensions(INITIAL_MAZE_WIDTH, INITIAL_MAZE_HEIGHT);
            string gameModeName = gameMode.GetType().Name;

            for (int i = 0; i < DIMENTIONS_COUNT; i++)
            {
                for (int id = 1; id <= NUM_OF_LEVELS; id++)
                {
                    Maze.Instance.Dimensions = mazeDimentions;
                    new BranchedDFSGeneration().Generate();
                    gameMode.PlaceItems(Maze.Instance);
                    LevelIO.SaveLevel(
                        new LevelSettings(gameModeName, mazeDimentions, id),
                        new LevelData(maze: Maze.Instance,
                                        levelTime: GetLevelTime(mazeDimentions, id),
                                        mode: gameModeName,
                                        mobs: gameMode.GetMovables(),
                                        levelPoints: i * id)
                    );

                    Maze.Instance.Clear();
                }

                mazeDimentions.Width += MAZE_WIDTH_INCREMENT;
                mazeDimentions.Height += MAZE_HEIGHT_INCREMENT;
            }
        }
    }
}
