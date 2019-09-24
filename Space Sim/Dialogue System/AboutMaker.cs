using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AboutMaker : MonoBehaviour
{

    public NPC P;
    string Output;

    void Start()
    {

        TextMeshProUGUI T = GetComponentInChildren<TextMeshProUGUI>();
        Output = "My name is " + P.Name + " and I work " + P.Location + ".";
        T.text = Output;

    }
}
