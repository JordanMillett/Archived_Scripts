using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEditor;

public class OptionsMenu : MonoBehaviour
{

    TextAsset ConfigFile;
    public TextMeshProUGUI Resolution;
    public Toggle FullscreenButton;

    string path = "Assets/Resources/Options/Config.txt";

    bool Fullscreen;
    int xRes;
    int yRes;

    void UpdateFile()
    {

        ClearFile();

        StreamWriter writer = new StreamWriter(path);
        writer.WriteLine(Fullscreen);
        writer.WriteLine(xRes);
        writer.WriteLine(yRes);
        writer.Close();

        //AssetDatabase.ImportAsset(path); 
        ConfigFile = (TextAsset)Resources.Load("Options/Config");

    }

    void OnEnable()
    {

        //AssetDatabase.ImportAsset(path); 
        ConfigFile = (TextAsset)Resources.Load("Options/Config");

        ReadFile();
        SetValues();

    }

    void ReadFile()
    {

        //ConfigFile = Resources.Load<TextAsset>("Config");

        StreamReader reader = new StreamReader(path);
        string[] lines = ConfigFile.text.Split("\n"[0]);


        //Debug.Log(lines[0] + " = " + (lines[0] == "True"));   Are you
        //Debug.Log(lines[0].Equals("True"));                   Fucking
        //Debug.Log(lines[0].Trim().Equals("True"));            Kidding me?


        Fullscreen = lines[0].Trim().Equals("True");
        xRes = int.Parse(lines[1]);
        yRes = int.Parse(lines[2]);
        reader.Close();

    }
    
    void SetValues()
    {

        Resolution.text = xRes + "x" + yRes;
        FullscreenButton.Change(Fullscreen);

    }

    void ClearFile()
    {
        System.IO.File.WriteAllText(path,string.Empty);
        //FileStream writer = new FileStream(AssetDatabase.GetAssetPath(ConfigFile), FileMode.Truncate);
        //writer.WriteLine("");
        //writer.Close();

    }

    public void Apply()
    {


        GetValues();
        UpdateFile();

        ReadFile();
        SetValues();

        Screen.SetResolution(xRes, yRes, Fullscreen);

        //Debug.Log("Resolution set to " + xRes + "x" + yRes);

    }

    void GetValues()
    {
        
        Fullscreen = FullscreenButton.On;

        string[] R = Resolution.text.Split('x');

        xRes = int.Parse(R[0]);
        yRes = int.Parse(R[1]);
    }
}
