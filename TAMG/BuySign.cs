using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuySign : MonoBehaviour
{
    public Property Connected;

    public void Toggle()
    {
        Connected.SpawnInterior();
        Destroy(this.gameObject);
    }
}
