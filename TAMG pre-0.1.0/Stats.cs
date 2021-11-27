using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Stats : MonoBehaviour
{
    public TextMeshProUGUI Balance;

    void Update()
    {
        Balance.text = "$" + PlayerInfo.Balance.ToString();
    }
}
