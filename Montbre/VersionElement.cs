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
        if(MenuManager.MM.CurrentScreen != "Blank")
            VersionText.text = "Version " +  Application.version;
        else
            VersionText.text = "";
    }
}