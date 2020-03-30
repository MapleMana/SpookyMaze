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

    public void HeightChanged(float height)
    {
        this.height.text = height.ToString();
    }
}
