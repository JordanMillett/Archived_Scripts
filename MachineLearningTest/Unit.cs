using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //DATATYPES

    //PUBLIC COMPONENTS

    //PUBLIC VARS
    public Commander _commander;
    public float DetectRadius = 2f;
    public int Health = 4;

    //PUBLIC LISTS
    public List<Material> TeamColors;

    //COMPONENTS
    Rigidbody r;

    //VARS
    IEnumerator MoveRoutine;
    bool isMoving = false;
    float AttackInterval = 0.25f;

    //LISTS

    void Start()
    {
        r = GetComponent<Rigidbody>();
        InvokeRepeating("Attack", AttackInterval, AttackInterval);
    }

    void Update()
    {
        float Dist = Vector3.Distance(this.transform.localPosition, _commander.EnemyFlag.localPosition);

        if(Dist < DetectRadius)
        {
            _commander.Enemy.Lost = true;
        }else
        {
            if(Dist < _commander.ClosestDistance)
            {
                _commander.ClosestDistance = Dist;
            }
        }
    }

    void Attack()
    {
        Collider[] Nearby = Physics.OverlapSphere(this.transform.localPosition, DetectRadius);
        bool AttackSpent = false;
        
        foreach(Collider Col in Nearby)
        {
            if(Col.transform.GetComponent<Unit>() != null && !AttackSpent)
            {
                if(Col.transform.GetComponent<Unit>()._commander != _commander) //if leaders are different
                {
                    Col.transform.GetComponent<Unit>().TakeDamage();
                    AttackSpent = true;
                }
            }
        }
    }

    public void TakeDamage()
    {
        Health--;

        if(Health == 0)
        {
            Reset();
        }
    }

    public void Initialize(Commander C)
    {
        _commander = C;

        this.transform.GetChild(0).GetComponent<MeshRenderer>().material = TeamColors[(int) _commander._team];
        Reset();
    }

    public void Reset()
    {
        Health = 4;
        this.transform.localPosition = _commander.Spawn.localPosition + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));

        if(isMoving)
            StopCoroutine(MoveRoutine);
    }

    public void GoTo(float x, float z)
    {
        if(isMoving)
            StopCoroutine(MoveRoutine);

        MoveRoutine = Travel(x, z);
        StartCoroutine(MoveRoutine);
    }

    IEnumerator Travel(float x, float z)    //DO IT
    {
        Vector3 Start = this.transform.localPosition;
        Vector3 End = new Vector3(x, Start.y, z);

        yield return null;
        while(Vector3.Distance(Start, End) > 0.5f)
        {
            r.AddForce(Vector3.Normalize(End - Start) * 1f);
            yield return null;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(Color.white.r, Color.white.g, Color.white.b, 0.1f);
        Gizmos.DrawSphere(this.transform.localPosition, DetectRadius);
    }
}