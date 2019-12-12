using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour
{
    GameObject PopupObject;
    bool Active = false;
    Transform Camera;

    float Scale = 0f;

    void Start()
    {
        Camera = GameObject.FindWithTag("MainCamera").transform;
        PopupObject = this.transform.GetChild(0).gameObject;
    }

    void Update()
    {

        if(Active)
        {
            if(Scale < 1f)
                Scale += 0.05f;

            PopupObject.transform.LookAt(Camera.position);
            transform.parent.localScale = new Vector3(Scale, Scale, 1f);
        }
        else
        {

            Scale = 0f;

        }

        PopupObject.SetActive(Active);
        Active = false;

    }

    public void Activate()
    {
        Active = true;
    }
}
