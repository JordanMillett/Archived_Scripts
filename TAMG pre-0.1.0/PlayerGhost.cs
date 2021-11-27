using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class PlayerGhost : NetworkBehaviour
{
    Player Connected;
    Transform Player;

    public SkinnedMeshRenderer Model;
    public Material[] Mats = new Material[4];

    public SkinnedMeshRenderer Ragdoll;
    public Material[] RagdollMats = new Material[4];

    public Animator An;

    bool DebugMode = false;

    public TextMeshProUGUI NameText;

    public LookAtCamera LAC;

    bool isOwner = false;

    public GameObject StartingVehicle;

    public Transform Head;

    public BodyPositions bodyModel;
    public BodyPositions ragModel;

    public Collider Col;

    public Collider OnlineTrigger;

    NetworkIdentity netID;

    public ASwithBuffer Voice;

    [SyncVar]
    public float VoicePitch;

    void Awake()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        netID = GetComponent<NetworkIdentity>();

        Voice.AS.pitch = VoicePitch;

        if(netID.hasAuthority == false)   //new ghosts don't belong to player (they belong to client)
        {
            isOwner = false;
            NameText.text = GetComponent<VariableSync>().playerName;
            LAC.gameObject.SetActive(true);
            LAC.transform.parent = null;    //unattach name component
            //Destroy(this);  //destroys the script
        }else                                                       //if this belongs to the user
        {
            isOwner = true;
            Connected = GameObject.FindWithTag("Player").GetComponent<Player>();      //connect ghost to player
            Player = Connected.transform;

            Player.transform.position = this.transform.position;
            
            //Connected.OnlineAn = An;                                                            //Sync player anim to ghost
            //Head = Connected.CamEmpty.transform;      //Sync player head to ghost head
            //Connected.PG = this;

            //GameObject.FindWithTag("Camera").GetComponent<MenuManager>().CM.OnlineGhost = this;
            //GameObject.FindWithTag("Camera").GetComponent<MenuManager>().CM.InitToGhost();

            //InitializeSettings();

            if(!DebugMode)                                                                      //Show model if debug mode
            {
                this.transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);
                Col.enabled = false;
            }
        }
        
    }

    public void SpawnVehicle(NetworkConnection NC)
    {
        Vector3 Pos = GameServer.GS.GetNearestVehicleSpawn(this.transform.position).position;
        Quaternion Rot = GameServer.GS.GetNearestVehicleSpawn(this.transform.position).rotation;
        //Car.transform.position = GameServer.GS.GetNearestVehicleSpawn(this.transform.position).position;
        //Car.transform.rotation = GameServer.GS.GetNearestVehicleSpawn(this.transform.position).rotation;
        //NetworkServer.Spawn(Car, NC);
        //GameServer.GS.SpawnObject("Veh_Golf", Pos, Rot, false);
        

        //GameManagerNetwork GMN = GameObject.FindWithTag("Manager").GetComponent<GameManagerNetwork>();
        //GMN.SpawnVehicleAtLocation(StartingVehicle, this.transform.position);
    }

    [Command]
    public void CmdSetVoiceBuffer(float[] _newBuffer)
    {
        RpcSetVoiceBuffer(_newBuffer);   
    }

    //[ClientRpc(excludeOwner = true)]
    [ClientRpc]
    public void RpcSetVoiceBuffer(float[] _newBuffer)
    {
        if(!Voice.Transmitting || true)
        {
            for(int i = 0; i < Voice.VoiceBuffer.Length; i++)
            {
                Voice.VoiceBuffer[i] = _newBuffer[i];
            }
            //Voice.VoiceBuffer = _newBuffer;
            Voice.Transmitting = true;
        }
    }

    [Command]
    public void CmdInitVoice(float pitch)
    {
        RpcInitVoice(pitch);   
    }

    [ClientRpc]
    public void RpcInitVoice(float pitch)
    {
        VoicePitch = pitch;
        Voice.AS.pitch = VoicePitch;
    }
    /*
    void InitializeSettings()
    {
        string path = Application.persistentDataPath + "customize.txt";
 
        if(!File.Exists(path)) 
        {
            FinalizeInfo(0, Color.black,Color.black,Color.black);
        }else
        {

            StreamReader SR = new StreamReader(path);
            string settingText = SR.ReadToEnd();
            SR.Close();

            string[] settingsLines = settingText.Split("\n"[0]);

            //Debug.Log(MeshColors[0]);

            Color skin = Color.black;
            Color pants = Color.black;
            Color shirt = Color.black;

            int face = int.Parse(settingsLines[0]);                                //FACE
            ColorUtility.TryParseHtmlString("#" + settingsLines[1], out skin);   //SKIN
            ColorUtility.TryParseHtmlString("#" + settingsLines[2], out pants);   //PANTS
            ColorUtility.TryParseHtmlString("#" + settingsLines[3], out shirt);   //SHIRT

            FinalizeInfo(face, skin, pants, shirt);

        }

    }*/
    /*
    public void FinalizeInfo(int faceIndex, Color skinColor, Color pantsColor, Color shirtColor)
    {
        VariableSync VS = GetComponent<VariableSync>();
        VS.faceIndex = faceIndex;
        VS.skinColor = skinColor;
        VS.pantsColor = pantsColor;
        VS.shirtColor = shirtColor;
        VS.RefreshColors();
    }*/

    void Update()
    {
        if(isOwner)                                                                            //Sync pos and rot if this is yours
        {
            Head.transform.rotation = Connected.CamEmpty.transform.rotation; 
            this.transform.position = Player.transform.position;
            this.transform.rotation = Player.transform.rotation;
        }

        //An.Play(Connected.An.GetCurrentAnimatorStateInfo(0).shortNameHash, -1, Connected.An.GetCurrentAnimatorStateInfo(0).normalizedTime);
        //An.Play(Connected.An.GetCurrentAnimatorStateInfo(1).shortNameHash, -1, Connected.An.GetCurrentAnimatorStateInfo(1).normalizedTime);
        
    }

    [Command]
    public void CmdKill(string message)
    {
        RpcKill(message);   
    }

    [ClientRpc]
    public void RpcKill(string message)
    {
        if(netID.hasAuthority == true)
            Connected.Die(message);   
    }

    [Command]
    public void CmdDie(Vector3 _velocity, Vector3 _angularVelocity)
    {
        RpcChangeTrigger(false);
        RpcDie(_velocity, _angularVelocity);   
    }

    [ClientRpc(excludeOwner = true)]
    void RpcDie(Vector3 _velocity, Vector3 _angularVelocity)
    {
        LAC.gameObject.SetActive(false);
        bodyModel.gameObject.SetActive(false);
        Col.enabled = false;

        ragModel.gameObject.SetActive(true);
        ragModel.transform.SetParent(null);

        SyncRagdoll(_velocity, _angularVelocity);
    }

    [Command]
    public void CmdRespawn()
    {
        RpcChangeTrigger(true);
        RpcRespawn();
    }

    [ClientRpc(excludeOwner = true)]
    void RpcRespawn()
    {
        LAC.gameObject.SetActive(true);
        ragModel.gameObject.SetActive(false);

        bodyModel.gameObject.SetActive(true);
        Col.enabled = true;
        ragModel.transform.SetParent(this.transform);
    }

    [ClientRpc]
    void RpcChangeTrigger(bool status)
    {
        OnlineTrigger.enabled = status;
    }

    void SyncRagdoll(Vector3 _velocity, Vector3 _angularVelocity)
    {
        for(int i = 0; i < ragModel.Syncs.Count; i++)
        {
            ragModel.Syncs[i].position = bodyModel.Syncs[i].position;
            ragModel.Syncs[i].rotation = bodyModel.Syncs[i].rotation;

            if(ragModel.Syncs[i].GetComponent<Rigidbody>() != null)
            {
                ragModel.Syncs[i].GetComponent<Rigidbody>().velocity = _velocity;
                ragModel.Syncs[i].GetComponent<Rigidbody>().angularVelocity = _angularVelocity;
            }
        }
    }
    
}
