using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoShooter : MonoBehaviour
{
    public WeaponInfo PrimaryInfo;

    public bool Shoot = false;

    [HideInInspector]
    public Weapon Equipped;

    void Start()
    {
        if(PrimaryInfo != null)
            Equipped = PrimaryInfo.CreateWeapon(this.transform, false);
    }

    void Update()
    {
        if(Shoot)
            Equipped.PullTrigger();
    }
}
