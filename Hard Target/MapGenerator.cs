using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class MapGenerator : MonoBehaviour
{
    public Map MapData;
    public Tileset TileSet;

    public bool Generated = false;

    //public Dictionary<Vector2Int, GameObject> References = new Dictionary<Vector2Int, GameObject>();
    public List<GameObject> References = new List<GameObject>();

    public void GenerateMap()
    {
        GenerateMap(MapData.TileData, MapData.Offset);
    }
    
    public void GenerateMap(Tiles _tiles, Vector2Int _offset)
    {
        if(References.Count != 0)
            ClearReferences();
            
        Vector3 offset = new Vector3(_offset.x, 0f, _offset.y);
        Vector2Int dim = _tiles.GetDimensions();

        for (int x = 0; x < dim.x; x++)
        {
            for (int y = 0; y < dim.y; y++)
            {
                #if UNITY_EDITOR
                GameObject Tile;
                
                if(!EditorApplication.isPlaying)
                    Tile = (GameObject) PrefabUtility.InstantiatePrefab(TileSet.Tiles[(int) _tiles.Column[x].Row[y]]) as GameObject;
                else
                    Tile = GameObject.Instantiate(TileSet.Tiles[(int) _tiles.Column[x].Row[y]]);
                #else
                    GameObject Tile = GameObject.Instantiate(TileSet.Tiles[(int) _tiles.Column[x].Row[y]]);
                #endif
                
                Tile.transform.position = this.transform.position;
                Tile.transform.rotation = this.transform.rotation;
                Tile.transform.SetParent(this.transform);
                Tile.transform.localScale = this.transform.localScale;

                Tile.transform.localPosition = new Vector3(x, 0f, y) + offset;

                //References.Add(new Vector2Int(x, y) + _offset, Tile);
                References.Add(Tile);

            }
        }
        
        Generated = true;
    }
    
    public void DestroyMap()
    {
        ClearReferences();
        Generated = false;
    }
    
    void ClearReferences()
    {
        List<GameObject> Refs = new List<GameObject>();
        
        foreach(GameObject Obj in References)
        {
            Refs.Add(Obj);
        }
        
        /*
        foreach(Vector2Int key in References.Keys)
        {
            Refs.Add(References[key]);
        }*/
        
        for (int i = 0; i < Refs.Count; i++)
        {
            DestroyImmediate(Refs[i]);
        }

        References.Clear();
    }
}
