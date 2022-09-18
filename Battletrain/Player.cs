using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Camera C;
    public Train T;
    public float MoveSpeed = 6f;
    public bool OnGun = false;
    
    void Update()
    {
        if(!OnGun)
        {
            if(Input.GetKey("a") && this.transform.localPosition.x > -4.5f)
            {
                this.transform.Translate(-transform.right * MoveSpeed * Time.deltaTime, Space.World);
            }
            if(Input.GetKey("d") && this.transform.localPosition.x < 9.5f)
            {
                this.transform.Translate(transform.right * MoveSpeed * Time.deltaTime, Space.World);
            }
        }
        
        if(Input.GetMouseButton(0) && OnGun)
        {
            T.ActiveGun.Fire(C.ScreenPointToRay(Input.mousePosition));
        }else if(Input.GetMouseButtonDown(0) && !OnGun)
        {
            T.ActiveGun = GetNearbyGun();
            if(T.ActiveGun)
                OnGun = true;
        }
        
        if(Input.GetMouseButtonDown(1) && OnGun)
        {
            OnGun = false;
            T.ActiveGun = null;
        }else if(Input.GetMouseButtonDown(1) && !OnGun)
        {
            Debug.Log("REPAIRING");
        }
    }
    
    Gun GetNearbyGun()
    {
        Collider[] Nearby = Physics.OverlapSphere(this.transform.position, 1.5f);
        foreach(Collider Col in Nearby)
        {
            try 
            {   
                Gun G = Col.transform.gameObject.GetComponent<Gun>();

                if(G != null)
                    return G;
            }
            catch{}
        }
        return null;
    }
}
