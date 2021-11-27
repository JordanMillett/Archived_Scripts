using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;
using TMPro;

public class MirrorPlayer : NetworkBehaviour
{
    Player MRD;

    public TextMeshProUGUI NameText;
    public LookAtCamera LAC;

    [SyncVar]
    public Color _skinColor = Color.black;

    [SyncVar]
    public Color _pantsColor = Color.black;

    [SyncVar]
    public Color _shirtColor = Color.black;

    [SyncVar]
    public int _faceIndex = 0;

    public SyncList<byte> _customFaceData = new SyncList<byte>();

    [SyncVar]
    public float VoicePitch;

    [SyncVar]
    public string playerName = "ERRORRRR";

    public void Initialize()
    {
        MRD = GetComponent<Player>();
    }

    public void MirrorStart()
    {
        NameText.text = playerName;
        LAC.gameObject.SetActive(true);
        LAC.transform.parent = null;    //unattach name component
    }

    [ClientRpc(excludeOwner = true)]
    void RpcChangeEquipped(int _equippedIndex)
    {
        if(MRD.EquippedInstance != null)
        {
            Destroy(MRD.EquippedInstance);
        }

        if(MRD.EQs[_equippedIndex].Prefab != null)
        {
            MRD.EquippedInstance = Instantiate(MRD.EQs[_equippedIndex].Prefab, Vector3.zero, Quaternion.identity);
            MRD.EquippedInstance.transform.SetParent(MRD.EquipSlot.transform);
            MRD.EquippedInstance.transform.localPosition = Vector3.zero;
            MRD.EquippedInstance.transform.localEulerAngles = Vector3.zero;
        }
    }

    //[ClientRpc(excludeOwner = true)]
    [ClientRpc]
    public void RpcSetVoiceBuffer(float[] _newBuffer)
    {
        if(!MRD.Voice.Transmitting || true)
        {
            for(int i = 0; i < MRD.Voice.VoiceBuffer.Length; i++)
            {
                MRD.Voice.VoiceBuffer[i] = _newBuffer[i];
            }
            MRD.Voice.Transmitting = true;
        }
    }

    [ClientRpc]
    void RpcInitializeVoice(float pitch)
    {
        VoicePitch = pitch;
        MRD.Voice.AS.pitch = VoicePitch;
    }

    [ClientRpc]
    void RpcToggleColliders()
    {
        MRD.CollidersOn = !MRD.CollidersOn;
        MRD.AllColliders[0].enabled = MRD.CollidersOn;
    }

    [ClientRpc(excludeOwner = true)]
    void RpcDie(Vector3 _velocity, Vector3 _angularVelocity)
    {
        MRD.Model.gameObject.SetActive(false); 
        LAC.gameObject.SetActive(false);
        MRD.AllColliders[0].enabled = false;
        
        if(MRD.EquippedInstance != null)
        {
            Destroy(MRD.EquippedInstance);
        }

        GameObject NewRagdoll = Instantiate(MRD.RagdollPrefab, this.transform.position, this.transform.rotation);
        NewRagdoll.GetComponent<Ragdoll>().Init(MRD, _velocity, _angularVelocity);
    }

    [ClientRpc(excludeOwner = true)]
    void RpcRespawn()
    {
        MRD.Model.gameObject.SetActive(true); 
        LAC.gameObject.SetActive(true);
        MRD.AllColliders[0].enabled = true;
    }

    [ClientRpc]
    void RpcChangeColors(int faceIndex, Color skinColor, Color pantsColor, Color shirtColor, List<byte> customFaceData)
    {
        _faceIndex = faceIndex;
        _skinColor = skinColor;
        _pantsColor = pantsColor;
        _shirtColor = shirtColor;

        _customFaceData.Clear();
        foreach(byte b in customFaceData)
        {
            _customFaceData.Add(b);
        }

        MRD.Mats = MRD.Model.materials;

        MRD.Mats[0].SetColor("_albedo", _skinColor);         //SKIN
        MRD.Mats[3].SetColor("_albedo", _skinColor);
        MRD.Mats[1].SetColor("_albedo", _pantsColor);         //PANTS
        MRD.Mats[2].SetColor("_albedo", _shirtColor);         //SHIRT
        

        if(_faceIndex != 4)
        {
            MRD.Mats[3].SetTexture("_face", GameServer.GS.GetFace(_faceIndex)); //FACE
        }else
        {
            Texture2D customFace = new Texture2D(32, 32, TextureFormat.Alpha8, false);
            byte[] CustomFaceDataArray = new byte[_customFaceData.Count];

            for(int i = 0; i < _customFaceData.Count; i++)
            {
                CustomFaceDataArray[i] = _customFaceData[i];
            }

            customFace.LoadImage(CustomFaceDataArray);
            customFace.filterMode = FilterMode.Point;
            customFace.Apply();


            MRD.Mats[3].SetTexture("_face", customFace); //FACE
        }
    }

    [Command]
    public void CmdChangeColors(int faceIndex, Color skinColor, Color pantsColor, Color shirtColor, List<byte> customFaceData)
    {
        RpcChangeColors(faceIndex, skinColor, pantsColor, shirtColor, customFaceData);
    }

    [Command]
    public void CmdRespawn()
    {
        RpcRespawn();
    }

    [Command]
    public void CmdChangeEquipped(int _equippedIndex)
    {
        RpcChangeEquipped(_equippedIndex);
    }

    [Command]
    public void CmdSetVoiceBuffer(float[] _newBuffer)
    {
        RpcSetVoiceBuffer(_newBuffer);   
    }

    [Command]
    public void CmdInitializeVoice(float pitch)
    {
        RpcInitializeVoice(pitch);   
    }
    
    [Command]
    public void CmdToggleColliders()
    {
        RpcToggleColliders();
    }

    [Command]
    public void CmdDie(Vector3 _velocity, Vector3 _angularVelocity)
    {
        RpcDie(_velocity, _angularVelocity);   
    }
}
