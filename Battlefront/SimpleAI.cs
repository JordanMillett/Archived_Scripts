using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAI : MonoBehaviour
{
    Rigidbody _rigidbody;
    public List<SkinnedMeshRenderer> _meshRenderer;

    public GameObject ProjectilePrefab;

    public Material Good;
    public Material Bad;

    public float maxSpeed = 7f;
    public float visionDistance = 5f;

    public float projectileVelocity = 15f;

    public bool teamType = false;

    bool walking = false;

    bool Dead = false;

    int Health = 3;

    List<CommandPost> Objectives = new List<CommandPost>();

    Vector3 SpawnPoint = Vector3.zero;

    //AudioClip FireSound;
    //AudioSource Source;

    void Start()
    {

        //Source = GetComponent<AudioSource>();

        SpawnPoint = new Vector3(Random.Range(-50f,50f), 0f, Random.Range(-50f,50f));

        GameObject[] Posts = GameObject.FindGameObjectsWithTag("CommandPost");
        for(int i = 0; i < Posts.Length; i++)
        {
            Objectives.Add(Posts[i].GetComponent<CommandPost>());
        }

        _rigidbody = GetComponent<Rigidbody>();

        teamType = ranBool();

        if(teamType)
            foreach(SkinnedMeshRenderer MR in _meshRenderer)
                MR.material = Good;
        else
            foreach(SkinnedMeshRenderer MR in _meshRenderer)
                MR.material = Bad;

        StartCoroutine(Exist());

    }

    IEnumerator Exist()
    {
        while(true)
        {

            if(!walking && !Dead)
            {
                if(!FoundEnemy())
                {   
                   

                    int UncontrolledCommandPost = GetUncontrolledCommandPost();

                    if(UncontrolledCommandPost == -1)
                    {
                        FaceRandomDirection();
                        yield return new WaitForSeconds(0.5f);
                        StartCoroutine(WalkForward(2f, true));
                    }else
                    {

                        LookAt(Objectives[UncontrolledCommandPost].transform.position);
                        yield return new WaitForSeconds(0.25f);
                        StartCoroutine(WalkForward(3f, true));

                    }

                }else
                {
                    if(ranBool())
                    {

                        StartCoroutine(WalkForward(3f, true));
                        yield return new WaitForSeconds(.5f);
                        Shoot();

                    }else
                    {

                        StartCoroutine(WalkForward(1f, false));
                        yield return new WaitForSeconds(.15f);
                        Shoot();
                        yield return new WaitForSeconds(.15f);
                        Shoot();
                        yield return new WaitForSeconds(.15f);
                        Shoot();

                    }
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    void Shoot()
    {
        if(!Dead)
        {
            //Source.Play();

            RaycastHit hit;
            Vector3 FirePosition = transform.position + new Vector3(0f, 2f, 0f);
                            
            if(Physics.Raycast(FirePosition, transform.forward, out hit, 100f))
            {
                GameObject Projectile;
                Projectile = Instantiate(ProjectilePrefab, FirePosition, Quaternion.identity);
                StartCoroutine(Travel(Projectile, hit.point, projectileVelocity, hit.normal));

                if(hit.transform.gameObject.GetComponent<SimpleAI>() != null)
                {
                    hit.transform.gameObject.GetComponent<SimpleAI>().Die();
                }

            }else
            {
                GameObject Projectile;
                Projectile = Instantiate(ProjectilePrefab, FirePosition, Quaternion.identity);
                StartCoroutine(Travel(Projectile, FirePosition + (transform.forward * 100f), projectileVelocity, hit.normal));
            }
        }
    }

    IEnumerator Travel(GameObject P, Vector3 D, float V, Vector3 N)
    {
        float targetDistance = Vector3.Distance(P.transform.position, D);
        float lerp = 0f;
        Vector3 startPosition = P.transform.position;

        float stopDistance = targetDistance/100f;

        while(lerp <= stopDistance && !Dead)
        {
            lerp += V;
            P.transform.position = Vector3.Lerp(startPosition, startPosition + (transform.forward * 100f), lerp);

            yield return null;
        }

        try
        {
            Destroy(P);
        }catch{}

    }

    int GetUncontrolledCommandPost()
    {

        int TeamID = 1;
        if(!teamType)
            TeamID = 2;

        int shortIndex = -1;
        float shortDistance = 100000;

        for(int i = 0; i < Objectives.Count; i++)
        {
            if(Objectives[i].State != TeamID)
            {
                float newDistance = Vector3.Distance(transform.position, Objectives[i].transform.position);

                if(newDistance < shortDistance)
                {

                    shortDistance = newDistance;
                    shortIndex = i;

                }
            }
        }

        return shortIndex;

    }

    void FaceRandomDirection()
    {

        transform.localEulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);

    }

    IEnumerator WalkForward(float timeWalking, bool direction)
    {

        walking = true;

        Vector3 moveDirection = Vector3.zero;

        if(direction)
            moveDirection = transform.forward;
        else
            moveDirection = -transform.forward;

        float duration = Time.time + timeWalking;
        while (Time.time < duration && !Dead)
        {
            if(_rigidbody.velocity.magnitude < maxSpeed)
                _rigidbody.velocity += moveDirection;

            yield return null;
        }

        walking = false;

    }

    bool FoundEnemy()
    {

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, visionDistance);
        for(int i = 0; i < hitColliders.Length; i++)
        {
            if(hitColliders[i].gameObject.GetComponent<SimpleAI>() != null)
            {

                if(hitColliders[i].gameObject.GetComponent<SimpleAI>().teamType != teamType)
                {

                    LookAt(hitColliders[i].transform.position);

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

    bool ranBool()
    {

        int ran = Random.Range(0, 2);

        if(ran == 0)
            return false;
        else
            return true;

    }

    public void Die()
    {
        Health--;
        if(Health == 0)
        {
            Dead = true;
            StartCoroutine(Despawn());
        }

    }

    IEnumerator Despawn()
    {

        yield return new WaitForSeconds(0.1f);
        GameObject.FindGameObjectWithTag("Spawner").GetComponent<TestSpawner>().Spawn();
        Destroy(this.transform.gameObject);

    }
}
