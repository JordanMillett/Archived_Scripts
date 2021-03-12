using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorButton : MonoBehaviour
{
    public CustomizationMenu CM;
    public CarMenu CaM;
    /*
    void Start()
    {
        Debug.Log(ColorUtility.ToHtmlStringRGB(GetComponent<RawImage>().color));
    }*/

    public void AssignTheColor()
    {
        if(CM)
            CM.SetColor(GetComponent<RawImage>().color);
        //else
            //CaM.SetColor(GetComponent<RawImage>().color);
    }
}
