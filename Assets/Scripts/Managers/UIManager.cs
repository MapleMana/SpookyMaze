using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;

    public static UIManager Instance { get => _instance; set => _instance = value; }

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

    void Start()
    {
        Invoke("StartGame", 1f);
    }

    /// <summary>
    /// This method is invoked when the "Play" button is pressed
    /// </summary>
    void StartGame()
    {
        GameManager.Instance.LoadLevel("Maze");
    }
    
    void Update()
    {
        
    }
}
