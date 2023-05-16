using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
    using UnityEditor;
#endif

[System.Serializable]
public class Tiles
{
    public List<Rows> Column = new List<Rows>();
    Tile.Types Fill = Tile.Types.Ocean;

    public Tiles(Tiles copy)
    {
        Vector2Int dim = copy.GetDimensions();
        
        for (int x = 0; x < dim.x; x++)
        {
            Rows data = new Rows(dim.y, Fill);
            
            for (int y = 0; y < dim.y; y++)
            {
                data.Row[y] = copy.Column[x].Row[y];
            }
            
            this.Column.Add(data);
        }
    }
    
    public Tiles()
    {
        Initialize(1, 1, Fill);
    }
    
    public Tiles(int xDim, int yDim)
    {
        Initialize(xDim, yDim, Fill);
    }
    
    public Tiles(int xDim, int yDim, Tile.Types fill)
    {
        Initialize(xDim, yDim, fill);
    }
    
    public void Initialize(int xDim, int yDim, Tile.Types fill)
    {
        for (int x = 0; x < xDim; x++)
        {
            Rows data = new Rows(yDim, Fill);
            
            this.Column.Add(data);
        }
    }
    
    public void AppendRow() //Down
    {
        for (int x = 0; x < GetDimensions().x; x++)
        {
            this.Column[x].Row.Add(Fill);
        }
    }
    
    public void PushRow() //Up
    {
        for (int x = 0; x < GetDimensions().x; x++)
        {
            this.Column[x].Row.Insert(0, Fill);
        }
    }
    
    public void AppendColumn() //Right
    {
        Rows data = new Rows(GetDimensions().y, Fill);
        
        this.Column.Add(data);
    }
    
    public void PushColumn() //Left
    {
        Rows data = new Rows(GetDimensions().y, Fill);
        
        this.Column.Insert(0, data);
    }
    
    public Vector2Int GetDimensions()
    {
        if(this.Column.Count == 0)
            return Vector2Int.zero;
        else
            return new Vector2Int(this.Column.Count, this.Column[0].Row.Count);
    }
    
    public bool IsEmpty()
    {
        return GetDimensions() == Vector2Int.zero;
    }
    
    public void Clear()
    {
        this.Column.Clear();
    }
}

[System.Serializable]
public class Rows
{
    public List<Tile.Types> Row = new List<Tile.Types>();
    
    public Rows(int size, Tile.Types fill)
    {
        for (int y = 0; y < size; y++)
        {
            this.Row.Add(fill);
        }
    }
}

[CreateAssetMenu(fileName ="New Map", menuName = "Map")]
public class Map : ScriptableObject
{
    public string MapName = "New Map";

    public Vector2Int Dimensions = Vector2Int.zero;
    public Vector2Int Offset = Vector2Int.zero;
    public Tiles TileData = new Tiles();

    void OnValidate()
    {
        Dimensions = TileData.GetDimensions();
    }
    
    public void Save(Tiles data, Vector2Int offset)
    {
        Offset = offset;
        TileData = new Tiles(data);

        OnValidate();

        Debug.Log("Saved " + MapName);

        #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        #endif
        
        //TEMP SAVE TO TEST JSON
        Directory.CreateDirectory(Application.persistentDataPath + "/mods");
        
        string ModPath = Application.persistentDataPath + "/mods/" + MapName + ".json";
        string json = JsonUtility.ToJson(this);
        File.WriteAllText(ModPath, json);
    }
}
