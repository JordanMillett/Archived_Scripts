using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour
{
    Rigidbody r;
    public Transform Seat;
    GameObject Player;

    public bool inUse = false;

    public int SittingAnimationIndex = 1;

    Vector3 ExitLocation = Vector3.zero;

    bool canLeave = false;

    void Start()
    {
        Player = GameObject.FindWithTag("Player").gameObject;
        r = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(inUse && canLeave)
        {
            if(Input.GetKeyDown(KeyCode.F))
                Toggle();
        }
    }

    IEnumerator LeaveCooldown()
    {
        canLeave = false;

        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;

        canLeave = true;
    }

    public void Toggle()
    {
        inUse = !inUse;

        StartCoroutine(LeaveCooldown());
        
        Player.GetComponent<Rigidbody>().isKinematic = inUse;
        Player.GetComponent<Player>().ToggleColliders();
        if(inUse)
            ExitLocation = Player.transform.position - this.transform.position;

        Player.transform.position = Seat.position;
        Player.transform.rotation = Seat.rotation;

        if(inUse)
        {
            Player.GetComponent<Player>().An.SetInteger("Sitting", SittingAnimationIndex);
            Player.transform.SetParent(this.transform);
        }
        else
        {
            Player.GetComponent<Player>().An.SetInteger("Sitting", 0);
            Player.transform.SetParent(null);
            Player.transform.position = this.transform.position + ExitLocation;
        }
        
    }
}
