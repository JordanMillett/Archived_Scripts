using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour
{
    public GameObject Shield;
    public GameObject Sword;

    public Vector3 blockDestination;

    //public Vector3 swingPositionDestination;
    //public Vector3 swingRotationDestination;

    public float blockSpeed = 0.01f;

    //public float swingSpeed = 0.01f;

    public bool Blocking = false;
    public bool Attacking = false;

    Vector3 blockOrigin = Vector3.zero;
    Vector3 blockOffset = Vector3.zero;

    //Vector3 swordPositionOrigin = Vector3.zero;
    //Vector3 swordRotationOrigin = Vector3.zero;

   // Vector3 swordPositionOffset = Vector3.zero;
    //Vector3 swordRotationOffset = Vector3.zero;
    
    //float attackLerp = 0;
    float blockLerp = 0;
    
    PlayerController PC;

    public AnimationCurve HeadbobCurve;
    float HeadbobAlpha = 0f;
    float HeadbobAmplitude = -.005f;
    float HeadbobOffset = 0f;
    float HeadbobTime = 0f;

    GameObject Cam;

    Animator swordAnimator;

    void Start()
    {
        Cam = this.gameObject.transform.GetChild(0).gameObject;
        PC = GetComponent<PlayerController>();
        swordAnimator = Sword.transform.GetChild(0).GetComponent<Animator>();
    }

    void Update()
    {
        if(!PC.Frozen)
        {
            Bob();

            Attack();
            Block();
         
        }

        
    }

    void Bob()
    {

        if(HeadbobTime < 1f)
            HeadbobTime += 0.04f * PC.SpeedMult;
        else
            HeadbobTime = 0f;


        if(PC.Moving && PC.isGrounded())
        {

            HeadbobOffset = Mathf.Lerp(HeadbobOffset, (1f - HeadbobCurve.Evaluate(HeadbobTime)) * HeadbobAmplitude * PC.SpeedMult, HeadbobAlpha);
            if(HeadbobAlpha < 1f)
                HeadbobAlpha += 0.04f;

        }else
        {

            HeadbobOffset = Mathf.Lerp(HeadbobOffset, 0f, 1f - HeadbobAlpha);
            if(HeadbobAlpha > 0f)
                HeadbobAlpha -= 0.04f;
        

        }

        Sword.transform.localPosition = new Vector3(0f, HeadbobOffset, 0f);
        //transform.localPosition = new Vector3(0f, HeadbobOffset, 0f);
      

    }

    void Attack()
    {

        if(Input.GetMouseButton(0) && !Blocking && !Attacking)
        {

            StartCoroutine(Swing());
            StartCoroutine(Damage());

        }

    }

    IEnumerator Damage()
    {

        yield return new WaitForSeconds(0.4f);
        RaycastHit hit;
                        
        if(Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit, 2f))
        {
        
            if(hit.transform.gameObject.GetComponent<EnemyStats>() != null)
            {
                hit.transform.gameObject.GetComponent<EnemyStats>().Damage(0f);
            }

        }

    }

    void Block()
    {

        if(Input.GetMouseButton(1) && !Attacking)
        {
            Blocking = true;
            
            blockOffset = Vector3.Lerp(blockOrigin, blockDestination, blockLerp);
            if(blockLerp < 1f)
                blockLerp += blockSpeed;

        }else
        {
            Blocking = false;

            blockOffset = Vector3.Lerp(blockDestination, blockOrigin, 1f - blockLerp);
            if(blockLerp > 0f)
                blockLerp -= blockSpeed;
        }

        Shield.transform.localPosition = blockOffset + new Vector3(0f, HeadbobOffset, 0f);

    }

    IEnumerator Swing()
    {

        Attacking = true;

        swordAnimator.SetInteger("attackState", Random.Range(1,3));

        yield return new WaitForSeconds(0.1f);

        swordAnimator.SetInteger("attackState", 0);

        yield return new WaitForSeconds(swordAnimator.GetCurrentAnimatorStateInfo(0).length);
            
        Attacking = false;

        /*
        while(attackLerp < 1f)
        {
            swordPositionOffset = Vector3.Lerp(swordPositionOrigin, swingPositionDestination, attackLerp);
            swordRotationOffset = Vector3.Lerp(swordRotationOrigin, swingRotationDestination, attackLerp);

            attackLerp += swingSpeed;

            Sword.transform.localPosition = swordPositionOffset + new Vector3(0f, HeadbobOffset, 0f);
            Sword.transform.localEulerAngles = swordRotationOffset;

            yield return null;
        }

        //Debug.Log("Hit");

        while(attackLerp > 0f)
        {
            swordPositionOffset = Vector3.Lerp(swingPositionDestination, swordPositionOrigin, 1f - attackLerp);
            swordRotationOffset = Vector3.Lerp(swingRotationDestination, swordRotationOrigin, 1f - attackLerp);

            attackLerp -= swingSpeed/2f;

            Sword.transform.localPosition = swordPositionOffset + new Vector3(0f, HeadbobOffset, 0f);
            Sword.transform.localEulerAngles = swordRotationOffset;

            yield return null;
        }*/

        
                
    /*
        }else
        

        yield return new WaitForSeconds(1f);
        Attacking = false;*/

    }
    /*
    
    IEnumerator Block()
    {
        
        Blocking = true;
        //for(int i = 0; i < )
        yield return new WaitForSeconds(1f);
        Blocking = false;

    }*/
    
}
