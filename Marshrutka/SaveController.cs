using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class SaveController : MonoBehaviour
{
    [System.Serializable]
    public struct Stats
    {
	    public int MoneyGained;
        public int GasUsed;
        public int Fines;
        public float TopSpeed;
    }

    [System.Serializable]
    public class Save
    {
        public bool New;
        public bool Cheating;
        public int Money;
        public int Night;
        public List<Stats> NightStats;
    }
    /*
    Save DefaultSave = new Save
    (
        false,
        GameSettings.StartingMoney,
        0,
        new List<Stats>()
    );*/

    public int SaveIndex = 0;
    public List<Save> Saves = new List<Save>();

    public string path = "";

    void Start()
    {
        GameSettings.SC = this;
        path = Application.persistentDataPath + "saves.txt";

        LoadSaves();
    }

    public void LoadSaves()
    {
        if(!File.Exists(path)) 
        {
            MakeNewSaves();
        }

        StreamReader SR = new StreamReader(path);
        string fullText = SR.ReadToEnd();
        SR.Close();

        try
        {
            string[] allLines = fullText.Split("\n"[0]);

            int LineCount = 0;

            if(Application.version != allLines[LineCount])
                throw new Exception("Regenerating Saves, Version out of date");

            LineCount++;
            for(int i = 0; i < 3; i++)
            {
                Saves[i].New = int.Parse(allLines[LineCount]) == 1;
                LineCount++;
                Saves[i].Cheating = int.Parse(allLines[LineCount]) == 1;
                LineCount++;
                Saves[i].Money = int.Parse(allLines[LineCount]);
                LineCount++;
                Saves[i].Night = int.Parse(allLines[LineCount]);
                LineCount++;
            }

        }catch
        {
            Debug.LogError("Error loading saves file, creating new from default.");
            MakeNewSaves();
        }

    }

    void MakeNewSaves()
    {
        Saves.Clear();
        for(int i = 0; i < 3; i++)
        {
            Saves.Add(GetDefaultSave());
        }

        SaveGame();
    }

    public void SaveGame()
    {
        File.WriteAllText(path, "");

        StreamWriter SW = new StreamWriter(path);

        string settingsLines = "";

        settingsLines += Application.version + "\n";

        for(int i = 0; i < 3; i++)
        {
            settingsLines += System.Convert.ToInt32(Saves[i].New) + "\n";
            settingsLines += System.Convert.ToInt32(Saves[i].Cheating) + "\n";
            settingsLines += Saves[i].Money + "\n";
            settingsLines += Saves[i].Night + "\n";
            //Add support for night stats
        }
        
        SW.Write(settingsLines);
        SW.Close();

        LoadSaves();
    }

    public void DeleteSave(int Index)
    {
        Saves[Index] = GetDefaultSave();

        SaveGame();
    }

    Save GetDefaultSave()
    {
        Save NewSave = new Save();
        NewSave.New = true;
        NewSave.Cheating = false;
        NewSave.Money = GameSettings.StartingMoney;
        NewSave.Night = 0;
        NewSave.NightStats = new List<Stats>();

        return NewSave;
    }

    public bool GetNew(){return Saves[SaveIndex].New;}
    public void SetNew(bool _New){Saves[SaveIndex].New = _New;}

    public bool GetCheating(){return SaveIndex != -1 ? Saves[SaveIndex].Cheating : false;}
    public void SetCheating(bool _Cheating){Saves[SaveIndex].Cheating = _Cheating;}

    public int GetMoney(){return Saves[SaveIndex].Money;}
    public void SetMoney(int _Money){Saves[SaveIndex].Money = _Money;}

    public int GetNight(){return Saves[SaveIndex].Night;}
    public void SetNight(int _Night){Saves[SaveIndex].Night = _Night;}

    public Stats GetNightStats(int Index){return Saves[SaveIndex].NightStats[Index];}
    public void AddNightStats(Stats NewStats){Saves[SaveIndex].NightStats.Add(NewStats);}
}

    