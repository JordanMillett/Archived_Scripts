using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Vehicle Config", menuName = "Vehicle Config")]
public class VehicleConfig : ScriptableObject
{
    public string VehicleName;
    public int VehicleBaseValue;
    public int Gears;
    public int SittingAnimationIndex;
    public float Turn;
    public float PitchHigh;
    public float PitchLow;

    public List<EngineKit> InstallableKits;
    public List<VehiclePart> InstallableParts;
}