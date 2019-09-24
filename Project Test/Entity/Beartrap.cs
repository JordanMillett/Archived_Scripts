using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beartrap : MonoBehaviour 
{

    Animator an;

    bool Activated = false;


	void Start () 
    {
		an = this.transform.GetChild(0).gameObject.GetComponent<Animator>();
    }

    void OnCollisionEnter(Collision col)
    {

        if(!Activated)
        {
            Activated = true;
            an.SetBool("Open",true);
    
            try
            {
                Stats Entity = col.gameObject.GetComponent<Stats>();

                if(Entity != null)
                    Entity.Health -= 50;

                Life_Manager Player = col.gameObject.GetComponent<Life_Manager>();
        
                if(Player != null)
                    Player.Health -= 50;
                

            }
            catch {}

            StartCoroutine(Reset());

        }



    }

    IEnumerator Reset()
    {

        yield return new WaitForSeconds(5f);
        an.SetBool("Open",false);
        yield return new WaitForSeconds(an.GetCurrentAnimatorStateInfo(0).length);
        Activated = false;

    }

}
