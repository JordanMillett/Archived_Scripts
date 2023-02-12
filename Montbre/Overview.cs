using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overview : MonoBehaviour
{
    public Camera C;
    
    void OnEnable()
    {
        C.cullingMask += LayerMask.GetMask("MapView");
    }
    
    void OnDisable()
    {
        C.cullingMask -= LayerMask.GetMask("MapView");
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if(Physics.Raycast(C.ScreenPointToRay(Input.mousePosition), out hit, 2000f, LayerMask.GetMask("MapView")))
            {
                if(hit.transform.root.gameObject.GetComponent<Unit>())
                    if(hit.transform.root.gameObject.GetComponent<Unit>().Targetable)
                        if(hit.transform.root.gameObject.GetComponent<Unit>().Controllable)
                            if(hit.transform.root.gameObject.GetComponent<Unit>().Team == Game.TeamOne)
                                C.transform.GetComponent<Player>().TakeControlBruh(hit.transform.root.gameObject.GetComponent<Unit>());
            }
        }
    }
}
