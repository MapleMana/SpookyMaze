using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Height : MonoBehaviour
{
    Text height;

    // Start is called before the first frame update
    void Start()
    {
        height = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Called when slider value is changed, displays height and changes it in GameManager
    /// </summary>
    /// <param name="height"></param>
    public void HeightChanged(float height)
    {
        this.height.text = height.ToString();
        GameManager.MazeHeight = Convert.ToInt32(height);
    }
}
