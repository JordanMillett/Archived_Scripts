using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FleetNumber : MonoBehaviour
{
    public FleetInfo FI;
    TextMeshProUGUI Count;

    void Start()
    {
        Count = GetComponent<TextMeshProUGUI>();
        Count.text = FI.Ships.Count.ToString();
    }

    void OnEnable()
    {
        Count = GetComponent<TextMeshProUGUI>();
        Count.text = FI.Ships.Count.ToString();
    }
}
