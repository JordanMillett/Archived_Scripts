using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arrow : MonoBehaviour
{
    public bool Higher = false;

    RawImage ArrowImage;

    public Color HigherColor;
    public Color LowerColor;

    void Start()
    {
        ArrowImage = GetComponent<RawImage>();
        Refresh();

    }

    public void Refresh()
    {

        if(Higher)
        {

            ArrowImage.color = HigherColor;
            this.transform.localEulerAngles = new Vector3(0f,0f,0f);

        }else
        {

            ArrowImage.color = LowerColor;
            this.transform.localEulerAngles = new Vector3(0f,0f,180f);

        }

    }

    
}
