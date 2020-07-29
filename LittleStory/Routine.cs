using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Routine : MonoBehaviour
{
    [System.Serializable]
    public struct Destination
    {
	    public Vector3 Position;
        public Vector3 Rotation;
        public Vector2 WaitRange;
        public int AnimationIndex;
    }

    public float Maxspeed = 5f;
    public float TurnSpeed = 1f;
    public bool Paused = false;
    public bool Frozen = false;
    public int CurrentIndex = 0;
    public float DistanceThreshold = 0.5f;
    public List<Destination> Destinations;

    Vector3 Target = Vector3.zero;

    Rigidbody r;

    void Start()
    {
        r = GetComponent<Rigidbody>();
        StartCoroutine(Walk());
    }

    IEnumerator Walk()
    {
        while(true)
        {
            if(Vector3.Distance(this.transform.position, Destinations[CurrentIndex].Position) > DistanceThreshold)
            {
                yield return null;
                Target = Destinations[CurrentIndex].Position - this.transform.position;
            }else
            {
                Paused = true;
                yield return new WaitForSeconds(Random.Range(Destinations[CurrentIndex].WaitRange.x,Destinations[CurrentIndex].WaitRange.y));
                if(CurrentIndex == Destinations.Count - 1)
                    CurrentIndex = 0;
                else
                    CurrentIndex++;
                Paused = false;
            }
        }
    }

    void FixedUpdate()
    {
        if(!Paused && !Frozen)
        {
            if(Target != Vector3.zero)
            {
                Quaternion newRotation = Quaternion.LookRotation(Target, Vector3.up);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, Time.fixedDeltaTime * TurnSpeed);
            }//this.transform.rotation = Quaternion.LookRotation(Target, Vector3.up);  //Look at target
            if(r.velocity.magnitude < Maxspeed)     //Walk forward
            {
                r.velocity += this.transform.forward;
            }
        }
    }

}
