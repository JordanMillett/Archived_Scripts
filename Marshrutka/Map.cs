using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    //DATATYPES

    //PUBLIC COMPONENTS
    public Transform Spawn;
    public Transform BusStops;

    //PUBLIC VARS
    public TreeGenerator TG;

    //PUBLIC LISTS
    public List<Road_Mesh> Roads;

    //COMPONENTS
    Manager M;
    MenuManager MM;
    ManagerUI MUI;

    //VARS

    //LISTS

    public IEnumerator Load()
    {
        M = GameObject.FindWithTag("Manager").GetComponent<Manager>();
        MM = GameObject.FindWithTag("MenuManager").GetComponent<MenuManager>();
        MUI = MM.GetComponent<ManagerUI>();

        StartCoroutine(TG.Generate());
        while(TG.isWorking)
            yield return null;

        for(int i = 0; i < Roads.Count; i++)
        {
            yield return null;
            yield return null;
            Roads[i].SpawnCars(M);
        }

        yield return null;
        MUI.ShowStartMenu();

        M.GameStarted = true;

        
    }
}