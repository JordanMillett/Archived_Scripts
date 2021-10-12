using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusStop : MonoBehaviour
{
    public int StopNumber = 0;

    PlayerController Player;

    IEnumerator WaitForLoadCoroutine;
    bool isWaitingForLoad = false;

    IEnumerator LoadCoroutine;
    bool isLoading = false;

    public List<Passenger> NPCs = new List<Passenger>();

    public GameObject NPCPrefab;
    public Transform[] Spawns;
    public Transform GetOff;
    public Transform DeleteLocation;

    public int LoadedAmount = 0;
    public int PaidCount = 0;

    //public bool OnCooldown = false;

    public bool Closed = false;

    public Light BusLight;

    public void GenerateNPCs()
    {
        for(int i = 0; i < NPCs.Count; i++)
        {
            Destroy(NPCs[i].gameObject);
        }
        NPCs.Clear();

        if(!Closed)
        {
            BusLight.color = Color.white;

            int RandomAmount = Random.Range(1, Spawns.Length);

            for(int i = 0; i < RandomAmount; i++)
            {
                GameObject NewNPC = Instantiate(NPCPrefab, Spawns[i].transform.position, Spawns[i].transform.rotation);
                NPCs.Add(NewNPC.GetComponent<Passenger>());
            }
        }else
        {
            BusLight.color = Color.red;
        }
    }

    IEnumerator WaitForLoad()
    {
        isWaitingForLoad = true;
        yield return new WaitForSeconds(3f);   //Delay between stopped and loading
        Debug.Log("Waiting Finished");
        Player.BusStopped = true;

        Player.isUnloading = true;      //unload
        Player.StartUnload(this);
        while(Player.isUnloading)
        {
            yield return null;
        }

        isLoading = true;               //load
        LoadCoroutine = Load();
        StartCoroutine(LoadCoroutine);
        while(isLoading)
        {
            yield return null;
        }

        Debug.Log("Loading Finished");
        Player.BusStopped = false;
        isWaitingForLoad = false;


        Closed = true;
        //OnCooldown = true;
        BusLight.color = Color.red;
        /*
        OnCooldown = true;
        yield return new WaitForSeconds(15f); //Cooldown to reset

        GenerateNPCs();

        OnCooldown = false;
        */

    }

    IEnumerator Load()
    {
        Debug.Log("Now Loading");

        int FreeSeats = Player.Seats.Length - Player.CurrentPassengers.Count;
        int StartSeat = Player.CurrentPassengers.Count;

        int StartCount = FreeSeats < NPCs.Count ? FreeSeats : NPCs.Count;

        List<Passenger> Disembarked = new List<Passenger>();

        for(int i = 0; i < StartCount; i++)
        {
            NPCs[NPCs.Count - i - 1].Load(this, Player, StartSeat + i);
            Disembarked.Add(NPCs[NPCs.Count - i - 1]);
            while(PaidCount != (i + 1))
            {
                yield return null;
            }
        }
        
        while(LoadedAmount != StartCount)
        {
            yield return null;
        }
        LoadedAmount = 0;
        PaidCount = 0;
       
        for(int i = 0; i < Disembarked.Count; i++)
        {
            NPCs.Remove(Disembarked[i]);
        }

        isLoading = false;
    }

    void OnTriggerEnter(Collider Col)
    {
        if(Col.transform.root.GetComponent<PlayerController>() != null)
        {
            if(Player == null)
                Player = Col.transform.root.GetComponent<PlayerController>();

            //if(!OnCooldown && !Closed)
            if(!Closed)
            {
                if(Aligned(Player.transform))
                {
                    if(isWaitingForLoad)
                        StopCoroutine(WaitForLoadCoroutine);

                    WaitForLoadCoroutine = WaitForLoad();
                    StartCoroutine(WaitForLoadCoroutine);
                }
            }
        }
    }

    public bool Aligned(Transform T)
    {
        //Debug.Log(Vector3.Angle(T.forward, -this.transform.right));

        if(Vector3.Angle(T.forward, -this.transform.right) < 40f) //def 25f
        {
            return true;
        }

        return false;
    }

    void OnTriggerExit(Collider Col)
    {
        if(Col.transform.root.GetComponent<PlayerController>() != null)
        {
            //Debug.Log(StopNumber + " Exited");
            if(isWaitingForLoad)
                StopCoroutine(WaitForLoadCoroutine);
        }
    }

    /*

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(GetComponent<BoxCollider>().center + this.transform.position, GetComponent<BoxCollider>().center + this.transform.position + (-this.transform.right * 5f));

        Gizmos.color = Color.red;
        if(Player != null)
        {
            Gizmos.DrawLine(GetComponent<BoxCollider>().center + this.transform.position, GetComponent<BoxCollider>().center + this.transform.position + (Player.transform.forward * 5f));
        }
    }

    */
}
