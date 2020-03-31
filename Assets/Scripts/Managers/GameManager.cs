using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static int MazeWidth = 0;
    public static int MazeHeight = 0;
    private const int INITIAL_VALUE = 12;

    public static GameManager Instance { get => _instance; }

    private GameManager() { }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnFullLoad;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnFullLoad;
    }

    private void OnFullLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Maze")
        {
            MazeWidth = MazeWidth == 0 ? INITIAL_VALUE : MazeWidth;
            MazeHeight = MazeHeight == 0 ? INITIAL_VALUE : MazeHeight;

            Maze.Instance.Initialize(MazeWidth, MazeHeight);
            Maze.Instance.Generate();
            Maze.Instance.Display();

            CameraManager.Instance.FocusOn(Maze.Instance);

            Player.Instance.PlaceOnMaze();
        }
    }

    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
