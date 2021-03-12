using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPicker : MonoBehaviour
{
    MenuManager MM;

    void Start()
    {
        MM = GameObject.FindWithTag("Camera").GetComponent<MenuManager>();
    }

    public void PlaySound()
    {
        MM.PlaySound();
    }
}
