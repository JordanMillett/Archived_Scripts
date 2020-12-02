/*

    public void GenerateNeighbors()
    {
        GameObject Empty = new GameObject(this.gameObject.name + "_Trigger");
        Empty.transform.position = this.transform.position;
        Empty.transform.rotation = this.transform.rotation;

        MeshTrigger MT = Empty.AddComponent(typeof(MeshTrigger)) as MeshTrigger;
        MT.BP = this;
    }


    bool foundPath = false;             //There is no path right now
    while(foundPath == false)           //Do this while there is no path
    {
        foundPath = AdvanceIndex();     //check if the next route is avaliable
        if(foundPath == false)          //there is no next path
        {
            foundPath = true;           //break out of statement
            Break();                    //break the piece for it is no longer connected
        } 
    }

    void Update()
    {
        if(Broken)
        {
            if(!Ren.isVisible)
            {
                Destroy(this.gameObject);
            }
        }
    }

    //float alpha = (float)Distances[CurrentDistanceIndex]/(float)BiggestDistance;
    //Gizmos.color = Color.Lerp(Color.green, Color.red, alpha);
    //Gizmos.color = Color.green;
    
    int CloserRootIndex()
    {
        for(int i = 0; i < Neighbors.Count; i++)
        {
            for(int j = 0; j < TotalRoots; j++)
            {
                if(Neighbors[i].Distances[j] < Distances[j]) 
                {
                    return j;           //this is the new root path to use
                }
            }
        }
        
        return -1;                      //no neighbors are closer than this part
    }

    bool AdvanceIndex()          //used to check if there is a next route
    {
        CurrentDistanceIndex = CloserRootIndex();
        if(OutOfPaths())
            return false;       //No neighbors are closer than this part is

         return true;            //This route index works
    }
*/

