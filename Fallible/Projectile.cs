using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Projectile : MonoBehaviour
{
    public GameObject DecalPrefab;
    public Health Target;
    public int Damage;

    public void Travel(Vector3 D, Vector3 N, float V, bool MakeDecal, Color HitColor)
    {
        StartCoroutine(TravelLoop(D, N, V, MakeDecal, HitColor));
    }

    IEnumerator TravelLoop(Vector3 D, Vector3 N, float V, bool MakeDecal, Color HitColor)
    {   
        float Distance = Vector3.Distance(this.transform.position, D);

        while(Distance > 0.1f)
        {
            yield return null;
            this.transform.position = Vector3.MoveTowards(this.transform.position, D, Time.deltaTime * V);
            Distance = Vector3.Distance(this.transform.position, D);
        }

        if(Target != null)
        {
            Target.TakeDamage(Damage);
        }

        if(MakeDecal)
        {
            GameObject Dec = Instantiate(DecalPrefab, D, Quaternion.identity);
            Dec.transform.rotation = Quaternion.LookRotation(N, Vector3.up);
            Dec.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.x, this.transform.localScale.x);
            Dec.transform.GetChild(0).GetComponent<VisualEffect>().SetVector4("Color", HitColor);
            Debug.DrawRay(Dec.transform.position, N * 0.25f, Color.blue, 1f);
        }

        Destroy(this.gameObject);
    }
}