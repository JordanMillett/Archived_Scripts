using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MapGenerator
{
    public bool Editing = false;
    public Tile.Types Paint = Tile.Types.Field;

    public Tiles TileData;
    public Vector2Int Offset = Vector2Int.zero;

    public void ToDefaults()
    {
        Editing = false;
        Paint = Tile.Types.Field;
        Offset = Vector2Int.zero;

        if (Generated)
            UnloadMap();
    }
    
    public void LoadMap()
    {
        if(!MapData.TileData.IsEmpty())
        {
            TileData = new Tiles(MapData.TileData);
            GenerateMap();
        }else
        {
            Debug.LogError("Cannot load an empty map.");
        }
    }
    
    public void UnloadMap()
    {
        TileData.Clear();
        
        DestroyMap();
    }
    
    public void Save()
    {
        if(TileData.IsEmpty())
        {
            Debug.LogError("Cannot save an empty map.");
            return;
        }
        
        MapData.Save(TileData, Offset);
    }
    
    public void Set(int x, int y)
    {
        if(InBounds(x, y))
        {
            TileData.Column[x].Row[y] = Paint;
        }else
        {
            Vector2Int dim = TileData.GetDimensions();
            
            if(x == dim.x)
            {
                TileData.AppendColumn();
                TileData.Column[x].Row[y] = Paint;
            }
            
            if(x == -1)
            {
                TileData.PushColumn();
                TileData.Column[0].Row[y] = Paint;
                Offset += new Vector2Int(-1, 0);
            }
            
            if(y == dim.y)
            {
                TileData.AppendRow();
                TileData.Column[x].Row[y] = Paint;
            }
            
            if(y == -1)
            {
                TileData.PushRow();
                TileData.Column[x].Row[0] = Paint;
                Offset += new Vector2Int(0, -1);
            }
        }

        GenerateMap(TileData, Offset);
    }
    
    public bool InBounds(int x, int y)
    {
        Vector2Int dim = TileData.GetDimensions();
        
        return x > -1 && x < dim.x && y > -1 && y < dim.y;
    }
    
    public bool IsCorner(int x, int y)
    {
        Vector2Int dim = TileData.GetDimensions();

        return (x == dim.x && y == dim.y) || (x == dim.x && y == -1) || (x == -1 && y == dim.y) || (x == -1 && y == -1);
    }
}
