using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage_Part : MonoBehaviour
{
    public Damage Link;
    public bool Critical = false;

    public void Hurt(int Amount, bool AntiArmor, bool PlayerDealt)
    {
        Link.Hurt(Amount, AntiArmor, Critical, PlayerDealt);
    }
}
