using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveHalf : MonoBehaviour
{
    void Start()
    {
        int Delete = Random.Range(0,2);

        if(Delete == 0)
            Destroy(this.gameObject);
    }
}
