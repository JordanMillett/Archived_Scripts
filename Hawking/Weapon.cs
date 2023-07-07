using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public WeaponStats Stats;

    float lastFired = 0f;

    public Vector3 OffsetPos;
    public Vector3 ADSPos;

    public GameObject DecalPrefab;

    Vector3 SwaySeed;

    bool Dropped = true;

    void Start()
    {
        SwaySeed = new Vector3(Random.Range(0f, 1000f), Random.Range(0f, 1000f), Random.Range(0f, 1000f));
    }

    void FixedUpdate()
    {
        if(!Dropped)
            UpdateModel();
    }

    void UpdateModel()
    {
        float RecoveryAlpha = Mathf.Clamp((Time.time - lastFired)/(60f/Stats.RPM), 0, 1);//Time.fixedDeltaTime * 5f before

        float NoiseSpeed = Time.time * 0.5f;
        Vector3 Noise = new Vector3
        (
            Mathf.PerlinNoise(NoiseSpeed + SwaySeed.x, NoiseSpeed + SwaySeed.z) - 0.5f, 
            Mathf.PerlinNoise(NoiseSpeed + SwaySeed.y, NoiseSpeed + SwaySeed.x) - 0.5f,
            Mathf.PerlinNoise(NoiseSpeed + SwaySeed.z, NoiseSpeed + SwaySeed.y) - 0.5f
        );

        Vector3 PosSway = Noise;
        PosSway.y -= 0.5f;
        PosSway.y *= 0.5f;
        PosSway *= 0.01f;
        
        this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, PosSway, RecoveryAlpha);
        
        Vector3 RotSway = Noise;
        RotSway.x -= 0.5f;
        RotSway.x *= 0.5f;
        RotSway *= 2f;
        
        this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(RotSway), RecoveryAlpha);
    }

    void Recoil()
    {
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, -Stats.BackRecoil);
        this.transform.localEulerAngles += new Vector3(Random.Range(-1f, -1.5f) * Stats.TurnRecoil, Random.Range(-0.5f, 0.5f) * Stats.TurnRecoil, Random.Range(-0.5f, 0.5f) * Stats.TurnRecoil);
        UpdateModel();
    }
    
    public void Pickup()
    {
        if(Player.Instance.Equipped)
            Player.Instance.Equipped.Drop();

        this.transform.SetParent(Player.Instance.Hands);
        GetComponent<MeshCollider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        this.gameObject.layer = 7;
        this.transform.localEulerAngles = Vector3.zero;
        this.transform.localPosition = Vector3.zero;
        Player.Instance.Equipped = this;

        Dropped = false;
    }
    
    public void Drop()
    {
        Player.Instance.Equipped = null;
        this.transform.SetParent(null);
        GetComponent<MeshCollider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        this.transform.rotation = Player.Instance._camera.rotation;
        this.transform.position = Player.Instance._camera.position + (Player.Instance._camera.up * -0.3f);
        GetComponent<Rigidbody>().velocity = Player.Instance.transform.forward * 2f;
        this.gameObject.layer = 6;
        
        Dropped = true;
    }
    
    public bool CanFire()
    {
        return Time.time > lastFired + (60f/Stats.RPM);
    }

    public void Fire(Transform Origin)
    {
        if(!CanFire())
            return;

        lastFired = Time.time;

        for (int i = 0; i < Stats.Shots; i++)
        {

            Vector3 AimVector = Random.insideUnitSphere.normalized * Random.Range(0f, 1f);
            AimVector *= 100f - Stats.Accuracy;
            AimVector /= 250f;
            AimVector += this.transform.forward;

            Vector3 point = Vector3.zero;
            RaycastHit hit;
            if (Physics.Raycast(Origin.position, AimVector, out hit, Stats.Range))
            {
                LifePart Part = hit.collider.GetComponent<LifePart>();
                if (Part)
                {
                    int Damage = Mathf.RoundToInt(Stats.Damage * (1f - (hit.distance / Stats.Range))); //linear falloff for damage

                    Part.Hurt(Damage);
                }

                if (hit.transform.gameObject.isStatic)
                {
                    GameObject Decal = GameObject.Instantiate(DecalPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                }

                point = hit.point;
            }
            else
            {
                point = Origin.position + (AimVector * Stats.Range);
            }

            Debug.DrawLine(Origin.position, point, Color.red, 2f);
        }
        
        Recoil();
    }
}