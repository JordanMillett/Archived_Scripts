using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Tileset", menuName = "Tileset")]
public class Tileset : ScriptableObject
{   
    public GameObject Empty;
    public List<GameObject> CrewRooms;
    public List<GameObject> CargoRooms;
    public List<GameObject> MedicalRooms;
    public List<Vector3> WallPositions;
    
    public struct RoomData
    {
        public GameObject Room;
        public float Spin;
    }
    
    public RoomData MatchingRoom(Zone Type, List<Vector3> Doorways)
    {
        //randomly shuffle list
        RoomData NewData = new RoomData();
        NewData.Room = Empty;
        NewData.Spin = 0f;
        foreach(GameObject Room in Type == Zone.Crew ? CrewRooms : Type == Zone.Cargo ? CargoRooms : MedicalRooms)
        {
            Room R = Room.GetComponent<Room>();
            if (R.BlockedDoorways.Count == 0)
            {
                NewData.Room = Room;
                return NewData;
            }
            else
            {
                int MatchedDoorways = 0;
                for (int i = 0; i < 4; i++)    //for each spin of the room
                {
                    MatchedDoorways = 0;
                    foreach (Vector3 Blocked in R.BlockedDoorways)  //check every blocked doorway
                    {
                        Vector3 RotatedBlocker = Quaternion.Euler(0f, i * 90f, 0f) * Blocked;
                        RotatedBlocker = new Vector3(Mathf.Round(RotatedBlocker.x), Mathf.Round(RotatedBlocker.y), Mathf.Round(RotatedBlocker.z));

                        if (Doorways.Contains(RotatedBlocker))    //if blocked is in the way of actual doorway
                            break;
                        else
                            MatchedDoorways++;
                    }
                    if (MatchedDoorways == R.BlockedDoorways.Count)
                    {
                        NewData.Room = Room;
                        NewData.Spin = i * 90f;
                        return NewData;
                    }
                }
            }
        }
        
        return NewData;
    }
}