using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EQ_Phone : MonoBehaviour
{
    public GameObject PhoneCam;

    public void ToggleOn()
    {
        PhoneCam.SetActive(true);
    }
}
