using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Openable : MonoBehaviour
{
    public Vector3 ClosedVector;
    public Vector3 OpenVector;

    public float Increment;

    bool isRunning = false;
    bool Open = false;

    void Start()
    {

        ClosedVector = this.transform.eulerAngles;

    }

    public void Toggle()
    {
        if(!isRunning)
            StartCoroutine(Move());

    }

    IEnumerator Move()
    {

        isRunning = true;

        float lerptime = 0f;
        while(lerptime < 1f + Increment)
        {
            yield return null;
            if(!Open)
                this.transform.eulerAngles = Vector3.Lerp(ClosedVector,OpenVector,lerptime);
            else
                this.transform.eulerAngles = Vector3.Lerp(OpenVector,ClosedVector,lerptime);

            lerptime += Increment;
        }
      
        Open = !Open;

        isRunning = false;

    }
}
