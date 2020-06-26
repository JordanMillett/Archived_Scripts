using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplacementShaderTest : MonoBehaviour
{
    public Shader ReplaceShader;
    
    Camera Cam;

    bool Toggled = false;

    void Start()
    {
        Cam = GetComponent<Camera>();
    }

    void Update()
    {
        if(Input.GetKeyDown("p"))
            Toggle();
    }

    void Toggle()
    {

        Toggled = !Toggled;

        

        if(Toggled)
            Cam.SetReplacementShader(ReplaceShader, "");
        else
            Cam.ResetReplacementShader();

    }
}
