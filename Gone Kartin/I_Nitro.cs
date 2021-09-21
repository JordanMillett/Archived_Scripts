using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class I_Nitro : MonoBehaviour
{
    public float BoostForce;

    public void Use()
    {
        this.transform.GetComponent<ItemInvoker>().KC.GetComponent<Rigidbody>().AddForce(this.transform.GetComponent<ItemInvoker>().KC.transform.forward * BoostForce);
        this.transform.GetComponent<ItemInvoker>().KC.ItemReference = null;
        Destroy(this.gameObject);
    }
}
