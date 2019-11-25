using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hostage : MonoBehaviour
{
    public void Pickup()
    {
        GameObject.FindWithTag("Player").GetComponent<ObjectiveManager>().HostageCaptured = true;

        Destroy(this.transform.gameObject);
    }
}
