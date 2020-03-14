using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);

        wall.transform.position = new Vector3(0, 4, 0);
        wall.transform.localScale = new Vector3(1, 8, 6);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
