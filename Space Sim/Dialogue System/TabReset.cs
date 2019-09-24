using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabReset : MonoBehaviour
{

    void OnEnable()
    {
        
        Transform C = this.gameObject.transform.GetChild(0).transform;

        for(int i = 0; i < C.childCount;i++)
        {
            if(i == 0)
                C.GetChild(i).gameObject.SetActive(true);
            else
                C.GetChild(i).gameObject.SetActive(false);
        }
        
        

    }
}
