using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public int Value;
    public float DissolveSpeed = 0.01f;
    public MeshRenderer Mesh;

    public void Collect()
    {
        
        StartCoroutine(PlayEffect());

    }

    IEnumerator PlayEffect()
    {
        Hands H = GameObject.FindWithTag("Hands").GetComponent<Hands>();
        
        while(H.Busy)
        {

            yield return null;

        }

        float alpha = 0f;
        while(alpha < 1f)
        {
            Mesh.material.SetFloat("_ClipLerpAlpha", alpha);
            alpha += DissolveSpeed;
            yield return null;
        }

        H.DeleteActiveItem();    
    }

}
