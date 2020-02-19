using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AreaTrigger : MonoBehaviour
{
    public UnityEvent Function;

    void OnTriggerEnter(Collider Col)
    {
        if(Col.gameObject.CompareTag("Player"))
            Activate();
    }

    public void Activate()
    {
        Function.Invoke();

        Destroy(this.gameObject);
    }
}
