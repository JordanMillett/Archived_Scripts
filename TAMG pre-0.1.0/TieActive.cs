using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TieActive : MonoBehaviour
{
    public string FindTag = "";
    public bool Child = false;

    GameObject Tied;

    void OnEnable()
    {
        if(Tied == null)
        {
            if(Child)
            {
                Tied = GameObject.FindWithTag(FindTag).transform.GetChild(0).gameObject;
            }else
            {
                Tied = GameObject.FindWithTag(FindTag).gameObject;
            }
        }else
        {
            Tied.SetActive(true);
        }
    }

    void OnDisable()
    {
        if(Tied != null)
            Tied.SetActive(false);
    }
}
