using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Vehicle Part", menuName = "Vehicle Part")]
public class VehiclePart : ScriptableObject
{
    public string PartName;
    public GameObject PartPrefab;
    public Location PartLocation;
    public int Cost;

    public enum Location
	{
		Front,
		Middle,
		End
	};
}