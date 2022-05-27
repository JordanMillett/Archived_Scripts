using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public bool Mine = false;
    public float ForceNeeded = 25f;

    public GameObject Prefab;
    public float EffectSize = 4f;
    public float ExplosionSize = 2f;
    public int ExplosionDamage = 100;
    public float SoundVolume = 1f;

    public bool PlayerOwned = false;

    bool Armed = false;

    void Start()
    {
        Invoke("Arm", 2f);
    }

    void Arm()
    {
        Armed = true;
    }

    void OnCollisionEnter(Collision Col)
    {
        if(Col.impulse.magnitude > ForceNeeded && Armed)
        {
            GameObject Dec = Instantiate(Prefab, this.transform.position, Quaternion.identity);
            if(Col.contacts[0].normal != Vector3.zero)
                Dec.transform.rotation = Quaternion.LookRotation(Mine ? transform.up : Col.contacts[0].normal, Vector3.up);

            Dec.GetComponent<Explosion>().Explode(EffectSize, ExplosionSize, ExplosionDamage, SoundVolume, PlayerOwned);

            Destroy(this.transform.gameObject);
        }
    }
}
