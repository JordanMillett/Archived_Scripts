using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Watcher : MonoBehaviour
{
    public float FOV = 60f;
    public float Range = 6f;

    public MeshRenderer Indicator;

    void Update()
    {
        if(NetworkServer.active) //serverside, if camera can see ai
        {
            if(!Server.instance)
                return;
            
            foreach (Player Bot in Server.instance.Bots)
            {
                if (CanSee(Bot))
                {
                    if (!Bot.Watchers.Contains(this))
                        Bot.Watchers.Add(this);
                }
                else
                {
                    if (Bot.Watchers.Contains(this))
                        Bot.Watchers.Remove(this);
                }
            }
        }
        
        if(NetworkClient.active) //clientside, if camera can see player
        {
            if(!Player.localPlayer)
                return;

            if(CanSee(Player.localPlayer))
            {
                Indicator.material.SetFloat("_Visible", 1f);
                
                if(!Player.localPlayer.Watchers.Contains(this))
                    Player.localPlayer.Watchers.Add(this);
            }else
            {
                Indicator.material.SetFloat("_Visible", 0f);
                
                if(Player.localPlayer.Watchers.Contains(this))
                    Player.localPlayer.Watchers.Remove(this);
            }
        }
    }

    bool CanSee(Player Target)
    {   
        float dist = Vector3.Distance(this.transform.position, Target.Eyes.transform.position);
        
        if (dist < Range)
        {
            Vector3 dir = (Target.Eyes.transform.position - this.transform.position).normalized;

            Vector3 localDir = this.transform.InverseTransformDirection(dir);

            float xAngle = Vector3.Angle(Vector3.forward, new Vector3(localDir.x, 0f, localDir.z));
            float yAngle = Vector3.Angle(Vector3.forward, new Vector3(0f, localDir.y, localDir.z));

            float HFOV = FOV * Mathf.Deg2Rad;
            HFOV = Mathf.Atan(Mathf.Tan(HFOV * .5f) * (16/9f)) * Mathf.Rad2Deg;

            if(xAngle < HFOV && yAngle < FOV/2f)
            {
                RaycastHit hit;

                if (Physics.Raycast(Target.Eyes.transform.position, -dir, out hit, Range, Game.CameraMask))
                {
                    if (hit.transform.gameObject == this.transform.gameObject)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawFrustum(Vector3.zero, FOV, Range, 0f, 16f/9f);
    }
}
