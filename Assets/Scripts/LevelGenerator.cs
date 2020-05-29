using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelGenerator
{
    public const int NUM_OF_LEVELS = 10;
    const int MAZE_WIDTH_INCREMENT = 1;
    const int MAZE_HEIGHT_INCREMENT = 2;
    const int INITIAL_MAZE_WIDTH = 8;
    const int INITIAL_MAZE_HEIGHT = 8;

    private static int GetLevelTime(Dimensions dimensions)
    {
        return Mathf.FloorToInt(dimensions.Width * dimensions.Height / 2);
    }

    public static void GenerateLevels()
    {
        LevelIO.ClearAll();
        Dimensions mazeDimentions = new Dimensions(INITIAL_MAZE_WIDTH, INITIAL_MAZE_HEIGHT);
        for (int id = 1; id <= NUM_OF_LEVELS; id++)
        {
            Maze.Instance.SetDimensions(mazeDimentions);
            new BranchedDFSGeneration().Generate();
            MazeState state = new MazeState(Maze.Instance);
            string gameMode = "Classic";
            LevelIO.SaveLevel(
                new LevelSettings(id, gameMode, mazeDimentions),
                new LevelStatus(Maze.Instance, GetLevelTime(mazeDimentions), gameMode, Maze.Instance.GetRandomPositions(3))
            );
            state.SaveTo($"/{id}.maze");

            mazeDimentions.Width += MAZE_WIDTH_INCREMENT;
            mazeDimentions.Height += MAZE_HEIGHT_INCREMENT;
            Maze.Instance.Clear();
        }
    }
}
