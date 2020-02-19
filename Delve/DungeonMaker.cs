using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMaker : MonoBehaviour
{

    public GameObject RoomEmpty;

    public GameObject HallPrefab;
    public GameObject CornerPrefab;
    public GameObject FinalPrefab;

    public Vector2Int HallAmounts;
    public Vector2Int HallLengths;


    Vector3 Pointer = Vector3.zero;

    int Increment = 12;

    Vector3 Direction;

    bool North = true;

    void Start()
    {
        Direction = new Vector3(0f,0f, Increment);

        int Halls = Random.Range(HallAmounts.x,HallAmounts.y);

        for(int i = 0; i < Halls; i++)
        {
            
            int Distance = Random.Range(HallLengths.x, HallLengths.y);//randomly spawn in a turn and then continue going to make hall snake around in one direction

            for(int j = 0; j < Distance; j++)
            {
                Pointer += Direction;

                GameObject Room = Instantiate(HallPrefab, Pointer, Quaternion.identity);
                Room.transform.parent = RoomEmpty.transform;
            }

            if(North)
            {
                Pointer += new Vector3(0f,0f, Increment);
                GameObject Turn1 = Instantiate(CornerPrefab, Pointer, Quaternion.Euler(0,-90f,0));
                Turn1.transform.parent = RoomEmpty.transform;
                Pointer += new Vector3(-Increment,0f, 0f);
                GameObject Turn2 = Instantiate(CornerPrefab, Pointer, Quaternion.Euler(0,-180f,0));
                Turn2.transform.parent = RoomEmpty.transform;
                Direction = new Vector3(0f,0f, -Increment);
                North = !North;
            }else
            {

                Pointer += new Vector3(0f,0f, -Increment);
                GameObject Turn3 = Instantiate(CornerPrefab, Pointer, Quaternion.Euler(0,0f,0));
                Turn3.transform.parent = RoomEmpty.transform;
                Pointer += new Vector3(-Increment,0f, 0f);
                GameObject Turn4 = Instantiate(CornerPrefab, Pointer, Quaternion.Euler(0,90f,0));
                Turn4.transform.parent = RoomEmpty.transform;
                Direction = new Vector3(0f,0f, Increment);
                North = !North;

            }



        }

  

        Pointer += Direction;
        if(North)
        {
            GameObject lastRoom = Instantiate(FinalPrefab, Pointer, Quaternion.Euler(0,180,0));
            lastRoom.transform.parent = RoomEmpty.transform;
        }else
        {
            GameObject lastRoom = Instantiate(FinalPrefab, Pointer, Quaternion.Euler(0,0f,0));
            lastRoom.transform.parent = RoomEmpty.transform;
        }
    }

}
