using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : MonoBehaviour
{
    GameObject Door;

    public float Speed;
    public bool Open = false;
    bool Activated = false;
    float lerp;

    void Start()
    {
        Door = this.transform.GetChild(0).gameObject;
    }

    void FixedUpdate()
    {

        if(Activated)
        {
            lerp += Time.fixedDeltaTime/Speed;

            if(Open)
            {

                Door.transform.localRotation = Quaternion.Lerp(Door.transform.localRotation, Quaternion.Euler(-90f,0f,-90f), lerp);
                

            }else
            {
                Door.transform.localRotation = Quaternion.Lerp(Door.transform.localRotation, Quaternion.Euler(-90f,0f,0f), lerp);
            }

        }

    }

    public void Toggle()
    {

        if(!Activated)
        {
            lerp = 0;
            Open = !Open;
            Activated = true;
            StartCoroutine(Finished());
        }

    }

    IEnumerator Finished()
    {

        while (lerp < 1)
        {
            yield return null;
        }
        
        Activated = false;

    }
    
}
