using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    public Transform CameraEmpty;
    public SkinnedMeshRenderer Model;
    public Material[] Mats = new Material[4];
    BodyPositions BP; 

    public void Init(Player P, Vector3 _velocity, Vector3 _angularVelocity)
    {
        BP = GetComponent<BodyPositions>();
        P.CamOverride = CameraEmpty;

        for(int i = 0; i < BP.Syncs.Count; i++)
        {
            BP.Syncs[i].position = P.BodyInfo.Syncs[i].position;
            BP.Syncs[i].rotation = P.BodyInfo.Syncs[i].rotation;

            if(BP.Syncs[i].GetComponent<Rigidbody>() != null)
            {
                BP.Syncs[i].GetComponent<Rigidbody>().velocity = _velocity;
                BP.Syncs[i].GetComponent<Rigidbody>().angularVelocity = _angularVelocity;
            }
        }

        Mats = Model.materials;
        Mats[0].SetColor("_albedo", P.MRD._skinColor);          //SKIN
        Mats[3].SetColor("_albedo", P.MRD._skinColor);          //FACE
        Mats[1].SetColor("_albedo", P.MRD._pantsColor);         //PANTS
        Mats[2].SetColor("_albedo", P.MRD._shirtColor);         //SHIRT
        
        if(P.MRD._faceIndex != 4)
        {
            Mats[3].SetTexture("_face", Client.Instance.GetFace(P.MRD._faceIndex)); //FACE
        }else
        {
            Texture2D customFace = new Texture2D(32, 32, TextureFormat.Alpha8, false);
            byte[] CustomFaceDataArray = new byte[P.MRD._customFaceData.Count];

            for(int i = 0; i < P.MRD._customFaceData.Count; i++)
            {
                CustomFaceDataArray[i] = P.MRD._customFaceData[i];
            }

            customFace.LoadImage(CustomFaceDataArray);
            customFace.filterMode = FilterMode.Point;
            customFace.Apply();


            Mats[3].SetTexture("_face", customFace); //FACE
        }

        Invoke("Despawn", Settings._ragdollDespawnTime);
    }

    void Despawn()
    {
        Destroy(this.gameObject);
    }
}
