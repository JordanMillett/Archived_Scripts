using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class E_Timer : MonoBehaviour
{
    TextMeshProUGUI Text;

    void OnEnable()
    {
        Text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        Text.text = Game.GetTimeFormatted(Time.time - Game.RunData.StartTime);
    }
}
