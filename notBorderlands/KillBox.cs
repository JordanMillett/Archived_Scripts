using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    void OnTriggerEnter(Collider obj)
    {
        if(obj.GetComponent<PlayerStats>() != null)
        {

            obj.GetComponent<PlayerStats>().Damage(obj.GetComponent<PlayerStats>().Health * 2f);
        }
    }
}
