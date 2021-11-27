using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    LineRenderer LR;

    public int LineLength = 20;
    public int MaxLines = 4;

    public float LightningLifeTime = 0.5f;

    public float RaycastAngle = 10f;

    public List<AudioClip> ThunderImpacts;

    void Start()
    {
        LR = GetComponent<LineRenderer>();
        LR.positionCount = 0;

        Strike();
    }

    void Strike()
    {
        List<Vector3> Positions = new List<Vector3>();
        Positions.Add(this.transform.position);

        for(int i = 0; i < MaxLines; i++)
        {
            Vector3 OffsetVector = Random.insideUnitSphere.normalized * RaycastAngle;
            Positions.Add(Positions[i] + ((Vector3.up + OffsetVector) * LineLength));
        }

        GameObject Sound = new GameObject();
        Sound.transform.position = this.transform.position;
        Despawn DSpawn = Sound.AddComponent(typeof(Despawn)) as Despawn;
        AudioClip Clip = ThunderImpacts[Random.Range(0, ThunderImpacts.Count)];
        DSpawn.DespawnTime = Clip.length + 0.1f;
        AudioSource AS = Sound.AddComponent(typeof(AudioSource)) as AudioSource;
        AS.spatialBlend = 1f;
        AS.volume = (1f * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);
        AS.maxDistance = 3000f;
        AS.clip = Clip;
        AS.Play();

        Vector3[] LinePositions = new Vector3[Positions.Count];

        for(int i = 0; i < LinePositions.Length; i++)
        {
            LinePositions[i] = Positions[i];
        }

        LR.positionCount = Positions.Count;
        LR.SetPositions(LinePositions);

        Invoke("Hide", LightningLifeTime);
    }

    void Hide()
    {
        Destroy(this.gameObject);
        //LR.positionCount = 0;
    }
}
