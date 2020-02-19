using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public float Cooldown = 1f;
    public int AnimationCount = 1;

    Animator an;
    PlayerController PC;

    bool Running = false;


    
    void Start()
    {
        PC = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        an = GetComponent<Animator>();
    }

    void Update()
    {
        if(Input.GetMouseButton(0) && !Running)
        {

            StartCoroutine(Swing(Random.Range(1, AnimationCount)));

        }
    }

    IEnumerator Swing(int Index)
    {
        Running = true;
        an.SetInteger("State", Index);
        yield return null;
        an.SetInteger("State", 0);
        yield return new WaitForSeconds(Cooldown);
        Running = false;

    }
}
