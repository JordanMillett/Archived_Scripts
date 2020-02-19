using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flicker : MonoBehaviour
{
    public float Max = 35f;
    public float Min = 30f;
    public float Speed = 1f;

    Light L;

    float Noise = 0f;
    float TimeAlive = 0f;

    float Seed = 0f;

    void Start()
    {
        Seed = Random.Range(0f, 250f);
        TimeAlive += Seed;
        L = GetComponent<Light>();
    }

    void Update()
    {
        TimeAlive += Time.fixedDeltaTime;
        Noise = Mathf.PerlinNoise(TimeAlive * Speed, 0f);
        L.intensity = Mathf.Lerp(Min, Max, Noise);
    }
}
