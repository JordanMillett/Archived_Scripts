using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : MonoBehaviour
{
    public bool canPickup = true;
    GameObject Player;

    void Start()
    {

        Player = GameObject.FindWithTag("Player");

    }

    public void Pickup()
    {
        if(canPickup)
        {
            Player.GetComponent<PlayerController>().Pickup(this.gameObject);
        }

    }
}
