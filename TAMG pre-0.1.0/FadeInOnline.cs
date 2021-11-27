using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOnline : MonoBehaviour
{
    public float AppearTime = 2f;
    public List<MeshRenderer> MeshRends;
    //public List<SkinnedMeshRenderer> MeshRends;

    void Start()
    {
        Invoke("Appear", AppearTime);
    }

    void Appear()
    {
        //foreach(MeshRenderer )
    }
}
