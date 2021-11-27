using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HDScreenshot : MonoBehaviour
{
    public int SuperAmount = 1;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            ScreenCapture.CaptureScreenshot("NewestScreenshot.png", SuperAmount);
            Debug.Log("Screenshot Taken");
        }
    }
}
