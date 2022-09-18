using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject ProjectilePrefab;
    public Transform FirePos;
    
    public void Fire(Ray R)
    {
        RaycastHit hit;
        if(Physics.Raycast(R, out hit, 500f))
        {
            Vector3 dir = (hit.point - FirePos.position).normalized;
            GameObject Proj = Instantiate(ProjectilePrefab, FirePos.position, Quaternion.identity); //Quaternion.LookRotation(dir, Vector3.up)
            Proj.GetComponent<Rigidbody>().AddForce(dir * 7500f);
        }
    }
}
