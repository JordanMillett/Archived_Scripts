using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        if(Input.GetKeyDown("i"))
        {
            Application.targetFrameRate = 25;
        }

        if(Input.GetKeyDown("o"))
        {
            Application.targetFrameRate = 60;
        }
        
        if(Input.GetKeyDown("p"))
        {
            Application.targetFrameRate = 144;
        }
    }
}