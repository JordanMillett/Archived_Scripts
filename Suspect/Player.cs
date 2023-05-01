using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class Player : NetworkBehaviour
{
    [SyncVar]
    public int Health = 2;

    [SyncVar]
    public bool AIControlled = false;

    [SyncVar(hook = nameof(Die))]
    public Status LifeStatus = Status.Alive;

    [SyncVar]
    public int MaterialIndex = 0;

    [SyncVar]
    public int MeshIndex = 0;

    public int PocketMoney = 0;

    public MeshRenderer MR;
    public MeshFilter MF;

    public HashSet<Watcher> Watchers = new HashSet<Watcher>();

    public enum Status
    {
        Alive,
        Hunted,
        Killed,
        KilledSelf
    };

    public AudioSource Voice;

    public static Player localPlayer = null;

    public Dictionary<Game.Needs, Need> Needs = new Dictionary<Game.Needs, Need>
    {
        {Game.Needs.Hunger, new Need(4, Need.Effects.Kill)},
        {Game.Needs.Thirst, new Need(3, Need.Effects.Kill)},
        {Game.Needs.Sleep, new Need(5, Need.Effects.Sleep)},
        {Game.Needs.Social, new Need(7, Need.Effects.Kill)},
        {Game.Needs.Boredom, new Need(6, Need.Effects.Kill)},
        {Game.Needs.Hygiene, new Need(6, Need.Effects.Stink)},
        {Game.Needs.Solid, new Need(Need.Effects.Dirty)},
        {Game.Needs.Liquid, new Need(Need.Effects.Dirty)}
    };

    float movementForce = 60f;
    float maxSpeed = 5f;
    float yaw = 0f;
    float pitch = 0f;

    float pitchLimits = 80f;

    public Transform Eyes;

    public GameObject Hands;

    public bool Animating = false;

    Vignette Hurt;

    public enum States
    {
        Idle,
        Hunting
    };
    public States State;
    NavMeshQueryFilter NavInfo;
    NavMeshPath PathToGoal;
    int PathIndex = 0;
    
    Rigidbody _rigidbody;
    
    readonly Queue<float> _streamingReadQueue = new();

    public List<Mesh> Meshes;
    public List<Material> Materials;

    InputAction Action_Interact;
    InputAction Action_Attack;
    InputAction Action_Move;
    InputAction Action_Sprint;
    InputAction Action_Talk;
    InputAction Action_Look;
    
    [Client]
    void RegisterInput()
    {
        Action_Interact = Game.ActionMap.FindAction("Interact");
        Action_Attack = Game.ActionMap.FindAction("Attack");
        Action_Move = Game.ActionMap.FindAction("Move");
        Action_Sprint = Game.ActionMap.FindAction("Sprint");
        Action_Talk = Game.ActionMap.FindAction("Talk");
        Action_Look = Game.ActionMap.FindAction("Look");

        Action_Interact.performed += Result_Interact;
        Action_Attack.performed += Result_Attack;
        
        Action_Talk.started += Result_Talk_Start;
        Action_Talk.canceled += Result_Talk_Stop;

        Action_Interact.Enable();
        Action_Attack.Enable();
        Action_Move.Enable();
        Action_Sprint.Enable();
        Action_Talk.Enable();
        Action_Look.Enable();
    }
    
    [Client]
    void Result_Interact(InputAction.CallbackContext action)
    {
        if (Cursor.lockState == CursorLockMode.Locked && !Animating)
        {
            RaycastHit hit;
            if (Physics.Raycast(Eyes.transform.position, Eyes.transform.forward, out hit, 4f, LayerMask.GetMask("Interactable")))
            {
                Interactable I = hit.transform.gameObject.GetComponent<Interactable>();

                if (I)
                    I.Activate();
            }
        }
    }
    
    [Client]
    void Result_Attack(InputAction.CallbackContext action)
    {
        if (Cursor.lockState == CursorLockMode.Locked && !Animating)
        {
            if (UIManager.instance.Hands.transform.GetChild(0).GetComponent<Weapon>().CanFire())
            {
                FireData data = UIManager.instance.Hands.transform.GetChild(0).GetComponent<Weapon>().Fire(UIManager.instance.Cam.transform, netId);
                data.FromViewModel = false;
                CmdShootAt(data);
            }
        }
    }
    
    [Command]
    void CmdShootAt(FireData data)
    {
        RpcShootAt(data);
    }
    
    [ClientRpc(includeOwner = false)]
    void RpcShootAt(FireData data)
    {
        Hands.transform.GetChild(0).GetComponent<Weapon>().DrawSmokeLine(data);
    }
    
    void OnDestroy()
    {
        if(isLocalPlayer)
        {
            Action_Interact.performed -= Result_Interact;
            Action_Attack.performed -= Result_Attack;
            
            Action_Talk.started -= Result_Talk_Start;
            Action_Talk.canceled -= Result_Talk_Stop;
            
            Action_Interact.Disable();
            Action_Attack.Disable();
            Action_Move.Disable();
            Action_Sprint.Disable();
            Action_Talk.Disable();
            Action_Look.Disable();
        }
    }

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        //Voice.clip = AudioClip.Create("Voice", 11025 , 1, 11025, false);
        Voice.clip = AudioClip.Create("Voice", System.Convert.ToInt32(SteamUser.GetVoiceOptimalSampleRate()), 1, System.Convert.ToInt32(SteamUser.GetVoiceOptimalSampleRate()), true, PcmReaderCallback);
        Voice.Play();

        if (isLocalPlayer)
        {
            RegisterInput();
            _rigidbody.isKinematic = false;
            Hands.SetActive(false);
            localPlayer = this;

            Server.instance.CmdUpdateRoster(SteamUser.GetSteamID(), netId, new PlayerInfo
            (
                SteamFriends.GetFriendPersonaName(SteamUser.GetSteamID()).ToString()
            ));
        
            if(UIManager.instance.GetComponent<Volume>().profile.TryGet<Vignette>(out Vignette tmpVignette) )
                Hurt = tmpVignette;

            CmdRequestNewModel();

            InvokeRepeating("CheckNeeds", 1f, 1f);
        }
        
        if(isServer && AIControlled)
        {
            MeshIndex = Random.Range(0, Meshes.Count);
            MaterialIndex = Random.Range(0, Materials.Count);
            RpcSignalModelUpdate();

            Server.instance.AddAIToRoster(netId, new PlayerInfo
            (
                Game.Usernames[Random.Range(0, Game.Usernames.Count)]
            ));
            
            Server.instance.ServerRoster[netId].CharacterName = Game.CharacterFirstNames[Random.Range(0, Game.CharacterFirstNames.Count)] + " " + Game.CharacterLastNames[Random.Range(0, Game.CharacterLastNames.Count)];
            
            _rigidbody.isKinematic = false;
            
            PathToGoal = new NavMeshPath();
            NavInfo = new NavMeshQueryFilter();
            NavInfo.areaMask = 1;
            NavInfo.agentTypeID = 0;
            
            State = States.Idle;


            Server.instance.Bots.Add(this);

            InvokeRepeating("ChangeBehavior", 2f, 2f);

            

            //InvokeRepeating("Spin", Random.Range(2f, 7f), Random.Range(2f, 7f));
        }
    }
    
    void PcmReaderCallback(float[] data)
    {
        for (int i = 0; i < data.Length; i++)
            data[i] = _streamingReadQueue.TryDequeue(out float sample) ? sample : 0f;
    }
    
    [Command]
    void CmdRequestNewModel()
    {
        Server.instance.ServerRoster[netId].CharacterName = Game.CharacterFirstNames[Random.Range(0, Game.CharacterFirstNames.Count)] + " " + Game.CharacterLastNames[Random.Range(0, Game.CharacterLastNames.Count)];
        
        MeshIndex = Random.Range(0, Meshes.Count);
        MaterialIndex = Random.Range(0, Materials.Count);

        RpcSignalModelUpdate();
    }
    
    [ClientRpc (includeOwner = false)]
    void RpcSignalModelUpdate()
    {
        UpdateModel();
    }
    
    void UpdateModel()
    {
        MF.mesh = Meshes[MeshIndex];

        Material[] mats = MR.materials;
        mats[1] = Materials[MaterialIndex];
        MR.materials = mats;
    }
    
    [Client]
    void CheckNeeds()
    {
        foreach(Game.Needs key in Needs.Keys)
        {
            switch (Needs[key].Tick())
            {
                case Need.Effects.Kill :
                {
                    Needs[key].Current = 0;
                    //CmdDamage(netId, netId);
                } break;
                case Need.Effects.Dirty :
                {
                    Needs[key].Current = 0;
                    Needs[Game.Needs.Hygiene].Current = 100;
                } break;
                case Need.Effects.Stink :
                {
                    Debug.Log("Stinky"); //sync
                } break;
                case Need.Effects.Sleep :
                {
                    Needs[key].Current = 0;
                    //CmdDamage(netId, netId);
                } break;
            }
        }
    }
    
    [Server]
    void ChangeBehavior()
    {
        if (State == States.Hunting)
        {
            NavMesh.CalculatePath(this.transform.position + new Vector3(0f, 0.25f, 0f), NetworkServer.spawned[Server.instance.ServerRoster[netId].Target_Id].transform.position, NavInfo, PathToGoal);
            PathIndex = 0;
        }
        
        State = GetState();
    }
    
    [Server]
    States GetState()
    {    
        if(Server.instance.ServerRoster[netId].Hunting)
            return States.Hunting;

        return States.Idle;
    }
    
    [Command]
    public void CmdDamage(uint target_id, uint attacker_id)
    {
        Player targetPlayer = NetworkServer.spawned[target_id].GetComponent<Player>();
        if(targetPlayer.LifeStatus != Status.Alive)
            return;
        
        PlayerInfo targetInfo = Server.instance.ServerRoster[target_id];
        PlayerInfo attackerInfo = Server.instance.ServerRoster[attacker_id];

        targetPlayer.Health--;

        if(targetPlayer.Health == 0)
        {
            targetPlayer.GetComponent<NetworkTransformReliable>().RpcTeleport(OnlineManager.startPositions[Random.Range(0, OnlineManager.startPositions.Count)].position);
            
            Server.instance.Say(targetInfo.Username + " died");
            
            if(attackerInfo.Hunting && attackerInfo.Target_Id == target_id) //if the attacker is hunting, and this is their target
            {
                targetPlayer.LifeStatus = Status.Hunted;
                
                attackerInfo.TargetKilled = true;
                attackerInfo.Hunting = false;
                
                Server.instance.Say(attackerInfo.Username + " hunted their target");
            }else if(targetInfo.Hunting && targetInfo.Target_Id == attacker_id) //if the target is hunting, and you are their target
            {
                targetPlayer.LifeStatus = Status.Killed;
                
                Server.instance.Say(attackerInfo.Username + " killed their hunter");
            }else
            {
                targetPlayer.LifeStatus = Status.Killed;
                
                attackerInfo.BankMoney -= 250;
                Server.instance.Say(attackerInfo.Username + " hunted the wrong person");
            }
        }
    }
    
    void Die(Status oldValue, Status newValue)
    {
        if(newValue == Status.Alive)
            return;

        if(isLocalPlayer)
        {
            Animating = false;

            if (newValue == Status.Hunted)
            {
                foreach (Game.Needs key in Needs.Keys)
                {
                    Needs[key].Current = 0;
                    Needs[key].Pool = 0;
                }
            }

            Invoke("CmdRespawn", 0.25f);
        }
        
        if(isServer && AIControlled)
        {
            Invoke("AIRespawn", 0.25f);
        }
    }
    
    [Command]
    void CmdRespawn()
    {
        if(LifeStatus == Status.Hunted)
            CmdRequestNewModel();

        LifeStatus = Status.Alive;
        Health = 2;
    }
    
    [Server]
    void AIRespawn()
    {
        if (LifeStatus == Status.Hunted)
        {
            Server.instance.ServerRoster[netId].CharacterName = Game.CharacterFirstNames[Random.Range(0, Game.CharacterFirstNames.Count)] + " " + Game.CharacterLastNames[Random.Range(0, Game.CharacterLastNames.Count)];
            MeshIndex = Random.Range(0, Meshes.Count);
            MaterialIndex = Random.Range(0, Materials.Count);
            RpcSignalModelUpdate();
        }

        LifeStatus = Status.Alive;
        Health = 2;
    }
    
    void Update()
    {
        if(isClient && !isLocalPlayer)
            Voice.GetComponent<AudioSourceController>().Refresh();

        if (isLocalPlayer)
        {
            UIManager.instance.Cam.transform.position = Eyes.transform.position;
            UIManager.instance.Cam.transform.rotation = Eyes.transform.rotation;
            
            if(Cursor.lockState == CursorLockMode.Locked && !Animating)
                CameraControls();            
                
            _rigidbody.drag = Physics.CheckSphere(transform.position + new Vector3(0f, 0.2f, 0f), 0.3f, ~LayerMask.GetMask("Player")) ? 5f : 0f;
            
            if(Hurt)
                Hurt.intensity.Override(0.5f * (1f - (Health/2f)));
            
            EVoiceResult voiceResult  = SteamUser.GetAvailableVoice(out uint compressedBytes);
            if(voiceResult == EVoiceResult.k_EVoiceResultOK)
            {
                byte[] compressedAudioBuffer = new byte[compressedBytes];

                voiceResult = SteamUser.GetVoice(true, compressedAudioBuffer, compressedBytes, out uint bytesWritten);
                
                if(voiceResult == EVoiceResult.k_EVoiceResultOK)
                    CmdTransmitVoice(compressedAudioBuffer, compressedBytes, Game.SettingsData._micGain);
            }
        }
        
        if(isServer && AIControlled)
        {
            _rigidbody.drag = Physics.CheckSphere(transform.position + new Vector3(0f, 0.2f, 0f), 0.3f, ~LayerMask.GetMask("Player")) ? 5f : 0f;
        }
    }
    
    [Command (channel = 1)]
    void CmdTransmitVoice(byte[] compressedAudioBuffer, uint bytesReceived, float magnitude)
    {
        RpcTransmitVoice(compressedAudioBuffer, bytesReceived, magnitude);
    }
    
    [ClientRpc (includeOwner = false, channel = 1)]
    void RpcTransmitVoice(byte[] compressedAudioBuffer, uint bytesSent, float magnitude)
    {
        byte[] uncompressedAudioBuffer = new byte[20000];

        EVoiceResult voiceResult = Steamworks.SteamUser.DecompressVoice
        (
            compressedAudioBuffer, bytesSent,
            uncompressedAudioBuffer, (uint) uncompressedAudioBuffer.Length,
            out uint bytesUnpacked, SteamUser.GetVoiceOptimalSampleRate() 
        );

        System.Array.Resize(ref uncompressedAudioBuffer, System.Convert.ToInt32(bytesUnpacked));

        if(voiceResult == EVoiceResult.k_EVoiceResultOK)
        {
            //Debug.Log(System.Convert.ToSingle(System.BitConverter.ToInt16(uncompressedAudioBuffer, 0)) / short.MaxValue);

            for (int i = 0; i < uncompressedAudioBuffer.Length; i += 2)
            {
                float stream = System.Convert.ToSingle(System.BitConverter.ToInt16(uncompressedAudioBuffer, i)) / short.MaxValue;
                stream *= magnitude;
                _streamingReadQueue.Enqueue(stream);
            }
        }
    }

    [Client]
    void Result_Talk_Start(InputAction.CallbackContext action)
    {
        SteamUser.StartVoiceRecording();
    }
    
    [Client]
    void Result_Talk_Stop(InputAction.CallbackContext action)
    {
        SteamUser.StopVoiceRecording();
    }

    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            if(Cursor.lockState == CursorLockMode.Locked && !Animating)
                MovementControls();
        }
        
        if(isServer && AIControlled)
        {
            switch(State)
            {
                case States.Hunting :         S_Hunting();  break;
            }
        }
    }
    
    [Server]
    void S_Hunting()
    {
        if(PathToGoal.status == NavMeshPathStatus.PathComplete)
        {
            if(PathIndex == PathToGoal.corners.Length - 1)
            {
                if (LineOfSight(NetworkServer.spawned[Server.instance.ServerRoster[netId].Target_Id].GetComponent<Collider>()))
                {
                    if (Watchers.Count == 0)
                    {
                        LookAt(NetworkServer.spawned[Server.instance.ServerRoster[netId].Target_Id].transform.position);
                        if (Hands.transform.GetChild(0).GetComponent<Weapon>().CanFire())
                        {
                            FireData data = Hands.transform.GetChild(0).GetComponent<Weapon>().Fire(null, netId);
                            data.FromViewModel = false;
                            RpcShootAt(data);
                        }
                    }
                }else
                {
                    HeadTo(NetworkServer.spawned[Server.instance.ServerRoster[netId].Target_Id].transform.position);
                }

            }else
            {
                HeadTo(PathToGoal.corners[PathIndex]);
                if(Vector3.Distance(this.transform.position, PathToGoal.corners[PathIndex]) < 1f)  //reached path corner
                    PathIndex++;              //go to next one
            }
        }
    }
    
    [Server]
    bool LineOfSight(Collider Target)
    {
        bool sight = false;
        Vector3 dir = (Target.transform.position + new Vector3(0f, 1f, 0f)) - (this.transform.position + new Vector3(0f, 1f, 0f));
        
        RaycastHit hit;             
        if(Physics.Raycast((this.transform.position + new Vector3(0f, 1f, 0f)), dir.normalized, out hit, 10f, Game.WeaponMask))
            if(hit.collider == Target)
                sight = true;

        return sight;
    }
    
    [Server]
    void HeadTo(Vector3 Destination)
    {
        LookAt(Destination);

        float lerp = Mathf.Lerp(1f, 0f, _rigidbody.velocity.magnitude / maxSpeed);
        _rigidbody.AddForce((this.transform.forward * movementForce) * lerp, ForceMode.Acceleration);
    
    }
    
    [Server]
    void LookAt(Vector3 Position)
    {
        Vector3 TargetDirection = Position - this.transform.position;
        Quaternion Look = Quaternion.LookRotation(TargetDirection, Vector3.up);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Look, Time.fixedDeltaTime * 6f);//speed
        this.transform.localRotation = new Quaternion(0f, this.transform.localRotation.y, 0f, this.transform.localRotation.w);
    }
    
    [Client]
    void CameraControls()
    {
        yaw += Action_Look.ReadValue<Vector2>().x/50f;
        this.transform.rotation = Quaternion.AngleAxis(yaw, Vector3.up);
        
        pitch += Action_Look.ReadValue<Vector2>().y/50f;
        pitch = Mathf.Clamp(pitch, -pitchLimits, pitchLimits);
        Eyes.transform.localRotation = Quaternion.AngleAxis(pitch, Vector3.left);
    }

    [Client]
    void MovementControls()
    {
        Vector3 moveDirection = new Vector3();
        moveDirection += this.transform.forward * Action_Move.ReadValue<Vector2>().y;
        moveDirection += this.transform.right * Action_Move.ReadValue<Vector2>().x;

        float sprintMultiplier = 1f;
        if(Action_Sprint.IsPressed())
            sprintMultiplier = 1.5f;

        
        float lerp = Mathf.Lerp(1f, 0f, _rigidbody.velocity.magnitude / (maxSpeed * sprintMultiplier));
        _rigidbody.AddForce((moveDirection * movementForce * sprintMultiplier) * lerp, ForceMode.Acceleration);
    }
}
