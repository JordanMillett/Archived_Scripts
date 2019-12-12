using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnRoom : MonoBehaviour
{
    public GameObject Screen;
    public GameObject Camera;

    public bool Active = false;

    float Alpha = 1f;
    bool reset = false;

    Renderer R;

    void Start()
    {

        R = Screen.GetComponent<Renderer>();
        
        R.material.SetFloat("White", Alpha);

        Screen.SetActive(Active);
        Camera.SetActive(Active);


    }

    void Update()
    {

        if(Active)
        {

            R.material.SetFloat("White", Alpha);

            if(reset)
            {

                Alpha += 0.0025f;


            }else
            {

                if(Alpha >= 0.02f)
                    Alpha -= 0.02f;
                else
                    reset = true;

            }


        }else
        {

            Alpha = 1f;
            reset = false;

        }

    }

    public void Toggle()
    {
        Active = !Active;
        Screen.SetActive(Active);
        Camera.SetActive(Active);

    }
}
