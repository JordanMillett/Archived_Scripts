using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_Controller : MonoBehaviour
{
    public GameObject SpawnPoint;
    public GameObject Customer;
    public GameObject[] Seats;
    public bool[] Occupied;
    public float Delay;
    public int OccupiedSeats = 0;

    void Start()
    {

        FindSeats();
        InvokeRepeating("SpawnCustomer",Delay,Delay);

    }

    void FindSeats()
    {

        Seats = GameObject.FindGameObjectsWithTag("Seat");
        Occupied = new bool[Seats.Length];
        for(int i = 0; i < Occupied.Length;i++)
            Occupied[i] = false;

    }

    void SpawnCustomer()
    {
        if(OccupiedSeats != Seats.Length)
        {
            GameObject C = Instantiate(Customer,SpawnPoint.transform.position,Quaternion.identity);
            C.GetComponent<Customer>().Seat_Location = GetSeat();
        }

    }

    Transform GetSeat()
    {
        
        
            int Index = Random.Range(0,Seats.Length);

            while(Occupied[Index] == true)
            {
                Index = Random.Range(0,Seats.Length);
            }

            Occupied[Index] = true;
            OccupiedSeats++;

            Transform Loc = Seats[Index].transform;

            return Loc;
        

    }
}
