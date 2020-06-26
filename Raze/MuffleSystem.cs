using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuffleSystem : MonoBehaviour //Raymarching
{
    Transform Camera;
    AudioLowPassFilter Filter;

    Vector3 Direction;
    float Distance;

    void Start()
    {
        Camera = GameObject.FindWithTag("Camera").transform;
        Filter = GetComponent<AudioLowPassFilter>();
    }

    void Update()
    {
        Direction = Camera.position - transform.position;
        Distance = Vector3.Distance(Camera.position, transform.position);

        RaycastHit hit;

        if(Physics.Raycast(transform.position, Direction, out hit, Distance))
        {

            if(hit.transform.gameObject.tag == "Player")
            {

                Filter.cutoffFrequency = 22000.0f;

            }else
            {

                Filter.cutoffFrequency = 4000.0f;

            }

        }

    }
}
