using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelGenerator
{
    public const int NUM_OF_LEVELS = 10;
    const int MAZE_WIDTH_INCREMENT = 0;
    const int MAZE_HEIGHT_INCREMENT = 0;
    const int INITIAL_MAZE_WIDTH = 8;
    const int INITIAL_MAZE_HEIGHT = 8;
    private static readonly List<string> gameModes = new List<string>()
    {
        "Classic", "DoorKey", "Oil", "Ghost"
    };

    private static int GetLevelTime(Dimensions dimensions, int id)
    {
        return Mathf.FloorToInt(dimensions.Width * dimensions.Height / 2) - 3 * id;
    }

    private static int GetNumberOfItems(Dimensions mazeDimensions)
    {
        return (Maze.Instance.Dimensions.Height + Maze.Instance.Dimensions.Width) / 8;
    }

    public static void GenerateLevels()
    {
        LevelIO.ClearAll();
        Dimensions mazeDimentions = new Dimensions(INITIAL_MAZE_WIDTH, INITIAL_MAZE_HEIGHT);

        foreach (string gameMode in gameModes)
        {
            string gameModeName = gameMode + "GM";
            for (int id = 1; id <= NUM_OF_LEVELS; id++)
            {
                Maze.Instance.SetDimensions(mazeDimentions);
                new BranchedDFSGeneration().Generate();
                int numberOfGhosts = gameMode == "Ghost" ? 0 : 1;
                int numberOfItems = GetNumberOfItems(Maze.Instance.Dimensions);
                List<Vector2Int> itemPositions = Maze.Instance.GetRandomPositions(numberOfItems);
                foreach (Vector2Int itemPosition in itemPositions)
                {
                    Maze.Instance[itemPosition].ItemType = ItemType.Oil;
                }
                LevelIO.SaveLevel(
                    new LevelSettings(gameModeName, mazeDimentions, id),
                    new LevelStatus(maze: Maze.Instance, 
                                    levelTime: GetLevelTime(mazeDimentions, id), 
                                    mode: gameModeName, 
                                    ghostStartVectors: Maze.Instance.GetRandomPositions(numberOfGhosts))
                );

                mazeDimentions.Width += MAZE_WIDTH_INCREMENT;
                mazeDimentions.Height += MAZE_HEIGHT_INCREMENT;
                Maze.Instance.Clear();
            }
        }
    }
}
