using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    private static LightManager _instance;
    public new Light light;

    public static LightManager Instance { get => _instance; }

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

    /// <summary>
    /// DirectionalLight turns on, all maze becomes visible 
    /// </summary>
    public void TurnOn()
    {
        light.intensity = 1;
        Player.Instance.PlayerLight.intensity = 0;
    }

    /// <summary>
    /// DirectionalLight turns off, only highlighted part with the player is visible
    /// </summary>
    public void TurnOff()
    {
        light.intensity = 0;
        Player.Instance.PlayerLight.intensity = Player.Instance.LightIntensity;
    }
}
