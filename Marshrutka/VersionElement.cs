  
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VersionElement : MonoBehaviour
{   
    TextMeshProUGUI VersionText;

    void Start()
    {
        VersionText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        VersionText.text = "Version " +  Application.version + (GameSettings.SC.GetCheating() ? " (CHEATING)" : "");
    }
}