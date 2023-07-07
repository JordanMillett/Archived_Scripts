using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePart : MonoBehaviour
{
    public Life _life;
    public int Multiplier = 1;

    public void Hurt(int Amount)
    {
        _life.Hurt(Amount * Multiplier);
    }
}
