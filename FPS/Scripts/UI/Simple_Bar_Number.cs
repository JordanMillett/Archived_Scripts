using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Simple_Bar_Number : MonoBehaviour
{

    public Simple_Bar B;

    TextMeshProUGUI T;


    void Start()
    {
        T = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        T.text = B.Current.ToString();
    }
}

