using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Transmission", menuName = "Transmission")]
public class TransmissionData : ScriptableObject
{
    public string Manufacturer;

    public List<float> Gears;
}
