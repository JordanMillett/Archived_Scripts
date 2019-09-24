using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class GreetingsMaker : MonoBehaviour
{
    bool FilesLoaded = false;
    TextMeshProUGUI MainText;
    TextAsset Greetings_File;
    string[] Greetings;


    void Start()
    {
        if(!FilesLoaded)
            LoadFiles();

        MainText = GetComponentInChildren<TextMeshProUGUI>();
        MainText.text = Greetings[Random.Range(0,Greetings.Length)];
        
    }

    void OnEnable()
    {

        if(FilesLoaded)
            MainText.text = Greetings[Random.Range(0,Greetings.Length)];

    }


    void LoadFiles()
    {
        Greetings_File = Resources.Load<TextAsset>("Texts/Greetings");
        Greetings = Greetings_File.text.Split('\n');

        FilesLoaded = true;
    }
}
