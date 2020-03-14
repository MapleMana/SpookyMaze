using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

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
            GenerateMaze();
        }
    }


    void Start()
    {
        
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene("Maze");
    }

    private void GenerateMaze()
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);

        wall.transform.position = new Vector3(0.5f, 0.5f, 0);
        wall.transform.localScale = new Vector3(1.25f, 1, 0.25f);
    }

    void Update()
    {
        
    }
}
