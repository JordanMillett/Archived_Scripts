using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationDirector : MonoBehaviour
{
    [HideInInspector]
    public List<Room> Rooms;

    public Faction[] Factions = new Faction[3];

    public Loot StartingWeapons;

    public const int RECORD_LENGTH = 60;

    private int powerAvailable = 0;
    public int PowerAvailable { get { return powerAvailable; } set { powerAvailable = Mathf.Max(value, 0); RecalculateSystems(); } }
    
    [HideInInspector]
    public int StartingRooms = 0;

    public int EnemyLevel = 1;
    //public int EnemyStartingLevel = 1;

    Vector3 SpawnPosition = Vector3.zero;

    public void Entry(Vector3 _SpawnPosition)
    {
        SpawnPosition = _SpawnPosition;
        StartingRooms = Rooms.Count;
        PowerAvailable = StartingRooms;

        StartCoroutine(Begin());
    }
    
    IEnumerator Begin()
    {
        yield return new WaitForSeconds(0.25f);

        EnemyLevel = Game.StationData.EnemyStartingLevel;

        Game.StartRun(SpawnPosition);

        Player.P.PickupItem(StartingWeapons.Items[Random.Range(0, StartingWeapons.Items.Count)]);
        
        InvokeRepeating("RampUp", Game.StartingGrace + Game.RampUpTime, Game.RampUpTime);
        InvokeRepeating("SpawnEnemy", Game.StartingGrace, Game.EnemySpawnInterval);
    }
    
    void RampUp()
    {
        EnemyLevel++;
        PowerAvailable--;
    }
    
    void SpawnEnemy()
    {
        GameObject NewEnemy = GameObject.Instantiate(GetEnemyPrefab(), FindSpawnPosition(), Quaternion.identity);
        NewEnemy.GetComponent<Enemy>().Level = EnemyLevel;
    }
    
    GameObject GetEnemyPrefab()
    {
        float TimePassed = Time.time - Game.RunData.StartTime;

        int RandomFaction = Random.Range(0, Factions.Length);
        int RandomType = Random.Range(0, 3);

        //if(TimePassed > 60 && Random.value > 0.5f)
        
        if(RandomType == 0)
            return Factions[RandomFaction].RegularPrefab;
        if(RandomType == 1)
            return Factions[RandomFaction].RusherPrefab;
        return Factions[RandomFaction].SniperPrefab;
        
    }
    
    Vector3 FindSpawnPosition()
    {
        List<Room> Valid = new List<Room>();

        foreach(Room R in Rooms)
        {
            if(Vector3.Distance(R.transform.position, Player.P.transform.position) > 75f)
                Valid.Add(R);
        }

        int Index = Random.Range(0, Valid.Count);

        return Valid[Index].SpawnPositions[Random.Range(0, Valid[Index].SpawnPositions.Count)] + Valid[Index].transform.position;
    }
    
    void RecalculateSystems()
    {
        int PowerUsed = 0;

        foreach (Room R in Rooms)
            if (R.Power)
                PowerUsed++;
        
        if(PowerAvailable != PowerUsed)
            if(PowerAvailable <= Rooms.Count)
                RecalculatePower(PowerAvailable - PowerUsed);
    }
    
    void RecalculatePower(int Difference)
    {   
        bool PowerDeficit = Difference < 0;     //60 - 80   need to shut off 20 rooms
        for (int i = 0; i < Mathf.Abs(Difference); i++)
        {
            int PathChosen = PowerDeficit ? 0 : 1000;
            int RoomChosen = 0;
            for (int x = 0; x < Rooms.Count; x++)
            {
                if(PowerDeficit)
                {
                    if (Rooms[x].Power)
                    {
                        if (Rooms[x].PathNumber >= PathChosen)   //if path is less important
                        {
                            PathChosen = Rooms[x].PathNumber;
                            RoomChosen = x;
                        }
                    }
                }else
                {
                    if (!Rooms[x].Power)
                    {
                        if (Rooms[x].PathNumber <= PathChosen)   //if path is more important
                        {
                            PathChosen = Rooms[x].PathNumber;
                            RoomChosen = x;
                        }
                    }
                }
            }
            
            for (int x = 0; x < Rooms.Count; x++)
            {
                if (Rooms[x].PathNumber == PathChosen)
                {
                    if(PowerDeficit)
                    {
                        if(Rooms[x].Power)
                            if (Rooms[x].PathIndex >= Rooms[RoomChosen].PathIndex)
                                RoomChosen = x;
                    }else
                    {
                        if(!Rooms[x].Power)
                            if (Rooms[x].PathIndex <= Rooms[RoomChosen].PathIndex)
                                RoomChosen = x;
                    }
                }
            }
            
            //Debug.Log("Path - " + PathChosen + ", Index - " + Rooms[RoomChosen].PathIndex + ", Room - " + RoomChosen);

            Rooms[RoomChosen].Power = PowerDeficit ? false : true;
        }
    }
}
