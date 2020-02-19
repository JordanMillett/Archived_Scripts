using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    RawImage Mask;
    Color newColor;
    float lerp = 1f;

    void Start()
    {
        Mask = GetComponent<RawImage>();
        newColor = Mask.color;
        newColor.a = lerp;
        Mask.color = newColor;

    }

    void Update()
    {

        
        if(lerp >= 0f)
        {
            
            newColor.a = lerp;
            Mask.color = newColor;

            lerp -= 0.01f;
            
        }else
        {

            Destroy(this.gameObject);

        }
        

    }

    
}
