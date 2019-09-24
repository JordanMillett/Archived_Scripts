using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ship_Notifications : MonoBehaviour
{
    Ship S;
    public List<TextMeshProUGUI> TextBoxes = new List<TextMeshProUGUI>();
    int currentPriority = 0;
    int clearTimer = 0;

    void Start()
    {
        S = GameObject.FindWithTag("Ship").GetComponent<Ship>();
    }

    void Update()
    {
        clearTimer++;
        if(clearTimer > 500)
            Clear();
    }

    public void Alert(string Message, int Priority)
    {

        if(Priority >= currentPriority)
        {
            clearTimer = 0;
            currentPriority = Priority;

            foreach(TextMeshProUGUI t in TextBoxes)
            {

                t.text = Message;

            }

        }

    }

    void Clear()
    {

        currentPriority = 0;
        clearTimer = 0;

        foreach(TextMeshProUGUI t in TextBoxes)
        {

            t.text = "";

        }

    }
}
