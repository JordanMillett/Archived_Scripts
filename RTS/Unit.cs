using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MyScripts.TeamColors;

public class Unit : MonoBehaviour
{

    public float Health;
    public float Strength;
    public float Range;
    public float AttackSpeed;
    public int Team;
    public float DespawnTime;

    public float DetectDistance;

    public Texture Icon;

    public bool Selected = false;
    public bool useAnimation = false; 
    public bool isDead = false;

    public Unit Target;
    public GameObject Select_Graphic;
    public Animator An;
    public Renderer TeamRenderer;
    public Simple_Bar HealthBar;

    public AudioClip[] DamageSounds;
    public AudioClip[] DeathSounds;

    NavMeshAgent Agent;
    AudioSource Source;

    void Start()
    {
        UpdateColor();
        HealthBar.Max = Health;
        Select_Graphic.SetActive(false);
        Agent = GetComponent<NavMeshAgent>();
        Source = GetComponent<AudioSource>();
        InvokeRepeating("Attack", AttackSpeed, AttackSpeed);
    }

    void Update()
    {
        if(!isDead)   
            if(!Agent.pathPending && !Agent.hasPath)
                if(useAnimation)
                    An.SetBool("Moving", false);

        //Debug.Log(GetTarget().gameObject.name);
        if(!isDead)
        {
            if(Target == null || Target.isDead)
                Target = GetTarget();
            else
            {
                Agent.destination = Target.transform.position;
                
            }
        }
    }

    Unit GetTarget()
    {

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, DetectDistance);

        Unit Closest = null;
        float minddist = DetectDistance;

        foreach(Collider C in hitColliders)
        {
            if(C.GetComponent<Unit>() != null)
                if(!C.GetComponent<Unit>().isDead && C.GetComponent<Unit>().Team != GetComponent<Unit>().Team)
                    if(Vector3.Distance(this.transform.position, C.gameObject.transform.position) < minddist)
                    {

                        minddist = Vector3.Distance(this.transform.position, C.transform.position);
                        Closest = C.GetComponent<Unit>();

                    }
                    //return C.GetComponent<Unit>();
        }

        if(Closest)
            return Closest;
        else
            return null;

    }

    public void Damage(float Amount)
    {
        if(Health > Amount)
        {
            Source.clip = DamageSounds[Random.Range(0, DamageSounds.Length)];
            if(!Source.isPlaying)
                Source.Play();
            Health -= Amount;
        }
        else
        {
            Source.clip = DeathSounds[Random.Range(0, DeathSounds.Length)];
            if(!Source.isPlaying)
                Source.Play();

            Health = 0;

            if(Selected)
                Select();
            
            isDead = true;

            Destroy(Agent);
            if(useAnimation)
                An.SetBool("Moving", false);
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

            

            StartCoroutine(Despawn());

        }

        HealthBar.Current = Health;

    }

    public void Attack() //.isStopped = true;
    {
        if(Target != null)
            if(Vector3.Distance(this.transform.position, Target.transform.position) < Range)
            {
                //Agent.destination = Target.transform.position;
                this.transform.LookAt(Target.transform.position, Vector3.up);
                Target.Damage(Strength);
                
            }

    }

    public void Select()
    {
        if(Team == 0)
        {
            Selected = !Selected;
            Select_Graphic.SetActive(Selected);
        }

    }

    public bool InRange(Vector2 x_axis, Vector2 z_axis)
    {

        if((this.transform.position.x > x_axis.x) && (this.transform.position.x < x_axis.y))
            if((this.transform.position.z > z_axis.y) && (this.transform.position.z < z_axis.x))
                return true;
                      
        if(Selected)
            Select();
        return false;

    }

    public void GotoPathFinding(Vector3 Pos, bool Override)
    {
        if(Override)
        {

            Agent.destination = Pos;

            if(useAnimation)
                An.SetBool("Moving",true);

        }else 
        {
        
        if(Selected && !isDead)
        {
            Agent.destination = Pos;

            if(useAnimation)
                An.SetBool("Moving",true);
        }

        }

    }

    void UpdateColor()
    {
        Color NewColor;
        if (ColorUtility.TryParseHtmlString(TeamColors.Colors[Team], out NewColor))
            TeamRenderer.material.SetColor("_BaseColor", NewColor);
    }

    IEnumerator Despawn()
    {

        yield return new WaitForSeconds(DespawnTime);
        Destroy(this.gameObject);

    }

}
