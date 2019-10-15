using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public float Duration;

    void Start()
    {
        StartCoroutine(Despawn());
    }

    IEnumerator Despawn()
    {

        yield return new WaitForSeconds(Duration);
        Destroy(this.gameObject.transform.GetChild(0).gameObject);

    }

}
