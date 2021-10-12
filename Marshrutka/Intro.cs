using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    public RawImage Fade;
    public List<AudioClip> Noises;
    AudioSource AS;

    void Start()
    {
        AS = GetComponent<AudioSource>();
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        int Alpha = 0;
        Color FadeColor;
        int Frames = 60;
        while(Alpha < Frames)
        {
            FadeColor = Fade.color;
            FadeColor.a = Mathf.Lerp(1f, 0f, (float)Alpha/(float) Frames);
            Fade.color = FadeColor;
            Alpha++;
            yield return new WaitForSeconds(0.01f);
        }
        FadeColor = Fade.color;
        FadeColor.a = 0f;
        Fade.color = FadeColor;

        yield return new WaitForSeconds(0.5f);

        int Children = this.transform.childCount;
        float MinPitch = 0.75f;
        float MaxPitch = 1.5f;
        int Index = 0;

        foreach(Transform Child in this.transform)
        {
            Child.GetComponent<Rigidbody>().isKinematic = false;
            SpawnSound(Child.position, 0.1f, Mathf.Lerp(MinPitch, MaxPitch, ((float)Index/(float)Children)));
            Index++;
            yield return new WaitForSeconds(0.01f);
        }

        Alpha = 0;
        while(Alpha < Frames)
        {
            FadeColor = Fade.color;
            FadeColor.a = Mathf.Lerp(0f, 1f, (float)Alpha/(float) Frames);
            Fade.color = FadeColor;
            Alpha++;
            yield return new WaitForSeconds(0.01f);
        }
        FadeColor = Fade.color;
        FadeColor.a = 1f;
        Fade.color = FadeColor;

        yield return new WaitForSeconds(0.25f);

        SceneManager.LoadScene(1);
    }

    void SpawnSound(Vector3 _position, float _volume, float _pitch)
    {
        GameObject Sound = new GameObject();
        Sound.transform.position = _position;
        Despawn DSpawn = Sound.AddComponent(typeof(Despawn)) as Despawn;

        AudioClip Clip = Noises[Random.Range(0, Noises.Count)];

        DSpawn.DespawnTime = Clip.length + 0.1f;
        AudioSource AS = Sound.AddComponent(typeof(AudioSource)) as AudioSource;
        AS.spatialBlend = 0f;
        AS.minDistance = 1f;
        AS.maxDistance = 25f;
        AS.pitch = _pitch;
        AS.clip = Clip;

        AS.volume = (_volume * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);
                
        AS.Play();
    }
}