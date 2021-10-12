using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSound : MonoBehaviour
{
    public SoundMaterial SM;

    public float LoudestForce = 1f;
    public float Threshold = 0f;

    float SoundDistance = 25f;

    void OnCollisionEnter(Collision Col)
    {
        if(Col.impulse.magnitude >= Threshold)
        {
            /*
            float Force = Col.impulse.magnitude - Threshold;
            float Alpha = Force/LoudestForce;

            if(Alpha > 1f)
                Alpha = 1f;
                        
            float Volume = Mathf.Lerp(0.5f, 1f, Alpha);
            */
            float Volume = 1f;

            if(Vector3.Distance(Col.contacts[0].point, GameObject.FindWithTag("Manager").GetComponent<Manager>().GetListenerTransform().position) < SoundDistance)
                SpawnSound(Col.contacts[0].point, Volume);
        }
    }

    void SpawnSound(Vector3 _position, float _volume)
    {
        GameObject Sound = new GameObject();
        Sound.transform.position = _position;
        Despawn DSpawn = Sound.AddComponent(typeof(Despawn)) as Despawn;

        AudioClip Clip = SM.ImpactSounds[Random.Range(0, SM.ImpactSounds.Count)];

        DSpawn.DespawnTime = Clip.length + 0.1f;
        AudioSource AS = Sound.AddComponent(typeof(AudioSource)) as AudioSource;
        AS.spatialBlend = 1f;
        AS.minDistance = 1f;
        AS.maxDistance = SoundDistance;
        AS.rolloffMode = AudioRolloffMode.Linear;
        AS.clip = Clip;

        AS.volume = (_volume * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);
                
        AS.Play();
    }
}