using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Transform RotEmpty;

    public Transform TrackLocation;
    Transform LookPosition;

    void Start()
    {
        LookPosition = GameObject.FindWithTag("Camera").transform;
    }

    void Update()
    {
        if(TrackLocation != null)
        {
            this.transform.position = TrackLocation.position;
            RotEmpty.LookAt(LookPosition.position);
        }else
        {
            Destroy(this.gameObject);
        }
    }
}
