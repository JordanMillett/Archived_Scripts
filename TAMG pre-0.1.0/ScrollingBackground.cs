using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    public float Speed = 1f;

    List<Transform> Tracks = new List<Transform>();
    
    void Start()
    {
        foreach (Transform T in transform)
        {
            Tracks.Add(T);
        }
    }

    void Update()
    {
        foreach (Transform T in Tracks)
        {
            T.Translate(Vector3.right * -Speed * Time.deltaTime);
            if(T.position.x <= -96f)
            {
                T.position = new Vector3(864f, 0f , 0f);   
            }
        }
    }
}
