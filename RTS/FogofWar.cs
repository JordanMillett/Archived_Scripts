﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogofWar : MonoBehaviour
{
    public float xSize;
    public float ySize;
    public int xNum;
    public int yNum;

    //public float Height

    float xTile;
    float yTile;

    public GameObject FogPrefab;

    void Start()
    {
        xTile = xSize / xNum;
        yTile = ySize / yNum;

        Init();
    }

    void Init()
    {
        /* 
        float xOffset = 0;
        float yOffset = 0;

        for(int x = 0; x < xSize; x++)
        {
            for(int y = 0; y < xSize; y++)
            {
                GameObject Tile = Instantiate(FogPrefab, this.transform.position + new Vector3(xOffset,0f,yOffset), Quaternion.identity);
                Tile.Name = x + " " + y;
                //Change size of collider and first child
                Tile.GetComponent<Collider>().SizeDelta(xTile, 1f, yTile);
                Tile.transform.GetChild(0).transform.SizeDelta(xTile, 1f, yTile);
                yOffset += yTile;
            }
            xOffset += xTile;
        }
        */

    }

}