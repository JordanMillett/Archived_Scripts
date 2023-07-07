using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    MeshRenderer M;
    MeshRenderer MM;
    public Package Active;

    void Start()
    {
        M = GetComponent<MeshRenderer>();
        MM = this.transform.GetChild(0).GetComponent<MeshRenderer>();
    }

    public void SetLocation(Vector3 Loc)
    {
        M.enabled = true;
        MM.enabled = true;
        this.transform.position = Loc;
    }

    public void Clear()
    {
        M.enabled = false;
        MM.enabled = false;
    }
}
