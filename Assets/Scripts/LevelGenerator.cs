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

    public static void GenerateLevels()
    {
        int mazeWidth = INITIAL_MAZE_WIDTH;
        int mazeHeight = INITIAL_MAZE_HEIGHT;
        for (int i = 0; i < NUM_OF_LEVELS; i++)
        { 
            // FIXME: instantiate the maze when it's not a singleton
            Maze.Instance.SetDimensions(mazeWidth, mazeHeight);
            new BranchedDFSGeneration().Generate();
            MazeState state = new MazeState(Maze.Instance);
            state.SaveTo($"/{i}.maze");
            mazeWidth += MAZE_WIDTH_INCREMENT;
            mazeHeight += MAZE_HEIGHT_INCREMENT;
        }
    }
}
