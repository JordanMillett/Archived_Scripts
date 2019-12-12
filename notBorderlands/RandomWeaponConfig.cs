using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class RandomWeaponConfig : MonoBehaviour
{

    TextAsset Adj;
    string AdjPath = "Assets/Resources/Adj.txt";
    TextAsset Noun;
    string NounPath = "Assets/Resources/Noun.txt";
    
    float ValueMult = 1f;

    WeaponConfig WC;

    public WeaponConfig GetRandomWC()
    {

        Adj = (TextAsset)Resources.Load("Adj");
        Noun = (TextAsset)Resources.Load("Noun");

        WC = ScriptableObject.CreateInstance<WeaponConfig>();

        WC.Name = GetRandomName();

        

        WC.Damage = Mathf.Round(Random.Range(2f,10f) * 10f) / 10f;
        WC.Accuracy = Mathf.Round(Random.Range(96f,100f) * 10f) / 10f;
        WC.RPM = Mathf.Round(Random.Range(600f,1500f) / 10f) * 10f;
        WC.MagSize = Random.Range(8,61);

        WC.Velocity = 100f;
        WC.Range = 100f;
        WC.RecoilAmount = 1f;
        WC.Force = 100f;
        WC.ReloadTime = 2f;
        WC.Bullets = 1;

        WC.Automatic = (Random.value > 0.5f);

        WC.Value = CalculateValue();

        return WC;
        

    }

    int CalculateValue()
    {

        float GunValue = 0f;

        GunValue += ((WC.Damage - 2f) / (10f - 2f)) * 10f * ValueMult;
        GunValue += ((WC.Accuracy - 96f) / (100f - 96f)) * 10f * ValueMult;
        GunValue += ((WC.RPM - 600f) / (1500f - 600f)) * 10f * ValueMult;
        GunValue += ((WC.MagSize - 8) / (61 - 8)) * 10f * ValueMult;

        return Mathf.RoundToInt(GunValue);

    }

    string GetRandomName()
    {
        string A = "ERROR";
        string B = "ERROR";

        string[] lines;

        StreamReader AdjReader = new StreamReader(AdjPath);
        lines = Adj.text.Split("\n"[0]);
        A = lines[Random.Range(0, lines.Length)];
        
        char firstLetter = A[0];
        firstLetter = char.ToUpper(firstLetter);
        A = A.Substring(1);
        A = firstLetter + A;

        A = A.Replace("\r","");
        AdjReader.Close();

        StreamReader NounReader = new StreamReader(NounPath);
        lines = Noun.text.Split("\n"[0]);
        B = lines[Random.Range(0, lines.Length)];
        B = B.Replace("\r","");
        NounReader.Close();

        return A + " " + B;

    }
}
