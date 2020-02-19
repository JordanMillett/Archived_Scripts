using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeSwing : MonoBehaviour
{

    Animator an;

    void Start()
    {
        an = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RotateWeapon();
        }
    }

    void RotateWeapon()
    {

        

        float sens = 3f;

        float XInput = Input.GetAxis ("Mouse X") * sens;
        float YInput = Input.GetAxis ("Mouse Y") * sens;
       

        this.transform.eulerAngles += new Vector3(YInput, XInput, 0f);

    }
}
