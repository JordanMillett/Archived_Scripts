using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour
{

    [System.Serializable]
    public struct Createable
    {
	    public string Name;
        public Texture Icon;
        public int Money;
        public int Oil;
        public int Metal;
        public float CraftTime;
        public GameObject Prefab;
    }

    public GameObject Canvas;
    public Createable[] Units;
    public Transform Spawn;
    public GameObject Destination;

    bool Toggled = false;
    Camera cam;
    MouseControls Mc;
    ResourceController Rc;

    int canDisable = 0;


    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Mc = cam.transform.GetComponent<MouseControls>();
        Rc = cam.transform.GetComponent<ResourceController>();
        Canvas.SetActive(false);
        Destination.SetActive(false);
    }

    void Update()
    {

        if(Toggled && Input.GetMouseButtonDown(1))
        {

            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
                Destination.transform.position = hit.point;

        }

        if(Toggled && Input.GetMouseButtonUp(0))
        {
            if(canDisable < 2)
                canDisable++;
            else
            {
                Toggle();
            }
        }

      
    }

    public void Toggle()
    {
        canDisable = 0;

        Toggled = !Toggled;

        Canvas.SetActive(Toggled);
        Destination.SetActive(Toggled);

    }

    public void MakeUnit(int Index)
    {
        canDisable = 0;

        if(canMake(Index))
        {
            GameObject Un = Instantiate(Units[Index].Prefab, Spawn.position, Quaternion.identity);
            Unit U = Un.GetComponent<Unit>();
            U.Team = 0;
            Mc.Units.Add(U);
            StartCoroutine(Move(U));
        }

    }

    bool canMake(int Index)
    {
        int Money = Rc.Money - Units[Index].Money;
        int Oil = Rc.Oil - Units[Index].Oil;
        int Metal = Rc.Metal - Units[Index].Metal;

        if(Money >= 0 && Oil >= 0 && Metal >= 0)
        {

            Rc.Change(0, -Units[Index].Money);
            Rc.Change(1, -Units[Index].Oil);
            Rc.Change(2, -Units[Index].Metal);

            return true;

        }else
        {
            return false;
        }
        

    }

    IEnumerator Move(Unit U)
    {

        yield return null;

        U.GotoPathFinding(Destination.transform.position, true);

    }
}
