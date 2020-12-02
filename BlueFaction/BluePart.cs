using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluePart : MonoBehaviour
{

    public List<BluePart> Neighbors;
    public bool EditorMode = false;
    public bool GroundPart = false;

    public List<int> Distances;
    public int CurrentDistanceIndex = 0;
    public List<bool> PathUnavailable;
    public int PartIndex = 0;

    int TotalRoots;

    bool Broken = false;
    Renderer Ren;

    float MinDespawnTime = 5f;
    float MaxDespawnTime = 10f;

    void Start()
    {
        Ren = this.gameObject.GetComponent<MeshRenderer>();
        if(Distances.Count > 0)
        {
            CurrentDistanceIndex = RecalculateIndex();
            if(OutOfPaths())  //No paths are free thus it's floating
                Break();
        }
    }

    public void Init(int Roots, int Index)     //init distance array
    {
        TotalRoots = Roots;
        PartIndex = Index;
        for(int i = 0; i < TotalRoots; i++)
        {
            Distances.Add(-1);
            PathUnavailable.Add(false);
        }
    }

    public void SetDistance(int OriginIndex, int NewDist)       //algorithm to calculate all distances
    {

        Distances[OriginIndex] = NewDist;

        for(int i = 0; i < Neighbors.Count; i++)
        {
            if(Neighbors[i].Distances[OriginIndex] == -1)
            {
                Neighbors[i].SetDistance(OriginIndex, Distances[OriginIndex] + 1);
            }else if(Neighbors[i].Distances[OriginIndex] > Distances[OriginIndex] + 1)
            {
                Neighbors[i].SetDistance(OriginIndex, Distances[OriginIndex] + 1);
            }
        }
    }

    public void RootDestroyed(int Index)
    {
        PathUnavailable[Index] = true;
        CurrentDistanceIndex = RecalculateIndex();
        if(OutOfPaths())  //No paths are free thus it's floating
            Break();
    }

    int RecalculateIndex()              //what path is closest now
    {
        for(int i = 0; i < Distances.Count; i++)  
            if(Distances[i] == -1)    
                PathUnavailable[i] = true;     

        int Lowest = CurrentDistanceIndex;          //default to zero
        for(int i = 0; i < Distances.Count; i++)    //for every path
            if(Distances[i] < Distances[Lowest] && !PathUnavailable[i])    //if a different path is better
                Lowest = i;
        
        return Lowest;
    }

    public void Break()
    {
        if(!Broken)
        {
            Broken = true;
            Rigidbody R = this.gameObject.GetComponent<Rigidbody>();        //Activate physics
            R.isKinematic = false;
            R.gameObject.layer = 8;                                         //non intercolliding layer

            if(GroundPart)
            {
                this.transform.parent.GetComponent<BlueObject>().RootDestroyed(PartIndex);
            }

            for(int i = 0; i < Neighbors.Count; i++)                        //Alert other parts that this is gone
            {
                Neighbors[i].ThisBroke(this, Distances);
            }

            Invoke("Despawn", Random.Range(MinDespawnTime, MaxDespawnTime));//Despawn particle over time
        }    
    }

    public void ThisBroke(BluePart BP, List<int> BrokeDists)
    {
        Neighbors.Remove(BP);                       //The broken object is no longer a neighbor
        if(Neighbors.Count == 0 && !GroundPart)     //if is alone and isn't attached to the ground
        {
            Break();
        }else 
        {
            MarkPaths(BrokeDists);                  //Find paths that are broken now
            RecalculateIndex();                     //Recalculate what path should be used as closest

            if(OutOfPaths())                        //No paths are free thus it's floating/not supported
                Break();                            //break it if there are no paths left
        }
    }

    void MarkPaths(List<int> BrokeDists)            //Find now broken paths
    {
        for(int i = 0; i < Distances.Count; i++)    //for every path
            if(BrokeDists[i] < Distances[i] || Distances[i] == -1)     //if the broken part is closer than the current part on any path
                PathUnavailable[i] = true;          //mark that path as unavailable
    }

    bool OutOfPaths()
    {
        for(int i = 0; i < PathUnavailable.Count; i++)  //for every path bool
            if(PathUnavailable[i] == false)             //if a path is free
                return false;                           //say we're not out of paths

        return true;                                    //else it is true that paths are gone
    }
    
    void OnDrawGizmos()
    {
        if(!Broken && Distances.Count > 0)
        {
            int value = Distances[CurrentDistanceIndex];
            if(value == 0)
                Gizmos.color = Color.cyan;
            if(value == 1)
                Gizmos.color = Color.green;
            if(value == 2)
                Gizmos.color = Color.yellow;
            if(value > 2)
                Gizmos.color = Color.red;
            if(value < 0)
                Gizmos.color = Color.black;

            Gizmos.DrawSphere(this.transform.position, 0.1f);
            
        }
    }

    void Despawn()
    {
        Destroy(this.gameObject);
    }

    void OnCollisionEnter(Collision col) 
    {
        if(EditorMode)
        {
            try
            {
                BluePart Part = col.gameObject.GetComponent<BluePart>();

                if(Part != null)
                {
                    Neighbors.Add(Part);
                }

            }catch{}
        }
    }
}
