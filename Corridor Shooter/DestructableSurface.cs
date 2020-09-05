using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableSurface : MonoBehaviour
{
    public int CubeAmount = 4;
    public GameObject CubePrefab;

    Material OriginalMaterial;
    List<Vector2> OriginalUVS = new List<Vector2>();

    void Start()
    {
        OriginalMaterial = GetComponent<MeshRenderer>().material;
        GetComponent<MeshFilter>().mesh.GetUVs(0, OriginalUVS);
    }

    public void Activate()
    {
        float Interval = transform.localScale.x; //Always a cube
        Interval = Interval/CubeAmount;

        Vector3 StartingCorner = new Vector3
        (
            transform.position.x - transform.localScale.x/2f,
            transform.position.y - transform.localScale.x/2f,
            transform.position.z - transform.localScale.x/2f
        );
        
        for(int x = 0; x < CubeAmount; x++)
        {
            for(int y = 0; y < CubeAmount; y++)
            {
                for(int z = 0; z < CubeAmount; z++)
                {
                    Vector3 InitPos = new Vector3
                    (
                    StartingCorner.x + ((Interval/2f) + (Interval * x)),
                    StartingCorner.y + ((Interval/2f) + (Interval * y)),
                    StartingCorner.z + ((Interval/2f) + (Interval * z))
                    );
                    GameObject C = Instantiate(CubePrefab, InitPos, this.transform.rotation);
                    C.transform.localScale = new Vector3(Interval, Interval, Interval);
                    C.GetComponent<MeshFilter>().mesh.SetUVs(0, UpdatedUVS(new Vector3(x,y,z)));
                    C.GetComponent<MeshRenderer>().material = OriginalMaterial;
                }
            }
        }
        Destroy(this.gameObject);
    }

    List<Vector2> UpdatedUVS(Vector3 Position)
    {
        List<Vector2> newUVS = new List<Vector2>();
        
        for(int i = 0; i < OriginalUVS.Count; i++)
        {
            newUVS.Add(OriginalUVS[i]);
            //newUVS[i] *= 1f/CubeAmount;
        }

        return newUVS;
    }
}
