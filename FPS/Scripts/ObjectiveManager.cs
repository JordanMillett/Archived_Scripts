using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{    

    int Enemies = 0;
    public bool HostageCaptured = false;

    float Timetaken = 0;

    int Won = 0;

    void Start()
    {

    }

    void Update()
    {
        if(Won == 0)
            Timetaken += Time.deltaTime;
    }

    public void Win()
    {   
        Won = 1;
        Debug.Log("Won!");
        Debug.Log("Time Taken : " + Timetaken);
        GameObject.FindWithTag("Pause").GetComponent<UIController>().Win(Won, Timetaken);
        Destroy(this);
    }

    public void Lost()
    {

        Won = -1;
        Debug.Log("Lost.");
        Debug.Log("Time Taken : " + Timetaken);
        GameObject.FindWithTag("Pause").GetComponent<UIController>().Win(Won, Timetaken);
        Destroy(this);

    }
}
