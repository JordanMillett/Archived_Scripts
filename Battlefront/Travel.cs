using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Travel : MonoBehaviour
{
    public AIUnit HitTarget;
    public GameObject Decal;
    public Vector3 MoveVector;

    public Color MatColor;
    public float ColorStrength = 3f;

    public AIUnit Creator;

    public int DamageAmount;

    public void Move(Vector3 D, float V, Vector3 N)
    {

        GetComponent<TrailRenderer>().material.SetColor("LaserColor", MatColor * ColorStrength);

        StartCoroutine(Go(D, V, N));

    }

    IEnumerator Go(Vector3 D, float V, Vector3 N)
    {
        float targetDistance = Vector3.Distance(transform.position, D);
        float lerp = 0f;
        Vector3 startPosition = transform.position;

        float stopDistance = targetDistance/100f;

        while(lerp <= stopDistance)
        {
            lerp += V;
            transform.position = Vector3.Lerp(startPosition, startPosition + (MoveVector * 100f), lerp);

            yield return null;
        }

        if(HitTarget != null)
        {
            if(HitTarget.health - DamageAmount <= 0)
                Creator.GetComponent<UnitStats>().Kills++;

            HitTarget.Damage(DamageAmount);
        }

        MakeDecal(D, N);

        Destroy(this.gameObject);

    }

    void MakeDecal(Vector3 D, Vector3 N)
    {

        //Debug.Log(N);

        GameObject Dec = Instantiate(Decal, D, Quaternion.LookRotation(N, Vector3.up));

    }
}
