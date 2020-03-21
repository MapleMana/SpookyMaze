using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private Maze _mazeObject;

    public Maze mazeTemplate;

    public static GameManager Instance { get => _instance; }

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
            _mazeObject = Instantiate(mazeTemplate, Vector3.zero, Quaternion.identity);
            _mazeObject.Initialize(10, 15);
            _mazeObject.Generate();
            _mazeObject.Display();
            CameraManager.Instance.FocusOn(_mazeObject);
        }
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene("Maze");
    }
}
