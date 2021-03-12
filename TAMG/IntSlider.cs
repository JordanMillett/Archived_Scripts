using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IntSlider : MonoBehaviour
{
    public RectTransform SliderObject;
    public TextMeshProUGUI Display;

    public int Value;

    public Vector2Int ValueBounds = new Vector2Int(10, 200);
    public float SliderScale = 200f;

    public void Initialize(int StartingValue)   //0.10 -> 2.00
    {
        Value = StartingValue;                  //Pass initial value

        //Debug.Log("Starting Value " + Value);
        float alpha = ((float)Value - (float)ValueBounds.x)/((float)ValueBounds.y - (float)ValueBounds.x);  //remap (0.10 -> 2.00) to (0 -> 1)      SENS -> ALPHA
        //Debug.Log("Alpha " + alpha);
        alpha = Mathf.Lerp(-SliderScale/2f, SliderScale/2f, alpha);             //remap (0 -> 1) to (-100 -> 100)       ALPHA -> POS
        //Debug.Log("Local Position " + alpha);
        UpdatePosition(alpha);                                                  //pass local position value to function POS
    }

    public void UpdatePosition(float localOffset)                               //take (-100 -> 100) value              POS
    {   
        //Debug.Log(localOffset);

        if(localOffset > (SliderScale/2f))              //Limit slider bounds
            localOffset = (SliderScale/2f);

        if(localOffset < (-SliderScale/2f))             //Limit slider bounds
            localOffset = (-SliderScale/2f);

        float convertedX = ((localOffset + (SliderScale/2f))/(SliderScale));    //remap (-100 -> 100) to (0 -> 1)       POS -> ALPHA

        Value = Mathf.RoundToInt(Mathf.Lerp(ValueBounds.x, ValueBounds.y, convertedX));        //Assign value to alpha                 SENS

        SliderObject.anchoredPosition = new Vector3(localOffset, 0f, 0f);   //offset pos to slider
        //Value = (((float) Value / 100f)).ToString();
        Display.text = (((float) Value / 100f)).ToString();
    }
}