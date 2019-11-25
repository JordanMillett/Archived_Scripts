using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabManager : MonoBehaviour
{

    public List<GameObject> Tabs = new List<GameObject>();

    void Start()
    {
        /* 
        foreach(Transform child in this.transform)
        {

            Tabs.Add(child.gameObject);

        }*/

        //ChangeTab(0);

    }

    public void ChangeTab(int Index)
    {

        for(int i = 0; i < Tabs.Count;i++)
        {

            if(Index == i)
                Tabs[Index].SetActive(true);
            else
                Tabs[Index].SetActive(false);

        }

    }

}
