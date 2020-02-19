using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    Animator An;

    void Start()
    {

        An = transform.GetChild(0).GetComponent<Animator>();

    }

    public void Damage(float Amount)
    {

        StartCoroutine(AnimateHurt());

        //Health subtraction here

    }

    IEnumerator AnimateHurt()
    {

        An.SetInteger("hitState", 1);

        yield return null;

        An.SetInteger("hitState", 0);

    }
}
