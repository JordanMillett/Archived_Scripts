using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private bool open = false;
    public bool Open 
    { 
        get { return open; }
        set { open = value; } 
    }

    List<Rigidbody> Nearby = new List<Rigidbody>();

    Transform Model;
    public Wall W;

    public float OpenAmount = 1f;

    void Start()
    {
        Model = this.transform.GetChild(0).transform;
        
    }

    void Update()
    {
        Open = Nearby.Count > 0;

        Model.transform.localPosition = Vector3.Lerp(Model.transform.localPosition, Open ? new Vector3(2.75f, 0f, 0f) : Vector3.zero, Time.deltaTime * 3f);
        OpenAmount = Model.transform.localPosition.x / 2f;
    }
    
    void OnTriggerEnter(Collider Col)
    {
        if(Col.isTrigger)
            return;

        try{
            Rigidbody R = Col.transform.root.transform.gameObject.GetComponent<Rigidbody>();
            Nearby.Add(R);
        }catch{}
    }

    void OnTriggerExit(Collider Col)
    {
        if(Col.isTrigger)
            return;
        
        try{
            Rigidbody R = Col.transform.root.transform.gameObject.GetComponent<Rigidbody>();
            Nearby.Remove(R);
        }catch{}
    }
}
