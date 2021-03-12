using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartFunction : MonoBehaviour
{
    //enum for what the thing does, change carry, change lights, etc

    public MeshRenderer PartMesh;
    public BoxCollider CarryBounds;

    public void Activate(VehicleController VC)
    {
        if(CarryBounds != null)
            VC.CarryBounds = CarryBounds;
    }
}
