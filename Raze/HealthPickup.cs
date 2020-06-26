using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{

    public AudioClip PickupSound;

    public float Amount;

    void OnTriggerEnter(Collider Col)
    {
        if(Col.GetComponent<PlayerController>() != null)
        {
            if(GameObject.FindWithTag("Player").GetComponent<LifeManager>().AddHealth(Amount))
            {
                GameObject Sound = new GameObject();
                Sound.transform.position = this.transform.position;
                Despawn DSpawn = Sound.AddComponent(typeof(Despawn)) as Despawn;
                DSpawn.DespawnTime = 1f;
                AudioSource AS = Sound.AddComponent(typeof(AudioSource)) as AudioSource;
                AS.spatialBlend = 1f;
                AS.volume = 0.5f;
                AS.clip = PickupSound;
                AS.Play();

                Destroy(this.gameObject);
            }
        }
    }

}
