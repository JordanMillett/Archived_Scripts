using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class E_Version : MonoBehaviour
{
    TextMeshProUGUI VersionText;

    void Start()
    {
        VersionText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        VersionText.text = "Version " +  Application.version;
    }
}