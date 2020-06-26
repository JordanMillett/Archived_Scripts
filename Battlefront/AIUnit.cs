using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Jello;

public class AIUnit : MonoBehaviour
{
    Rigidbody _rigidbody;
    NavMeshAgent _navMeshAgent;
    AIManager _aiManager;
    UnitStats _unitStats;
    AIUnit currentTarget;

    public List<SkinnedMeshRenderer> _skinnedMeshRenderer;

    public float maxSpeed;
    public float visionDistance;
    public float projectileVelocity;
    public float respawnTime;
    public float reactionTime;

    public int health;
    public int teamIndex = 0;
    int perLifeObjectivePreference = 0;

    public bool moving = false;
    public bool dead = false;

    public GameObject ProjectilePrefab;

    public Material goodMaterial;
    public Material badMaterial;
    
    public AIClass _class;

    void Start()
    {


        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _unitStats = GetComponent<UnitStats>();
        _aiManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<AIManager>();

        _navMeshAgent.avoidancePriority = Random.Range(1,98);

        teamIndex = Random.Range(1,3);
    

        _class = _aiManager.GetClass();
        health = _class.Health;
        respawnTime = _class.RespawnTime;
        

        if(teamIndex == 1)
            foreach(SkinnedMeshRenderer MR in _skinnedMeshRenderer)
                MR.material = goodMaterial;
        else
            foreach(SkinnedMeshRenderer MR in _skinnedMeshRenderer)
                MR.material = badMaterial;

        StartCoroutine(Loop());
    }

    IEnumerator Loop()
    {
        perLifeObjectivePreference = Random.Range(1, 100);
        yield return new WaitForSeconds(Random.Range(0f, 4f));

        while(!dead)
        {   
            if(!FoundEnemy())
            {
                GoToObjectives();
            }else
            {



                //Debug.Log("Enemy Spotted");

                if(Jello.Tools.RandomBool())
                {
                    WalkTo(RandomLocation());
                    
                }else
                {
                    moving = false;
                    _navMeshAgent.isStopped = true;
            
                }

                int Amount = _class.BurstAmount; //4

                for(int i = 0; i < Amount; i++)
                {

                    //yield return new WaitForSeconds(reactionTime/(Amount + 1));
                    yield return null;
                    yield return null;
                    yield return null;
                    LookAt(currentTarget.transform.position);
                    Shoot();

                }

                yield return new WaitForSeconds(reactionTime/4f);

                for(int i = 0; i < Amount; i++)
                {

                    //yield return new WaitForSeconds(reactionTime/(Amount + 1));
                    yield return null;
                    yield return null;
                    yield return null;
                    LookAt(currentTarget.transform.position);
                    Shoot();

                }


                
                /*
                yield return new WaitForSeconds(reactionTime/4f);
                LookAt(currentTarget.transform.position);
                Shoot();

                yield return new WaitForSeconds(reactionTime/4f);
                LookAt(currentTarget.transform.position);
                Shoot();

                yield return new WaitForSeconds(reactionTime/4f);
                LookAt(currentTarget.transform.position);
                Shoot();
                */
                


            }

            yield return new WaitForSeconds(reactionTime);

            
        }
    }

    void GoToObjectives()
    {

        if(_aiManager.FoundUncapturedPosts(teamIndex))
        {
            WalkTo(_aiManager.GetCommandPost(teamIndex, perLifeObjectivePreference));
        }else
        {

            WalkTo(RandomLocation());

        }

    }

    void WalkTo(Vector3 Destination)
    {

        moving = true;

        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(Destination);

    }
    
    bool FoundEnemy()
    {

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, visionDistance);
        for(int i = 0; i < hitColliders.Length; i++)
        {
            if(hitColliders[i].gameObject.GetComponent<AIUnit>() != null)
            {

                if(hitColliders[i].gameObject.GetComponent<AIUnit>().teamIndex != teamIndex && SeeTarget(hitColliders[i].gameObject.GetComponent<AIUnit>()))
                {
                    
                    currentTarget = hitColliders[i].gameObject.GetComponent<AIUnit>();
                    

                    return true;

                }


            }
        }

        return false;

    }

    void LookAt(Vector3 Pos)
    {
        Vector3 yaw = Vector3.zero;

        yaw = this.transform.position - Pos;
        yaw = new Vector3(yaw.x, 0f, yaw.z);
        
        this.transform.eulerAngles = new Vector3(0f, -Vector3.SignedAngle(yaw, Vector3.forward, Vector3.up) + 180f, 0f);
    }

    bool SeeTarget(AIUnit Target)
    {

        

        RaycastHit hit;
        Vector3 FirePosition = transform.position + new Vector3(0f, 2f, 0f);
        Vector3 TargetPosition = Target.transform.position + new Vector3(0f, 2f, 0f);

        Vector3 Dir = TargetPosition - FirePosition;

        
                            
        if(Physics.Raycast(FirePosition, Dir.normalized, out hit, 100f))
        {
            if(hit.transform.gameObject.GetComponent<AIUnit>() == Target)
            {
                //Debug.DrawRay(FirePosition, Dir.normalized * 100f, Color.green);
                return true;

            }
        }

        //Debug.DrawRay(FirePosition, Dir.normalized * 100f, Color.red);
        return false;

    }

    void Shoot()
    {
        
        RaycastHit hit;
        Vector3 FirePosition = transform.position + new Vector3(0f, 2f, 0f);

        float Accuracy = _class.Accuracy; //95

        Vector3 OffsetVector = Jello.Tools.GetOffsetVector(Accuracy);
        
        //Vector3 FireVector = transform.forward + OffsetVector;
        Vector3 FireVector = transform.forward + OffsetVector;

                            
        if(Physics.Raycast(FirePosition, FireVector, out hit, 1000f))
        {
            GameObject Projectile;
            Projectile = Instantiate(ProjectilePrefab, FirePosition, this.transform.rotation);
            Travel T = Projectile.GetComponent<Travel>();
            T.Creator = this;

            if(teamIndex == 1)
                T.MatColor = _class.GoodFireColor;
            else
                T.MatColor = _class.BadFireColor;

            T.DamageAmount = _class.DamageStrength;

            T.ColorStrength = _class.FireColorStrength;
            T.MoveVector = FireVector;
            T.Move(hit.point, projectileVelocity, hit.normal);
            
            //Debug.DrawRay(FirePosition, FireVector * 1000f, Color.green);

            //Debug.DrawRay(hit.point,  hit.normal * 25f, Color.red);

            if(hit.transform.gameObject.GetComponent<AIUnit>() != null)
            {
                Projectile.GetComponent<Travel>().HitTarget = hit.transform.gameObject.GetComponent<AIUnit>();
            }

            /*if(hit.transform.gameObject.GetComponent<AIUnit>() != null) instant hits
            {
                hit.transform.gameObject.GetComponent<AIUnit>().Damage();
            }*/

        }else
        {
            GameObject Projectile;
            Projectile = Instantiate(ProjectilePrefab, FirePosition, this.transform.rotation);
            Travel T = Projectile.GetComponent<Travel>();
            T.Creator = this;

            if(teamIndex == 1)
                T.MatColor = _class.GoodFireColor;
            else
                T.MatColor = _class.BadFireColor;

            T.DamageAmount = _class.DamageStrength;

            T.ColorStrength = _class.FireColorStrength;
            T.MoveVector = FireVector;
            T.Move(FirePosition + (FireVector * 1000f), projectileVelocity, hit.normal);
        }
        
    }

    public void Damage(int Amount)
    {

        health -= Amount;

        if(health <= 0)
        {

            Die();

        }

    }

    void Die()
    {

        dead = true;
        this.gameObject.SetActive(false);

        _unitStats.Deaths++;

        Invoke("Respawn", respawnTime);

    }

    void Respawn()
    {

        this.transform.position = _aiManager.GetSpawnPoint(teamIndex);
        health = _class.Health;
        dead = false;
        this.gameObject.SetActive(true);

        StartCoroutine(Loop());

    }

    Vector3 RandomLocation()
    {   
        Vector3 offset = new Vector3(Random.Range(-5f,5f), 0f,Random.Range(-5f,5f));
        return this.transform.position + offset;
    }
}
