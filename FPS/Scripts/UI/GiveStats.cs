using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveStats : MonoBehaviour
{
    public Stats P;
    public Stats S;
    public Stats G;

    void Start()
    {
        P.Guns = this.transform.GetChild(0).GetComponentsInChildren<Weapon>();
        S.Guns = this.transform.GetChild(1).GetComponentsInChildren<Weapon>();
        G.Grenades = this.transform.GetChild(2).GetComponentsInChildren<Explode>();
        //P.W = this.transform.GetChild(0).GetChild(0).GetComponent<Weapon>();
        //S.W = this.transform.GetChild(1).GetChild(0).GetComponent<Weapon>();
        //G.E = this.transform.GetChild(2).GetChild(0).GetComponent<Explode>();
    }

}
