using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDisabler : MonoBehaviour 
{

    public GameObject[] Effects;

    public Placing p;

    bool On = true;

	void Update () 
    {
		if(this.transform.parent.gameObject.layer == 10 || p.enabled)
        {

            foreach(GameObject g in Effects)
                g.SetActive(false);

            On = false;

        }else
        {
            if(!On)
            {
                foreach(GameObject g in Effects)
                    g.SetActive(true);

                On = true;
            }

        }
	}
}
