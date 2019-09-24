using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    GameObject Side_1;
    GameObject Side_2;

    public float Speed;
    public bool Open = false;
    bool Activated = false;
    float lerp;

    void Start()
    {
        Side_1 = this.transform.GetChild(0).gameObject;
        Side_2 = this.transform.GetChild(1).gameObject;
    }

    void FixedUpdate()
    {

        if(Activated)
        {
            lerp += Time.fixedDeltaTime/Speed;

            if(Open)
            {
    
                Side_1.transform.localPosition = Vector3.Lerp(Side_1.transform.localPosition, new Vector3(1.4f,0f,0f), lerp);
                Side_2.transform.localPosition = Vector3.Lerp(Side_2.transform.localPosition, new Vector3(-1.4f,0f,0f), lerp);
                

            }else
            {
                Side_1.transform.localPosition = Vector3.Lerp(Side_1.transform.localPosition, new Vector3(.5f,0f,0f), lerp);
                Side_2.transform.localPosition = Vector3.Lerp(Side_2.transform.localPosition, new Vector3(-.5f,0f,0f), lerp);
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
