using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilController : MonoBehaviour
{
    float recoil = 0;
    public float recoilSpeed;

    void Update()
    {

        if(recoil > 0)
        {
            this.transform.eulerAngles += new Vector3(recoil,0f,0f);
            //recoil -= recoilSpeed;
        }

    }

    public void AddRecoil(float Amount)
    {

        recoil += Amount;

    }
}
