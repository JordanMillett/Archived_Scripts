using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class TransformSync : NetworkBehaviour
{
    float positionThreshold = 0.05f;
    float rotationThreshold = 0.05f;
    float positionLerpSpeed = 5f;
    float rotationLerpSpeed = 5f;
    float extrapolateAmount = 0;
    /*
    [SyncVar]
    public List<Transform> SyncedTransforms;

    [SyncVar]
    List<float> lastUpdates = new List<float>();
    */
    
    public Transform Selected = null;
    bool Local = false;

    //Sync
    [SyncVar]
    private Vector3 newPos;
    [SyncVar]
    private Quaternion newRot;
    [SyncVar]
    private float lastUpdate;

    //Other
    private Vector3 prevPos;
    private Quaternion prevRot;
    private NetworkIdentity id;
    private Vector3 targetPos;
    private Quaternion targetRot;
    
    //TODO
    //Position Extrapolation
    //Rotation Extrapolation
    //Quaternion Distance Finder
    /*
    NetworkIdentity id;
    
    List<Quaternion> prevRots = new List<Quaternion>();
    List<Vector3> prevPoses = new List<Vector3>();

    List<Quaternion> targetRots = new List<Quaternion>();
    List<Vector3> targetPoses = new List<Vector3>();

    List<Quaternion> newRots = new List<Quaternion>();
    List<Vector3> newPoses = new List<Vector3>();
    */

    void Start()
    {
        if(Selected == null)
        {
            Selected = this.transform;
            Local = false;
        }else
        {
            Local = true;
        }

        id = GetComponent<NetworkIdentity>();
        /*
        for(int i = 0; i < SyncedTransforms.Count; i++)
        {
            prevRots.Add(Quaternion.identity);
            prevPoses.Add(Vector3.zero);

            targetRots.Add(Quaternion.identity);
            targetPoses.Add(Vector3.zero);

            newRots.Add(Quaternion.identity);
            newPoses.Add(Vector3.zero);

            lastUpdates.Add(0);
        }
        */
    }

    void Update()
    {
        if (id.hasAuthority)
        {
            //Only send update if postion has changed
            if (HasMoved())
            {
                if(!Local)
                {
                    CmdSendPosition(Selected.position, UpdatedTime());
                    prevPos = Selected.position;
                }else
                {
                    CmdSendPosition(Selected.localPosition, UpdatedTime());
                    prevPos = Selected.localPosition;
                }
            }

            //Only send update if rotation has changed
            if (HasRotated())
            {
                if(!Local)
                {
                    CmdSendRotation(Selected.rotation, UpdatedTime());
                    prevRot = Selected.rotation;
                }else
                {
                    CmdSendRotation(Selected.localRotation, UpdatedTime());
                    prevRot = Selected.localRotation;
                }
            }
        }
        else
        {
            if(!Local)
            {
                //Only move if position has changed
                if (Selected.position != newPos)
                {
                    prevPos = Selected.position;
                    ReceivePosition();
                }

                //Only rotate if rotation has changed
                if (Selected.rotation != newRot)
                {
                    prevRot = Selected.rotation;
                    ReceiveRotation();
                }
            }else
            {
                //Only move if position has changed
                if (Selected.localPosition != newPos)
                {
                    prevPos = Selected.localPosition;
                    ReceivePosition();
                }

                //Only rotate if rotation has changed
                if (Selected.localRotation != newRot)
                {
                    prevRot = Selected.localRotation;
                    ReceiveRotation();
                }
            }
        }
    }

    private void ReceivePosition()
    {
        //Lerp player position
        //transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * lerpSpeed);

        //Extrapolate player position
        targetPos = newPos + (newPos-prevPos) * extrapolateAmount * (UpdatedTime()-lastUpdate);

        if(!Local)
        {
            Selected.position = Vector3.Lerp(Selected.position, targetPos, Time.deltaTime * positionLerpSpeed);
        }else
        {
            Selected.localPosition = Vector3.Lerp(Selected.localPosition, targetPos, Time.deltaTime * positionLerpSpeed);
        }
    }

    private void ReceiveRotation()
    {
        //Extrapolate object rotation
        //targetPos = newPos + (newPos-prevPos) * extrapolateAmount * (UpdatedTime()-lastUpdate);

        targetRot = newRot;

        if(!Local)
        {
            Selected.rotation = Quaternion.Lerp(Selected.rotation, targetRot, Time.deltaTime * rotationLerpSpeed);
        }else
        {
            Selected.localRotation = Quaternion.Lerp(Selected.localRotation, targetRot, Time.deltaTime * rotationLerpSpeed);
        }
    }

    private bool HasMoved()
    {
        //If there is no transform threshold
        if (positionThreshold == 0)
        {
            return true;
        }
        else
        {
            if(!Local)
            {
                if (Vector3.Distance(prevPos, Selected.position) >= positionThreshold)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }else
            {
                if (Vector3.Distance(prevPos, Selected.localPosition) >= positionThreshold)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

    private bool HasRotated()
    {
        //If there is no transform threshold
        if (rotationThreshold == 0)
        {
            return true;
        }
        else
        {
            if(!Local)
            {
                if(GetQuaternionDistance(prevRot, Selected.rotation) >= rotationThreshold)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }else
            {
                if(GetQuaternionDistance(prevRot, Selected.localRotation) >= rotationThreshold)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            
        }
    }

    private float RoundTo2(float value)
    {
        return (Mathf.Round(value * 100f) * 0.01f);
    }

    private float UpdatedTime()
    {
        return RoundTo2((float)(NetworkTime.time + NetworkTime.rtt));
    }

    private float GetQuaternionDistance(Quaternion from, Quaternion to)
    {
        //To be implemented
        return Quaternion.Angle(from, to);
    }

    [Command]
    private void CmdSendPosition(Vector3 pos, float timestamp)
    {
        //Debug.Log("SENT POSITION : " + gameObject.name);

        //send updated position
        newPos = pos;

        //set last updated time
        lastUpdate = timestamp;

    }

    [Command]
    private void CmdSendRotation(Quaternion rot, float timestamp)
    {
        //Debug.Log("SENT ROTATION : " + gameObject.name);
        //send updated rotation
        newRot = rot;

        //set last updated time
        lastUpdate = timestamp;

    }
}
