using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyCounter : MonoBehaviour
{
    
    public TextMeshProUGUI Counter;

    void Start()
    {

        Counter.text = transform.childCount.ToString();

    }

    void Update()
    {

        Counter.text = transform.childCount.ToString();

    }


}
