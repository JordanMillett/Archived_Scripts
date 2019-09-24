using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {

	float speed = 5;
	Animator an;
	Rigidbody r;
	public GameObject ai;

	// Use this for initialization
	void Start () {
		r = GetComponent<Rigidbody>();
		an = ai.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

		/* if (Input.GetKey("1"))
		{
			an.SetBool("lmb",true);
		}else
		{
			an.SetBool("lmb",false);
		}*/

		if (Input.GetKey("f"))
		{
			an.SetBool("Moving",true);
			speed = 5;
		}else
		{
			an.SetBool("Moving",false);
			speed = 0;
		}

        if (Input.GetKey("z"))
        {
            an.SetBool("sal", true);
        }
        else
            an.SetBool("sal", false);

        r.velocity = transform.forward * speed;
	}
}
