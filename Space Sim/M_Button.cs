using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class M_Button : MonoBehaviour
{

    RawImage Overlay;
    Color C;

    public bool Enabled;

    void Start()
    {
        Overlay = this.transform.GetChild(2).GetComponent<RawImage>();
        C = Overlay.color;
    }

    void Update()
    {
        if(Enabled)
            C.a = 0f;
        else
            C.a = .60f;

        Overlay.color = C;
    }

    public void Toggle()
    {
        Enabled = !Enabled;
    }
}
