using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrigger : MonoBehaviour
{
    //Take bluePart as script input
    //make mesh collider as trigger
    //scale by slight amount
    //get trigger objects all in one frame
    //send data to bluepart
    //delete self

    public BluePart BP;

    List<BluePart> FoundNeighbors = new List<BluePart>();

    float Scale = 1.1f;
    
    void Start()
    {
        Invoke("Despawn", 2f);
        this.transform.localScale = new Vector3(Scale, Scale, Scale);
        MeshCollider MC = this.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        Rigidbody R = this.gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
        R.isKinematic = true;
        MC.convex = true;
        MC.sharedMesh  = BP.GetComponent<MeshCollider>().sharedMesh ;
        MC.isTrigger = true;
    }
    
    void OnTriggerEnter(Collider C)
    {
        
        try
        {
            BluePart Part = C.GetComponent<BluePart>();
            
            if(Part != null && Part != BP)
            {
                FoundNeighbors.Add(Part);
            }

        }catch{}

    }

    void Despawn()
    {
        Debug.Log(FoundNeighbors.Count);
        BP.Neighbors = FoundNeighbors;
        DestroyImmediate(this.gameObject);
    }
}
