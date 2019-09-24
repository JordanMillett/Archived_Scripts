using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;

public class PosRotSync : PosRotBehavior
{
    //void Start()
    protected override void NetworkStart()
    {
        Debug.Log("actually started");

        if(!networkObject.IsOwner)
        {

            Rigidbody r = GetComponent<Rigidbody>();
            Destroy(r);

        }
    }

    void Update()
    {
        if(networkObject == null)
            return;

        if(!networkObject.IsOwner)
        {
            this.transform.position = networkObject.Position;
            this.transform.eulerAngles = networkObject.Rotation;
            Debug.Log("Recieved Data");
            return;
        }

        networkObject.Position = this.transform.position;
        networkObject.Rotation = this.transform.eulerAngles;
        Debug.Log("Sent Data");

    }
}
