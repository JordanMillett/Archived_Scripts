/*

    private bool oxygen = true;
    public bool Oxygen { get { return oxygen; } set { oxygen = value; } }
    
    if(OxygenAvailable != OxygenUsed)
        if(OxygenAvailable <= Rooms.Count)
            RecalculateOxygen(OxygenAvailable - OxygenUsed);
            
    void RecalculateOxygen(int Difference)
    {   
        bool OxygenDeficit = Difference < 0;     //60 - 80   need to shut off 20 rooms
        for (int i = 0; i < Mathf.Abs(Difference); i++)
        {
            int PathChosen = OxygenDeficit ? 0 : 1000;
            int RoomChosen = 0;
            for (int x = 0; x < Rooms.Count; x++)
            {
                if(OxygenDeficit)
                {
                    if (Rooms[x].Oxygen)
                    {
                        if (Rooms[x].PathNumber >= PathChosen)   //if path is less important
                        {
                            PathChosen = Rooms[x].PathNumber;
                            RoomChosen = x;
                        }
                    }
                }else
                {
                    if (!Rooms[x].Oxygen)
                    {
                        if (Rooms[x].PathNumber <= PathChosen)   //if path is more important
                        {
                            PathChosen = Rooms[x].PathNumber;
                            RoomChosen = x;
                        }
                    }
                }
            }
            
            for (int x = 0; x < Rooms.Count; x++)
            {
                if (Rooms[x].PathNumber == PathChosen)
                {
                    if(OxygenDeficit)
                    {
                        if(Rooms[x].Oxygen)
                            if (Rooms[x].PathIndex >= Rooms[RoomChosen].PathIndex)
                                RoomChosen = x;
                    }else
                    {
                        if(!Rooms[x].Oxygen)
                            if (Rooms[x].PathIndex <= Rooms[RoomChosen].PathIndex)
                                RoomChosen = x;
                    }
                }
            }
            
            //Debug.Log("Path - " + PathChosen + ", Index - " + Rooms[RoomChosen].PathIndex + ", Room - " + RoomChosen);

            Rooms[RoomChosen].Oxygen = OxygenDeficit ? false : true;
        }
    }
    
    void DestroyRoom()
    {
        if(Rooms.Count > 0)
        {
            int Index = 0;
            for (int i = 0; i < Rooms.Count; i++)
            {
                if(Rooms[i].PathNumber > Rooms[Index].PathNumber)
                    Index = i;
            }

            int PathToDestroy = Rooms[Index].PathNumber;
            for (int i = 0; i < Rooms.Count; i++)
            {
                if(Rooms[i].PathIndex > Rooms[Index].PathIndex && Rooms[i].PathNumber == PathToDestroy)
                    Index = i;
            }

            Rooms[Index].Explode();
            Rooms.Remove(Rooms[Index]);
        }
    }
    
    foreach (Room R in Rooms)
        if (R.PathNumber == 0)
            R.LockDoors(false);
    
    foreach (Room R in Rooms)
    {
        if (R.PathNumber != 0)
        {
            R.Power = false;
            R.LockDoors(true);
        }
    }
    
    for (int i = 0; i < 10; i++)
        Instantiate(EnemyPrefab, Rooms[Random.Range(0, Rooms.Count)].transform.position, Quaternion.identity);
    
    while(Rooms.Count > 1)
    {
        yield return new WaitForSeconds(5f);
        DestroyRoom();
    }
    
    private int oxygenAvailable = 0;
    public int OxygenAvailable { get { return oxygenAvailable; } set { oxygenAvailable = Mathf.Max(value, 0); RecalculateSystems(); } }
    [HideInInspector]
    public List<int> OxygenHistory = new List<int>(RECORD_LENGTH);
    
    private bool locked = false;
    public bool Locked 
    { 
        get { return locked; } 
        set 
        { 
            locked = value; 
            Link.area = locked ? 1 : 0; 
        } 
    }
    
    bool HasPower()
    {
        foreach(Room R in W.Owners)
            if(R.Power)
                return true;

        return false;
    }
    
    public void LockDoors(bool State)
    {
        foreach(Wall W in Walls)
        {
            if(W._Door)
                W._Door.Locked = State;
        }
    }
    
    using Unity.AI.Navigation;
    NavMeshLink Link;
    Link = GetComponent<NavMeshLink>();
    
    [ExecuteInEditMode]
    public class GetShadowMap : MonoBehaviour
    {
        CommandBuffer cb;

        void OnEnable()
        {
            var light = GetComponent<Light>();
            if (light)
            {
                cb = new CommandBuffer();
                cb.name = "GetShadowMap";
                cb.SetGlobalTexture("_PointShadowMask", new RenderTargetIdentifier(BuiltinRenderTextureType.CurrentActive));
                light.AddCommandBuffer(UnityEngine.Rendering.LightEvent.AfterShadowMapPass, cb, ShadowMapPass.Pointlight);
            }
        }

        void OnDisable()
        {
            var light = GetComponent<Light>();
            if (light)
            {
                light.RemoveCommandBuffer(UnityEngine.Rendering.LightEvent.AfterShadowMapPass, cb);
            }
        }
    }
    
    public Station S;
    public LineRenderer LR;
    
    Vector3[] LinePositions = new Vector3[Station.RECORD_LENGTH];
    
    void Start()
    {
        for(int i = 0; i < Station.RECORD_LENGTH; i++)
            LinePositions[i] = Vector3.zero;
    }
    
    void Update()
    {
        if(S.Ready)
        {
            for(int i = 0; i < Station.RECORD_LENGTH; i++)
            {
                LinePositions[i] = new Vector3(((float)i/(Station.RECORD_LENGTH - 1f)) * 10f, ((float)S.PowerHistory[i]/(float)S.StartingRooms) * 10f, 0f);
            }
            
            LR.positionCount = Station.RECORD_LENGTH;
            LR.SetPositions(LinePositions);
        }
    }
    
    public static Color EvaluateDamageColor(DamageBonus input)
    {
        return input switch
        {
            DamageBonus.None => CommonColors["StandardDamage"],
            DamageBonus.Health => CommonColors["HealthDamage"],
            DamageBonus.Shields => CommonColors["ShieldDamage"],
            _ => CommonColors["StandardDamage"],
        };
    }
    
    public static float EvaluateProjectileSpeed(ProjectileSpeed input, bool PlayerControlled)
    {
        
        if (PlayerControlled)
        {
            return input switch
            {
                ProjectileSpeed.Slow => 19f,
                ProjectileSpeed.Regular => 21f,
                ProjectileSpeed.Fast => 23f,
                _ => 19f,
            };
        }else
        {
            return input switch
            {
                ProjectileSpeed.Slow => 15f,
                ProjectileSpeed.Regular => 17f,
                ProjectileSpeed.Fast => 19f,
                _ => 15f,
            };
        }
    }
    
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName ="New Station", menuName = "Station")]
    public class Station : ScriptableObject
    {
        public string Name;
        public int MaxBlueprints = 2;
        public int BlueprintTime = 30;
        public int EnemyStartingLevel = 1;
        public float ValuableMultiplier = 1f;
        public float DamageMultiplier = 1f;
        public Texture2D Map;
    }
    
    IEnumerator Cooldown()
    {
        float CooldownDelay = (float)Info.RPM/60f;
        float Timer = Time.time;

        while(true)
        {
            if(OnCooldown)
            {
                Timer = Time.time;
                while(Time.time < Timer + (1f/CooldownDelay))
                {
                    yield return null;
                }

                OnCooldown = false;
            }else
            {
                yield return null;
            }
        }
    }
    
    Info.text = "";
    Info.text += "Day " + Game.SaveData.Day.ToString() + "\n\n";
    Info.text += "Credits\t" + Game.SaveData.Credits.ToString() + "\n";
    Info.text += "Blueprints\t" + Game.SaveData.Blueprints.ToString() + "\n";
    Info.text += "Vault\t\t" + Game.SaveData.VaultCredits.ToString() + "\n\n";
    
*/