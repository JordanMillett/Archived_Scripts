using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artillery : MonoBehaviour
{
    public float Duration = 4f;

    void Start()
    {
        Invoke("Shoot", Random.Range(3f, 6f));
    }

    void Shoot()
    {
        transform.GetChild(0).gameObject.GetComponent<AutoShooter>().Shoot = true;
        Despawn D = this.gameObject.AddComponent(typeof(Despawn)) as Despawn;
        D.DespawnTime = Duration;
    }
}
