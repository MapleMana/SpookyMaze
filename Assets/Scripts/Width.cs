using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Width : MonoBehaviour
{
    Text width;

    // Start is called before the first frame update
    void Start()
    {
        width = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Called when slider value is changed, displays width and changes it in GameManager
    /// </summary>
    /// <param name="width"></param>
    public void WidthChanged(float width)
    {
        this.width.text = width.ToString();
        GameManager.MazeWidth = Convert.ToInt32(width);
    }
}
