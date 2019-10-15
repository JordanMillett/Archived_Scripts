using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyScripts.TeamColors;

public class MouseControls : MonoBehaviour
{

    public Camera CAMERA;
    public GameObject obj;
    public List<Unit> Units;
    public GameObject ToSpawn;

    Camera cam;

    void Start()
    {
        Camera cam = GetComponent<Camera>();
    }

    void Update()
    {

        Command();

        if(Input.GetKeyDown("q"))
            SelectAll();

        if(Input.GetMouseButtonUp(0))
            OpenMenus();

        RaycastHit hit;
        Ray ray = CAMERA.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit))
            obj.transform.position = hit.point;

        if(Input.GetKey("e"))
            Spawn(obj.transform.position);

        for(int i = 0; i < Units.Count;i++)
            if(Units[i].isDead)
                Units.Remove(Units[i]);
        

    }

    void OpenMenus()
    {

        RaycastHit hit;
        Ray ray = CAMERA.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
            if(hit.transform.GetComponent<Factory>() != null)
                hit.transform.GetComponent<Factory>().Toggle();

    }

    public void Spawn(Vector3 Loc)
    {

        GameObject Spawned = Instantiate(ToSpawn, Loc, Quaternion.identity);
        Spawned.GetComponent<Unit>().Team = Random.Range(0,TeamColors.Colors.Length);
        if(Spawned.GetComponent<Unit>().Team == 0)
            Units.Add(Spawned.GetComponent<Unit>());

    }

    public void SelectAll()
    {
        int UnitsSelected = 0;

        foreach(Unit U in Units)
        {
            if(U.Selected)
                UnitsSelected++;
        }

        if(UnitsSelected == Units.Count)
        {
            foreach(Unit U in Units)
            {
                if(U.Selected)
                    U.Select();
            }
        }else
        {
            foreach(Unit U in Units)
            {
            
                if(U.Selected == false)
                    U.Select();

            }
        }

    }

    public void Select(Vector2 x_axis, Vector2 z_axis)
    {

        foreach(Unit U in Units)
        {
            if(U.InRange(x_axis, z_axis) && !U.Selected)
                U.Select();

        }

    }

    void Command()
    {

         if(Input.GetMouseButton(1))
         {

            foreach(Unit U in Units)
            {
                if(U.Selected)
                    U.Target = GetTarget();
                U.GotoPathFinding(obj.transform.position, false);
            }

         }

    }

    Unit GetTarget()
    {

        Collider[] hitColliders = Physics.OverlapSphere(obj.transform.position, 1f);
        foreach(Collider C in hitColliders)
        {
            if(C.GetComponent<Unit>() != null)
                if(C.GetComponent<Unit>().Team != 0)
                    return C.GetComponent<Unit>();
        }

        return null;


    }
}
