using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    public Transform Eyes;
    public Transform HatLocation;
    public SkinnedMeshRenderer Model;
    BodyPositions BP; 

    public Rigidbody ForceReciever;

    public void Init(Infantry I, Vector3 _velocity, Vector3 _angularVelocity, float amount)
    {
        this.gameObject.name = "Ragdoll";
        this.transform.SetParent(GameObject.FindWithTag("Trash").transform);

        BP = GetComponent<BodyPositions>();

        for(int i = 0; i < Model.materials.Length; i++)
        {
            Model.materials[i].SetColor("Team", I.U.Team == Game.TeamOne ? Game.FriendlyColor : Game.EnemyColor);
            Model.materials[i].SetTexture("Shirt", Manager.M.Factions[(int) I.U.Team].Shirt);
            Model.materials[i].SetColor("BeltColor", Manager.M.Factions[(int) I.U.Team].BeltColor);
            Model.materials[i].SetColor("ButtonColor", Manager.M.Factions[(int) I.U.Team].ButtonColor);
        }
        

        for(int i = 0; i < Model.materials.Length; i++)
                Model.materials[i].SetFloat("Damage", amount);

        if(I.U.Controller)
        {
            Transform Cam = GameObject.FindWithTag("Camera").transform;
            Cam.transform.SetParent(Eyes);
            Cam.transform.localPosition = Vector3.zero;
            Cam.transform.localEulerAngles = Vector3.zero;
            Cam.GetComponent<Player>().Body = this;
        }

        if(I.HatLocation.childCount > 0)
        {
            Transform Hat = I.HatLocation.GetChild(0).transform;
            Hat.SetParent(HatLocation);
            Hat.transform.localPosition = Vector3.zero;
            Hat.transform.localEulerAngles = Vector3.zero;
        }

        for(int i = 0; i < BP.Syncs.Count; i++)
        {
            BP.Syncs[i].position = I.BodyInfo.Syncs[i].position;
            BP.Syncs[i].rotation = I.BodyInfo.Syncs[i].rotation;
            /*
            if(BP.Syncs[i].GetComponent<Rigidbody>() != null)
            {
                BP.Syncs[i].GetComponent<Rigidbody>().velocity = _velocity;
                BP.Syncs[i].GetComponent<Rigidbody>().angularVelocity = _angularVelocity;
            }*/
        }

        ForceReciever.velocity = _velocity;
        ForceReciever.angularVelocity = _angularVelocity;
    }
}