using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    public float Speed = 0.01f;
    public List<Vector3> Path;
    
    int NextLocation = 0;

    Vector3 CurrentLocation;
    float LerpAlpha = 0;

    void Start()
    {
        CurrentLocation = this.transform.position;
        //Look at destination
    }

    void Update()
    {
        //CurrentLocation = this.transform.position;

        transform.LookAt(Path[NextLocation]);

        CurrentLocation = Vector3.Lerp(CurrentLocation, Path[NextLocation], LerpAlpha);

        if(LerpAlpha <= 1f)
        {
            LerpAlpha += Speed;
        }else
        {
            
            LerpAlpha = 0f;

            if(NextLocation == Path.Count - 1)
                NextLocation = 0;
            else
                NextLocation++;

        }
        

        



        /*if(Input.GetMouseButton(1) && !Reloading)
            {
                
                if(ADSAlpha < 1f)
                    ADSAlpha += 0.02f;
            }else
            {
                CurrentLocation = Vector3.Lerp(CurrentLocation, Vector3.zero, 1f - ADSAlpha);
                if(ADSAlpha > 0f)
                    ADSAlpha -= 0.02f;
            }*/



        this.transform.position = CurrentLocation;
    }
}
