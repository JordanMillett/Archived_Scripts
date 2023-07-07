using System.Collections;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(BoxCollider))]
public class Package : NetworkBehaviour
{
    public float PackageValueBonus;
    float InitialDistance = 0f;

    MeshRenderer PackageMesh;
    IEnumerator EffectRoutine;
    float EffectFadeTime = .3f;
    bool EffectRunning = false;

    Color DamageColor = Color.red;
    Color FinishColor = Color.yellow;
    Color ScanColor = Color.blue;

    NetworkIdentity netID;

    public GameObject OpenEffect;
    public AudioClip UnboxSound;
    public AudioClip BreakSound;

    [SyncVar]
    public int ID;
    [SyncVar]
    public string Contents;
    [SyncVar]
    public string FileName;
    [SyncVar]
    public float Durability;
    [SyncVar]
    public Vector3 Destination;
    [SyncVar]
    bool beenDelivered = false;

    bool Initialized = false;

    void Start()
    {
        netID = GetComponent<NetworkIdentity>();

        if (netID.hasAuthority)
        {
            ID = Random.Range(0, 100000);
            Item I = Client.Instance.GetItem(GetComponent<BoxCollider>().size);
            Contents = I.Name;
            FileName = I.FileName;
            Destination = Client.Instance.GetLocation();
            Durability = 100f;
        }

        InitialDistance = Vector3.Distance(this.transform.position, Destination);
        PackageMesh = transform.GetChild(0).GetComponent<MeshRenderer>();
        Initialized = true;
    }

    public bool Delivered()
    {
        if (PackageMesh.isVisible && !beenDelivered && netID.hasAuthority)
        {
            if (Vector3.Distance(this.transform.position, Destination) < 1.5f)
            {
                //GameObject.FindWithTag("HUD").GetComponent<HUD>().ClearNavigation();
                if(GameObject.FindWithTag("Target").GetComponent<Target>().Active == this)
                    GameObject.FindWithTag("Target").GetComponent<Target>().Clear();
                //GameObject.FindWithTag("Compass").GetComponent<Compass>().MarkerLocation = Vector3.zero;

                int Earned =
                    Mathf.RoundToInt
                    ((
                        Client.Instance.ServerInstance.ServerRules.DistanceValueMultiplier * (InitialDistance / 10f) * PackageValueBonus * (Durability / 100f)
                    ));

                if (EffectRunning)
                    StopCoroutine(EffectRoutine);

                EffectRoutine = ColorEffect(FinishColor);
                StartCoroutine(EffectRoutine);

                transform.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative; ;
                transform.GetComponent<Rigidbody>().isKinematic = true;


                Client.Instance.SessionSave.Balance += Earned;
                Client.Instance.SessionSave.TotalDeliveries++;
                Client.Instance.SessionSave.TotalScore += 1;
                Client.Instance.SyncSessionSave();


                CmdDeliver();
                //transform.GetComponent<Collider>().enabled = false;


                return true;

            }
        }

        return false;
    }

    [Command]
    void CmdDeliver()
    {
        RpcDeliver();
    }

    [ClientRpc]
    void RpcDeliver()
    {
        if(!Initialized)
            return;
        
        beenDelivered = true;

        transform.GetComponent<Collider>().enabled = false;
        Despawn D = this.gameObject.AddComponent(typeof(Despawn)) as Despawn;
        D.DespawnTime = 10f;
    }

    void OnCollisionEnter(Collision col)
    {
        if (netID.hasAuthority)
        {
            //Debug.Log(col.impulse.magnitude);
            if ((col.impulse.magnitude / 4f) > Client.Instance.ServerInstance.ServerRules.PackageDamageThreshold)
            {
                CmdDamage(col.impulse.magnitude);
            }
        }
    }

    [Command]
    void CmdDamage(float force)
    {
        RpcDamage(force);
    }

    [ClientRpc]
    void RpcDamage(float force)
    {
        if(!Initialized)
            return;

        if (EffectRunning)
            StopCoroutine(EffectRoutine);

        EffectRoutine = ColorEffect(DamageColor);
        StartCoroutine(EffectRoutine);

        float Damage = Mathf.Round(force / 4f) - Client.Instance.ServerInstance.ServerRules.PackageDamageThreshold;
        Damage *= Client.Instance.ServerInstance.ServerRules.DamageMultiplier;
        if (Durability >= Damage)
        {
            Durability -= Damage;
        }
        else
        {
            Durability = 0f;

            if (netID.hasAuthority)
            {
                Client.Instance.SessionSave.TotalDeliveries++;
                Client.Instance.SyncSessionSave();
                if(GameObject.FindWithTag("Target").GetComponent<Target>().Active == this)
                    GameObject.FindWithTag("Target").GetComponent<Target>().Clear();
                //GameObject.FindWithTag("HUD").GetComponent<HUD>().ClearNavigation();
                CmdBreak();
            }
        }

        PackageMesh.materials[0].SetFloat("_deform", 1f - (Durability / 100f));
        PackageMesh.materials[1].SetFloat("_deform", 1f - (Durability / 100f));
    }

    public void Scanned()
    {
        GameObject.FindWithTag("Target").GetComponent<Target>().SetLocation(Destination);
        GameObject.FindWithTag("Target").GetComponent<Target>().Active = this;
        //GameObject.FindWithTag("HUD").GetComponent<HUD>().Navigate(Destination);

        if (EffectRunning)
            StopCoroutine(EffectRoutine);

        EffectRoutine = ColorEffect(ScanColor);
        StartCoroutine(EffectRoutine);
    }

    IEnumerator ColorEffect(Color C)
    {
        EffectRunning = true;

        PackageMesh.materials[0].SetColor("_glow", C);
        PackageMesh.materials[1].SetColor("_glow", C);

        float alpha = 0f;
        while (alpha < 1f)
        {
            PackageMesh.materials[0].SetFloat("_effect", 1f - alpha);
            PackageMesh.materials[1].SetFloat("_effect", 1f - alpha);
            yield return new WaitForSeconds(EffectFadeTime / 60f);
            alpha += 1f / 60f;
        }

        PackageMesh.materials[0].SetFloat("_effect", 0f);
        PackageMesh.materials[1].SetFloat("_effect", 0f);

        EffectRunning = false;
    }

    public void Open()
    {
        Client.Instance.SessionSave.TotalDeliveries++;
        Client.Instance.SyncSessionSave();
        CmdOpen();
        Client.Instance.SpawnItem(FileName, this.transform.position, this.transform.rotation, 1, false);
        if(GameObject.FindWithTag("Target").GetComponent<Target>().Active == this)
            GameObject.FindWithTag("Target").GetComponent<Target>().Clear();
    }

    public float GetDistance()
    {
        return InitialDistance;
    }

    [Command(requiresAuthority = false)]
    void CmdOpen()
    {
        RpcOpen();
    }

    [ClientRpc]
    void RpcOpen()
    {
        if(!Initialized)
            return;
        
        GameObject Sound = new GameObject();
        Sound.transform.position = this.transform.position;
        Despawn DSpawn = Sound.AddComponent(typeof(Despawn)) as Despawn;
        AudioClip Clip = UnboxSound;

        DSpawn.DespawnTime = Clip.length + 0.1f;
        AudioSource AS = Sound.AddComponent(typeof(AudioSource)) as AudioSource;
        AS.spatialBlend = 1f;
        AS.volume = (.2f * (Settings._sfxVolume / 100f)) * (Settings._masterVolume / 100f);
        AS.maxDistance = 50f;
        AS.clip = Clip;
        AS.Play();

        Instantiate(OpenEffect, this.transform.position, this.transform.rotation);
        
        Destroy(this.gameObject);
    }

    [Command]
    void CmdBreak()
    {
        RpcBreak();
    }

    [ClientRpc]
    void RpcBreak()
    {
        if(!Initialized)
            return;
        
        GameObject Sound = new GameObject();
        Sound.transform.position = this.transform.position;
        Despawn DSpawn = Sound.AddComponent(typeof(Despawn)) as Despawn;
        AudioClip Clip = BreakSound;

        DSpawn.DespawnTime = Clip.length + 0.1f;
        AudioSource AS = Sound.AddComponent(typeof(AudioSource)) as AudioSource;
        AS.spatialBlend = 1f;
        AS.volume = (.2f * (Settings._sfxVolume / 100f)) * (Settings._masterVolume / 100f);
        AS.maxDistance = 50f;
        AS.clip = Clip;
        AS.Play();

        Instantiate(OpenEffect, this.transform.position, this.transform.rotation);

        Destroy(this.gameObject);
    }
}
