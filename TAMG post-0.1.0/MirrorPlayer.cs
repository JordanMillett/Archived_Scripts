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
    public int _faceIndex = -1;

    public readonly SyncList<byte> _customFaceData = new SyncList<byte>();

    [SyncVar]
    public string playerName = "MISSING";

    void Start()
    {
        MRD = GetComponent<Player>();
        
        if(!GetComponent<NetworkIdentity>().hasAuthority)
        {
            NameText.text = playerName;
            LAC.gameObject.SetActive(true);
            LAC.transform.parent = null;
            RefreshColors();
        }
    }

    [ClientRpc(includeOwner = false)]
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
    
    [ClientRpc(includeOwner = false)]
    void RpcUseEquipped(int _equippedIndex)
    {
        switch (MRD.EQs[_equippedIndex].Name)
        {
            case "Phone" : MRD.EquippedInstance.GetComponent<Phone>().StartPhotoEffect(); break;
            case "Scanner" : MRD.EquippedInstance.GetComponent<Scanner>().StartScanEffect(); break;
        }
    }

    [ClientRpc(includeOwner = false)]
    void RpcDie(Vector3 _velocity, Vector3 _angularVelocity)
    {
        MRD.Model.gameObject.SetActive(false); 
        LAC.gameObject.SetActive(false);
        MRD.Col.enabled = false;
        
        if(MRD.EquippedInstance != null)
        {
            Destroy(MRD.EquippedInstance);
        }

        GameObject NewRagdoll = Instantiate(MRD.RagdollPrefab, this.transform.position, this.transform.rotation);
        NewRagdoll.GetComponent<Ragdoll>().Init(MRD, _velocity, _angularVelocity);
    }

    [ClientRpc(includeOwner = false)]
    void RpcRespawn()
    {
        MRD.Model.gameObject.SetActive(true); 
        LAC.gameObject.SetActive(true);
        MRD.Col.enabled = true;
    }

    [ClientRpc(includeOwner = false)]
    void RpcRefreshColors()                 //on they change refresh
    {
        RefreshColors();
    }
    
    void RefreshColors()
    {
        MRD.Mats = MRD.Model.materials;

        MRD.Mats[0].SetColor("_albedo", _skinColor);         //SKIN
        MRD.Mats[3].SetColor("_albedo", _skinColor);
        MRD.Mats[1].SetColor("_albedo", _pantsColor);         //PANTS
        MRD.Mats[2].SetColor("_albedo", _shirtColor);         //SHIRT

        if(_faceIndex != 4)
        {
            MRD.Mats[3].SetTexture("_face", Client.Instance.GetFace(_faceIndex)); //FACE
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
    public void CmdUploadColors(int faceIndex, Color skinColor, Color pantsColor, Color shirtColor, List<byte> customFaceData)
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

        RpcRefreshColors();
    }
    
    [Command]
    public void CmdRefreshColors()
    {
        RpcRefreshColors();
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
    public void CmdUseEquipped(int _equippedIndex)
    {
        RpcUseEquipped(_equippedIndex);
    }

    [Command]
    public void CmdDie(Vector3 _velocity, Vector3 _angularVelocity)
    {
        RpcDie(_velocity, _angularVelocity);   
    }
}
