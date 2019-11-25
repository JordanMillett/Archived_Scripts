using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour
{

    public float Delay;
    public float Radius;
    public float Magnitude;
    public float Damage;

    public GameObject Effects;

    public List<AudioClip> ExplodeSounds;

    public bool Multiply = false;

    void Start()
    {
        Invoke("Detonate", Delay);
    }

    void Detonate()
    {

        //Checksphere in range
        //add force to all rigidbodies with direction vector away from origin of explosion
        //falloff?

        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, Radius/3f);

        int i = 0;
        while (i < hitColliders.Length)
        {
            if(hitColliders[i].transform.gameObject.GetComponent<LifeManager>() != null)
            {
                float percent = (Vector3.Distance(this.transform.position, hitColliders[i].transform.position))/(Radius/3f);
                if(percent > 1)
                    percent = 1;
                
                float newDamage = Damage * (1 - percent);
                hitColliders[i].transform.gameObject.GetComponent<LifeManager>().Damage(Mathf.Round(newDamage));
            }

            if(hitColliders[i].GetComponent<Rigidbody>() != null)
            {
                hitColliders[i].GetComponent<Rigidbody>().isKinematic = false;
                Vector3 dir = hitColliders[i].transform.position - this.transform.position;

                float percent = (Vector3.Distance(this.transform.position, hitColliders[i].transform.position))/(Radius/3f);
                if(percent > 1)
                    percent = 1;
                float newForce = Magnitude * (1 - percent);

                hitColliders[i].GetComponent<Rigidbody>().AddForce(dir.normalized * newForce);
            }

            i++;
        }

        GameObject E = Instantiate(Effects, this.transform.position, Quaternion.identity);
        E.GetComponent<Explosion>().Radius = Radius;

        GameObject Sound = new GameObject();
        Sound.transform.position = this.transform.position;
        //Sound = Instantiate(Sound, this.transform.position, Quaternion.identity);
        Despawn DSpawn = Sound.AddComponent(typeof(Despawn)) as Despawn;
        DSpawn.DespawnTime = 6f;
        AudioSource AS = Sound.AddComponent(typeof(AudioSource)) as AudioSource;
        AS.spatialBlend = .6f;
        AS.clip = GetExplodeSound();
        AS.Play();

        if(Multiply)
        {

            Instantiate(this.transform.gameObject, this.transform.position, Quaternion.identity);
            Instantiate(this.transform.gameObject, this.transform.position, Quaternion.identity);

        }

        Destroy(this.transform.gameObject);

    }

    AudioClip GetExplodeSound()
    {

        int Index = Random.Range(0, ExplodeSounds.Count);

        return ExplodeSounds[Index];

    }

}
