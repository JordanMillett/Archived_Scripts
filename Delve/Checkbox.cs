using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkbox : MonoBehaviour
{
    public GameObject On;
    public GameObject Off;
    public bool isEnabled;

    void Start()
    {

        On.SetActive(isEnabled);
        Off.SetActive(!isEnabled);

    }

    public void Toggle()
    {

        isEnabled = !isEnabled;
        On.SetActive(isEnabled);
        Off.SetActive(!isEnabled);

    }
}
