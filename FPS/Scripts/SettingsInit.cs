using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class SettingsInit : MonoBehaviour
{

    string path = "Assets/Resources/Options/Config.txt";
    TextAsset ConfigFile;

    bool Fullscreen;
    int xRes;
    int yRes;

    void Start()
    {
        //AssetDatabase.ImportAsset(path); 
        //TextAsset ConfigFile = (TextAsset)Resources.Load("Options/Config");
        ConfigFile = (TextAsset)Resources.Load("Options/Config");

        ReadFile();
        Apply();
        Destroy(this.transform.gameObject);
    }

    void ReadFile()
    {

        StreamReader reader = new StreamReader(path);
        string[] lines = ConfigFile.text.Split("\n"[0]);
        Fullscreen = lines[0].Trim().Equals("True");
        xRes = int.Parse(lines[1]);
        yRes = int.Parse(lines[2]);
        reader.Close();

    }

    public void Apply()
    {

        Screen.SetResolution(xRes, yRes, Fullscreen);

    }

    
}
