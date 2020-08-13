using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverviewCull : MonoBehaviour
{
    
    public GameObject Icon;
    public GameObject Canvas;
    GameObject Player;
    GameObject Model;

    float Distance = 0f;

    void Start()
    {
        Model = transform.GetChild(0).gameObject;
        Player = GameObject.FindWithTag("Player").gameObject;
    }

    void Update()
    {
        Distance = Vector3.Distance(Player.transform.position, this.transform.position);

        if(Distance > UniversalConstants.CullDistance)
        {
            Model.SetActive(false);
            Icon.SetActive(true);
            Canvas.SetActive(false);
        }else
        {
            Model.SetActive(true);
            Icon.SetActive(false);
            Canvas.SetActive(true);
        }
    }
}
