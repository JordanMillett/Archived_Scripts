using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Zone
{
    Crew,
    Cargo,
    Medical,
    Lab,
    Vault,
    Armory,
    Power,
    Escape
}

public class Room : MonoBehaviour
{
    private bool power = true;
    public bool Power { get { return power; } set { power = value; PowerUpdated(); } }
    
    public LootConfiguration GoodLoot;
    public Loot BadLoot;
    public Zone Type = Zone.Crew;
    public List<Container> Containers;
    public List<Light> Lights;
    public List<Vector3> BlockedDoorways;
    public List<Vector3> SpawnPositions = new List<Vector3>{Vector3.zero};

    [HideInInspector]
    public int PathIndex = 0;
    [HideInInspector]
    public int PathNumber = 0;
    [HideInInspector]
    public List<Wall> Walls;
    [HideInInspector]
    public bool Discovered = false;
    [HideInInspector]
    public List<Vector2Int> Pixels;

    BoxCollider Bounds;

    void Start()
    {
        Bounds = GetComponent<BoxCollider>();

        //foreach(Container C in Containers)
        if (Containers.Count > 0)
        {
            if(Type != Zone.Armory)
            {
                if (Random.value < 0.75f)
                    FillContainer(Containers[Random.Range(0, Containers.Count)]);
            }else
            {
                foreach(Container C in Containers)
                    if (Random.value < 0.35f)
                        FillContainer(C);
            }
            
            
        }

        if(Type == Zone.Escape)
        {
            Discovered = true;
            foreach(Vector2Int Pixel in Pixels)
                UIManager.UI.M_Game.E_Minimap.DiscoverRoom(Pixel.x, Pixel.y, Type);
        }
    }

    void Update()
    {
        if (Bounds.bounds.Contains(Player.P.transform.position))
            if((int) Type >= 3)
                Player.P.Freezing = false;
            else
                Player.P.Freezing = !Power;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach(Vector3 Doorway in BlockedDoorways)
            Gizmos.DrawSphere(Doorway, 1f);
            
        Gizmos.color = Color.blue;
        foreach(Vector3 SpawnPosition in SpawnPositions)
            Gizmos.DrawSphere(SpawnPosition, 1f);
    }

    void PowerUpdated()
    {
        if((int) Type >= 3)
            return;
        
        foreach(Light _light in Lights)
            _light.color = Power ? Color.white : Color.red;
    }
    
    public void Explode()
    {
        for (int i = 0; i < Walls.Count; i++)
        {
            if (Walls[i].Owners.Count == 1)
            {
                Destroy(Walls[i].gameObject);
            }else
            {
                Walls[i].Owners.Remove(this);
            }
        }
        Destroy(this.gameObject);
    }
    
    void FillContainer(Container C)
    {
        for (int i = 0; i < 4; i++)
        {
            Item I = GoodLoot.GetLoot();
            if(I == null)
                I = BadLoot.Items[Random.Range(0, BadLoot.Items.Count)];

            C.Contents[i] = I;
        }

        C.Activate();
    }
    
    void OnTriggerEnter(Collider col)
    {
        if(Discovered)
            return;

        try{
            Player P = col.transform.root.transform.gameObject.GetComponent<Player>();
            if (P)
            {
                Discovered = true;
                foreach(Vector2Int Pixel in Pixels)
                    UIManager.UI.M_Game.E_Minimap.DiscoverRoom(Pixel.x, Pixel.y, Type);
            }
        }catch{}
    }
}
