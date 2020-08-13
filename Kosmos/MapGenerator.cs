using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Vector2 MapSize;

    public GameObject EnemyPrefab;
    public GameObject PlanetPrefab;

    public List<Ship> AllShips;

    void Start()
    {
        for(int x = Mathf.RoundToInt(-MapSize.x/2f); x < Mathf.RoundToInt(MapSize.x/2f); x++)
        {
            for(int y = Mathf.RoundToInt(-MapSize.y/2f); y < Mathf.RoundToInt(MapSize.y/2f); y++)
            {
                float R = Random.Range(0f, 100f);
                if(R < 1f)
                {
                    GenerateObject(x, y);
                }
            }
        }
    }

    void GenerateObject(int xLoc, int YLoc)
    {
        float R = Random.Range(0f, 100f);

        if(R < 15f)
        {

            GameObject Enemy = Instantiate(EnemyPrefab, new Vector3(xLoc, 0f, YLoc), Quaternion.identity);
            List<Ship> NewShips = new List<Ship>();

            int ShipCount = Random.Range(1, 40);

            for(int i = 0; i < ShipCount; i++)
            {
                int ShipIndex = Random.Range(0, AllShips.Count);
                NewShips.Add(AllShips[ShipIndex]);
            }

            Enemy.GetComponent<FleetInfo>().Ships = NewShips;
            Enemy.GetComponent<ShipController>().Speed = Random.Range(1f, 3f);
            Enemy.transform.SetParent(this.transform);

        }else if(R < 25f)
        {

            GameObject Planet = Instantiate(PlanetPrefab, new Vector3(xLoc, 0f, YLoc), Quaternion.identity);
            Planet.transform.SetParent(this.transform);
            Planet.GetComponent<Planet>().ShipPossibilities = AllShips;

        }
    }
}
