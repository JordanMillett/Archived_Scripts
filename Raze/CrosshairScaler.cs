using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairScaler : MonoBehaviour
{
    [Range(0.0f, 100.0f)]
    public float Accuracy;

    public float ZeroScale;
    public float HundredScale;

    RectTransform T;
    float Scale = 0.5f;


    void Start()
    {

        T = GetComponent<RectTransform>();

    }

    void Update()
    {
        Scale = Mathf.Lerp(ZeroScale, HundredScale, Accuracy/100f);
        //Scale = 100f - (Accuracy/2f);
        //Scale *= 0.01f;
        //Scale /= 75f;
        //Scale = 0.5f;
        //Scale = (((100f - Accuracy)/50f) + .25f));
        T.localScale = new Vector3(Scale, Scale, Scale);

    }
}
