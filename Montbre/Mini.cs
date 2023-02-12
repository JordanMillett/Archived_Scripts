using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mini : MonoBehaviour
{
    public Unit U;

    MeshRenderer _meshRenderer;
    SphereCollider _collider;

    void Start()
    {
        _collider = GetComponent<SphereCollider>();
        _collider.enabled = false;

        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.materials[0].SetFloat("_Team", U.Team == Game.TeamOne ? 1f : 0f);
        _meshRenderer.enabled = false;
    }

    void Update()
    {
        if(U.Team == Game.TeamOne)
        {
            if(U.Type == Unit.Types.Infantry)
            {
                _collider.enabled = U.Targetable;
                _meshRenderer.enabled = U.Targetable;
                
                _meshRenderer.materials[0].SetInt("_Flashing", U.inf.State == Infantry.States.Fighting ? 1 : 0);
            }else if(U.Type == Unit.Types.Tank)
            {
                _collider.enabled = U.Targetable;
                _meshRenderer.enabled = U.Targetable;
                
                _meshRenderer.materials[0].SetInt("_Flashing", U.tan.State == Tank.States.Fighting ? 1 : 0);
            }else if(U.Type == Unit.Types.Plane)
            {
                _collider.enabled = U.Targetable;
                _meshRenderer.enabled = U.Targetable;
                
                _meshRenderer.materials[0].SetInt("_Flashing", (U.pla.State == Plane.States.Attack || U.pla.State == Plane.States.Strafe) ? 1 : 0);
            }
        }else
        {
            if (U.Type == Unit.Types.Infantry)
            {
                _meshRenderer.enabled = U.inf.State == Infantry.States.Fighting;
            }else if(U.Type == Unit.Types.Tank)
            {
                _meshRenderer.enabled = U.tan.State == Tank.States.Fighting;
            }else if(U.Type == Unit.Types.Plane)
            {
                _meshRenderer.enabled = false;
                //_meshRenderer.enabled = (U.pla.State == Plane.States.Attack || U.pla.State == Plane.States.Strafe);
            }
        }
        /*
        Mini.materials[0].SetFloat("_Team", U.Team == Game.TeamOne ? 1f : 0f);
        if(DroppingIn)
            Mini.gameObject.SetActive(false);

        */
    }
}
