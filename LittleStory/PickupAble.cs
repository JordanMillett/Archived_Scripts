using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickupAble : MonoBehaviour
{
    public Vector3 holdPos;
    public Vector3 holdRot;
    public float holdScale;
    public float HoldSpeedMultiplier;
    public bool canDrop = true;

    public UnityEvent RunAfterPickup;
    
    public void Pickup()
    {
        
        if(GameObject.FindWithTag("Hands").GetComponent<Hands>().Pickup(this))
            RunAfterPickup.Invoke();

    }
}
