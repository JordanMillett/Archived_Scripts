using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatSync : MonoBehaviour
{
    public LifeManager LM;

    TextMeshProUGUI Health;
    TextMeshProUGUI Shield;

    void Start()
    {

        Health = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Shield = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

    }

    void Update()   //Remove Update and make it only update when needed
    {

        Health.text = LM.Health.ToString();
        Shield.text = LM.Shields.ToString();

    }


}
