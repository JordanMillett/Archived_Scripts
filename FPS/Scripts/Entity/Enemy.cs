using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    /* 
    
    Turn on yaw
    look on pitch
    use "camera" for canSee()

    run from grenades
    move towards last known location
    
    make view a cone, raycast connect and then check if certain degree is too far
    */
    LifeManager LM;
    Rigidbody R;
    Weapon W;
    Animator An;
    GameObject Camera;
    NavMeshAgent Agent;

    Vector3 Yaw_Dir;
    Vector3 Pitch_Dir;

    public GameObject Player;

    public float ShootDelay;
    //public float Accuracy; //or use built in weapon accuracy?
    public float MoveSpeed;
    public float FieldOfView;
    public float AttackRange;
    public float ViewRange;
    public float StopRange;
    //if hit need to look at player

    //give up chase after set amount of time
    //detect you when they are shot

    Vector3 ranLoc;
    int ranLocCounter = 125;

    Vector3 LastKnownLocation;

    float CurrentDistance;
    
    bool Moving = false;

    int GiveUpChase = 0;
    int AttentionSpan = 150;

    UIController UIC;

    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        //Destroy(Agent);
        UIC = GameObject.FindWithTag("Pause").GetComponent<UIController>();
        Player = GameObject.FindWithTag("Player").transform.GetChild(0).gameObject;
        Camera = this.transform.GetChild(0).gameObject;
        An = this.transform.GetChild(1).GetComponent<Animator>();
        LM = GetComponent<LifeManager>();
        R = GetComponent<Rigidbody>();
        W = this.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Weapon>();
 
    }

    void Update()
    {
        //LookAt(LastKnownLocation);
        if(!UIC.Paused)
        {

        CurrentDistance = Vector3.Distance(Camera.transform.position, LastKnownLocation);
        
        Animate();

        if(canSee(Player))
        {
            LookAt(LastKnownLocation);
            LastKnownLocation = Player.transform.position;
            AttentionSpan = 1000;

            if(CurrentDistance > StopRange)
                StartCoroutine(WalkTo());
            else
                Moving = false;

            if(CurrentDistance < AttackRange)
                StartCoroutine(Fire());
            
        }else
        {
            
            if(CurrentDistance > 2f && GiveUpChase < AttentionSpan)
            {
                //LookAt(LastKnownLocation);
                StartCoroutine(WalkTo());
                GiveUpChase++;
           
            }else
            {
                //LookAt(LastKnownLocation);
                AttentionSpan = 150;
                Moving = false;
                LastKnownLocation = RandomLocation();
                
                
            }

        }
        }

        /*
        if(!UIC.Paused)
        {

        CurrentDistance = Vector3.Distance(Camera.transform.position, LastKnownLocation);
        
        Animate();

        if(canSee(Player))
        {
            LookAt(LastKnownLocation);
            LastKnownLocation = Player.transform.position;
            AttentionSpan = 1000;

            if(CurrentDistance > StopRange)
                StartCoroutine(MoveTowards());
            else
                Moving = false;

            if(CurrentDistance < AttackRange)
                StartCoroutine(Fire());
            
        }else
        {
            
            if(CurrentDistance > 2f && GiveUpChase < AttentionSpan)
            {
                LookAt(LastKnownLocation);
                StartCoroutine(MoveTowards());
                GiveUpChase++;
           
            }else
            {
                AttentionSpan = 150;
                Moving = false;
                LastKnownLocation = RandomLocation();
                
                
            }

        }
        }*/
        
        

        
    }

    void Animate()
    {
        
        An.SetBool("Walking", Moving);

    }

    IEnumerator MoveTowards()
    {
        yield return new WaitForSeconds(ShootDelay);
        Moving = true;
        R.MovePosition(R.position + (transform.forward * MoveSpeed) * Time.fixedDeltaTime);
    }

    IEnumerator Fire()
    {
        yield return new WaitForSeconds(ShootDelay);
        if(canSee(Player))
            W.Fire();
    }

    Vector3 RandomLocation()
    {
        
        ranLocCounter++;
        if(ranLocCounter > 120)
        {
            GiveUpChase = 0;
            ranLocCounter = 0;
            Vector3 offset = new Vector3(Random.Range(-5f,5f),Camera.transform.position.y,Random.Range(-5f,5f));
            return this.transform.position + offset;


        }else
        {

            //return ranLoc; makes them return to last seen location
            return LastKnownLocation;
            
        }

    }

    IEnumerator WalkTo()
    {

        
        yield return new WaitForSeconds(ShootDelay);
        Moving = true;
        //Animate();
        Agent.SetDestination(LastKnownLocation);

    }

    public void LookAt(Vector3 Pos)
    {

        Yaw_Dir = this.transform.position - Pos;
        Yaw_Dir = new Vector3(Yaw_Dir.x, 0f, Yaw_Dir.z);

        Pitch_Dir = Camera.transform.position - Pos;
        Pitch_Dir = new Vector3(0f , Pitch_Dir.y, Vector3.Distance(Camera.transform.position, Pos));
        
        this.transform.eulerAngles = new Vector3(0f, -Vector3.SignedAngle(Yaw_Dir, Vector3.forward, Vector3.up) + 180f,0f);

        Camera.transform.localEulerAngles = new Vector3(Vector3.SignedAngle(Pitch_Dir, Vector3.forward, Vector3.right), 0f, 0f);

        //Vector3 relativePos = Pos - Camera.transform.position;
        //this.transform.rotation = Quaternion.LookRotation(new Vector3(relativePos.x,Pos.y,relativePos.z), Vector3.up);
        //Camera.transform.rotation = Quaternion.LookRotation(new Vector3(Pos.x,relativePos.y,Pos.z), Vector3.up);

        

    }

    bool canSee(GameObject Target)
    {

        //Vector3 Dir = Camera.transform.forward;

        bool see = false;

        Vector3 Dir = Vector3.zero;

        try{
            Dir = Target.transform.position - Camera.transform.position;
        }catch{return false;};

        float Distance = Vector3.Distance(Target.transform.position, Camera.transform.position);

        //Debug.DrawRay(Camera.transform.position, Dir.normalized * 100f, Color.green);
        Debug.DrawRay(Camera.transform.position, Dir.normalized * 100f, Color.green);
        Debug.DrawRay(Camera.transform.position, Camera.transform.forward * 100f, Color.red);

        RaycastHit hit;             
        if(Physics.Raycast(Camera.transform.position, Dir.normalized, out hit,100f)) //instead of forward, make them not always lookat
            if(hit.transform.gameObject == Target.transform.root.gameObject)
                see = true;
        
        float Angle = Vector3.Angle(Dir.normalized, Camera.transform.forward);

        //Debug.Log(Angle);

        if(see && Angle < FieldOfView && Distance < ViewRange)
            return true;
        else
            return false;
                
    }
}
