using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManagerNetwork : NetworkBehaviour
{
    [Command]
    public void SpawnVehicleAtLocation(GameObject Vehicle,Vector3 Location)
    {
        GameObject Car = Instantiate(Vehicle, Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(Car);
        Car.transform.position = GameServer.GS.GetNearestVehicleSpawn(Location).position;
        Car.transform.rotation = GameServer.GS.GetNearestVehicleSpawn(Location).rotation;
    }
}
