using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despawn : MonoBehaviour
{

    public float DespawnTime;

    void Start()
    {

        StartCoroutine(Delete(this.transform.gameObject, DespawnTime));

    }

    IEnumerator Delete(GameObject Gam, float Delay)
    {
        yield return new WaitForSeconds(Delay);
        try{
            Destroy(Gam);
        }catch {}

    }
}
