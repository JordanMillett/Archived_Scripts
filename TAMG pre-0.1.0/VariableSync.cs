using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class VariableSync : NetworkBehaviour
{
    [SyncVar]
    public string playerName = "ERRORRRR";
    
    [SyncVar]
    public Color _skinColor = Color.black;

    [SyncVar]
    public Color _pantsColor = Color.black;

    [SyncVar]
    public Color _shirtColor = Color.black;

    [SyncVar]
    public int _faceIndex = 0;

    [SyncVar]
    public int TimesUpdated = 0;

    public SyncList<byte> _customFaceData = new SyncList<byte>();

    int PrevTimesUpdated = 0;

    GameManager GM;
    NetworkIdentity netID;

    /*
    Every time colors is changed it increases an int
    if the local int doesn't equal the network int then refresh colors
    on refresh colors either send info or recieve info
    */

    void Awake()
    {
        netID = GetComponent<NetworkIdentity>();

        if(SceneManager.GetActiveScene().buildIndex != 0)
        {
            GM = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
        }
    }

    void Update()
    {
        if(PrevTimesUpdated != TimesUpdated)
        {
            PrevTimesUpdated = TimesUpdated;

            if(!netID.hasAuthority)
            {
                ReceiveColors();
            } 
        }
    }

    public void ChangeColors(int faceIndex, Color skinColor, Color pantsColor, Color shirtColor, List<byte> customFaceData)
    {
        

        TimesUpdated++;
        CmdSendColors(faceIndex, skinColor, pantsColor, shirtColor, TimesUpdated, customFaceData);
        
    }

    public void ReceiveColors()
    {
        //Debug.Log("COLORS Received");
        PlayerGhost PG = GetComponent<PlayerGhost>();

        PG.Mats = PG.Model.materials;

        PG.Mats[0].SetColor("_albedo", _skinColor);         //SKIN
        PG.Mats[3].SetColor("_albedo", _skinColor);
        PG.Mats[1].SetColor("_albedo", _pantsColor);         //PANTS
        PG.Mats[2].SetColor("_albedo", _shirtColor);         //SHIRT
        


        PG.RagdollMats = PG.Ragdoll.materials;

        PG.RagdollMats[0].SetColor("_albedo", _skinColor);         //SKIN
        PG.RagdollMats[3].SetColor("_albedo", _skinColor);
        PG.RagdollMats[1].SetColor("_albedo", _pantsColor);         //PANTS
        PG.RagdollMats[2].SetColor("_albedo", _shirtColor);         //SHIRT
        

        if(_faceIndex != 4)
        {
            PG.Mats[3].SetTexture("_face", GameServer.GS.GetFace(_faceIndex)); //FACE
            PG.RagdollMats[3].SetTexture("_face", GameServer.GS.GetFace(_faceIndex)); //FACE
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


            PG.Mats[3].SetTexture("_face", customFace); //FACE
            PG.RagdollMats[3].SetTexture("_face", customFace); //FACE
        }
    }

    [Command]
    public void CmdSendColors(int faceIndex, Color skinColor, Color pantsColor, Color shirtColor, int times, List<byte> customFaceData)
    {
        //Debug.Log("COLORS Sent");

        _faceIndex = faceIndex;
        _skinColor = skinColor;
        _pantsColor = pantsColor;
        _shirtColor = shirtColor;

        //List<byte> to SyncList<byte>
        //_customFaceData = customFaceData;

        _customFaceData.Clear();
        foreach(byte b in customFaceData)
        {
            _customFaceData.Add(b);
        }

        TimesUpdated = times;
    }
}
