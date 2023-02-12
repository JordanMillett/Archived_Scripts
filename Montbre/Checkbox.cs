using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Checkbox : MonoBehaviour
{
    public GameObject ToggledGraphic;

    public bool isOn = false;

    public void Initialize(bool StartingValue)
    {
        isOn = StartingValue;
        ToggledGraphic.SetActive(isOn);
    }

    public void Toggle()
    {
        MenuManager.MM.PlaySound();
        isOn = !isOn;
        ToggledGraphic.SetActive(isOn);
        
    }
}