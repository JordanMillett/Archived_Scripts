using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianAI : MonoBehaviour
{
    public SkinnedMeshRenderer SMR_model;
    public SkinnedMeshRenderer SMR_ragdoll;
    public BodyPositions BP_model;
    public BodyPositions BP_ragdollModel;
    public Animator _animator;

    float maxSpeed = 1f;
    float movementForce = 300f;
    float killForce = 35f;
    
    bool dead = false;

    Vector3 lastlastGoal = Vector3.zero;
    Vector3 lastGoal = Vector3.zero;
    Vector3 currentGoal = Vector3.zero;

    Vector3 builtInOffset = Vector3.zero;

    Rigidbody _rigidbody;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        movementForce += Random.Range(0f, 100f);

        float offset = 0.55f;

        builtInOffset = new Vector3(Random.Range(-offset, offset), 0f, Random.Range(-offset, offset));

        RandomizeAppearance();
        currentGoal = FindNewTarget();
    }

    void FixedUpdate()
	{   
        if(!dead)
        {
            _animator.SetBool("Walking", true);

            if(_rigidbody.velocity.magnitude < maxSpeed)
            {
                _rigidbody.AddForce(transform.forward * movementForce);
            }
            
            if(Vector3.Distance(this.transform.position, currentGoal + builtInOffset) < 1f)
            {
                Vector3 past = currentGoal;
                currentGoal = FindNewTarget();
                lastlastGoal = lastGoal;
                lastGoal = past;
            }else
            {
                Vector3 Target = (currentGoal + builtInOffset) - this.transform.position;
                Quaternion newRotation = Quaternion.LookRotation(Target, Vector3.up);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, Time.fixedDeltaTime * 3f);
            }

            
        }
    }

    Vector3 FindNewTarget()
    {
        float ClosestDistance = 100f;
        int ClosestIndex = 0;
        int layerMask = LayerMask.GetMask("Waypoint");
        bool found = false;

        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 12f, layerMask);
        for(int i = 0; i < hitColliders.Length; i++)
        {
            if(hitColliders[i].gameObject.CompareTag("PedWaypoint"))
            {
                Vector3 pointPos = hitColliders[i].transform.position;
                if(pointPos != currentGoal && pointPos != lastGoal && pointPos != lastlastGoal)
                {
                    float dist = Vector3.Distance(this.transform.position, pointPos);
                    if(dist < ClosestDistance)
                    {
                        //Debug.Log(hitColliders[ClosestIndex].transform.position);
                        found = true;
                        ClosestDistance = dist;
                        ClosestIndex = i;
                    }
                }
            }
        }

        if(found)
        {
            return hitColliders[ClosestIndex].transform.position;
        }

        return lastGoal;

    }

    void RandomizeAppearance()
    {
        Material[] Mats = SMR_model.materials;
        Material[] RagdollMats = SMR_ragdoll.materials;

        Color SkinColor = Random.ColorHSV(0f, 1f, 0.5f, 0.6f, 0.6f, 0.9f);
        Color ShirtColor = Random.ColorHSV(0f, 1f, 0.5f, 0.6f, 0.6f, 0.9f);
        Color PantsColor = Random.ColorHSV(0f, 1f, 0.5f, 0.6f, 0.6f, 0.9f);

        Texture2D Face = Client.Instance.GetRandomFace();

        Mats[0].SetColor("_albedo", SkinColor);
        Mats[1].SetColor("_albedo", ShirtColor);
        Mats[2].SetColor("_albedo", PantsColor);
        Mats[3].SetColor("_albedo", SkinColor);
        Mats[3].SetTexture("_face", Face);
        
        RagdollMats[0].SetColor("_albedo", SkinColor);
        RagdollMats[1].SetColor("_albedo", ShirtColor);
        RagdollMats[2].SetColor("_albedo", PantsColor);
        RagdollMats[3].SetColor("_albedo", SkinColor);
        RagdollMats[3].SetTexture("_face", Face);
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.impulse.magnitude > killForce || col.gameObject.CompareTag("Hazard"))
        {
            Die();
        }
    }

    public void Die()
    {
        if(!dead)
        {
            dead = true;
            Collider[] AllColliders = this.transform.GetComponentsInChildren<Collider>();
            BP_model.gameObject.SetActive(false);
            SyncRagdoll();
            AllColliders[0].enabled = false;
            BP_ragdollModel.gameObject.SetActive(true);

            Invoke("Despawn", 8f);  
        }
    }

    void SyncRagdoll()
    {
        for(int i = 0; i < BP_ragdollModel.Syncs.Count; i++)
        {
            BP_ragdollModel.Syncs[i].position = BP_model.Syncs[i].position;
            BP_ragdollModel.Syncs[i].rotation = BP_model.Syncs[i].rotation;
            if(BP_ragdollModel.Syncs[i].GetComponent<Rigidbody>() != null)
            {
                BP_ragdollModel.Syncs[i].GetComponent<Rigidbody>().velocity = _rigidbody.velocity;
                BP_ragdollModel.Syncs[i].GetComponent<Rigidbody>().angularVelocity = _rigidbody.angularVelocity;
            }
        }
    }

    void Despawn()
    {
        Destroy(this.gameObject);
    }
}
