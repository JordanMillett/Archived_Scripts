using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class App : MonoBehaviour
{
    public GameObject Screen;
    public bool AppRunning = false;

    public void Toggle()
    {
        AppRunning = !AppRunning;
        Screen.SetActive(AppRunning);
    }
}
