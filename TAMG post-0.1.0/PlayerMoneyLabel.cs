using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMoneyLabel : MonoBehaviour
{
    TextMeshProUGUI Title;

    void Start()
    {
        Title = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        
        Title.text = "$" + Client.Instance.SessionSave.Balance.ToString();
    }
}
