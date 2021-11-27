using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public MeshRenderer M;
    public GameObject Child;

    public void SetLocation(Vector3 Loc)
    {
        M.enabled = true;
        Child.SetActive(true);
        this.transform.position = Loc;
    }

    public void Clear()
    {
        M.enabled = false;
        Child.SetActive(false);
    }
}
