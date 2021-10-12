using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighLightable : MonoBehaviour
{
    public bool Over = false;
    public MeshRenderer MR;

    float alpha = 0f;

    void Update()
    {
        if(Over)
        {
            if(alpha < 1f)
                alpha += 0.1f;
        }
        else
        {
            if(alpha > 0f)
                alpha -= 0.1f;
        }

        MR.material.SetColor("_BaseColor", Color.Lerp(Color.white, new Color(1f, 1f, 0f), alpha));

        Over = false;
    }
}
