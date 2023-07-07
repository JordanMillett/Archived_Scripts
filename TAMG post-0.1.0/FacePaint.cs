using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FacePaint : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RawImage Painted;

    //public RawImage BG;

    public Texture2D Face;
    public Texture2D DefaultFace;

    public CustomizationMenu CM;

    bool isOver = false;

    void Update()
    {
        if(isOver)
        {
            if(Input.GetMouseButton(0))
            {
                PaintPixel(Input.mousePosition, true);
            }

            if(Input.GetMouseButton(1))
            {
                PaintPixel(Input.mousePosition, false);
            }
        }

    }

    public void Reset()
    {
        Face.SetPixels(DefaultFace.GetPixels());
        Face.Apply();
    }

    public void Clear()
    {
        Color[] blank = new Color[1024];
        Color blankColor = new Color(0f, 0f, 0f, 0f);

        for(int i = 0; i < blank.Length; i++)
            blank[i] = blankColor;

        Face.SetPixels(blank);
        Face.Apply();
    }

    public void PaintPixel(Vector2 clickedPos, bool Value)
    {
        bool canPaint = false;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(Painted.transform.GetComponent<RectTransform>(), clickedPos, null, out clickedPos);

        Vector2 RawImageDimensions = Painted.transform.GetComponent<RectTransform>().sizeDelta;
        Vector2 NormalizedPosition = clickedPos/RawImageDimensions;

        if(NormalizedPosition.x < 0.5f && NormalizedPosition.x > -0.5f)
            if(NormalizedPosition.y < 0.5f && NormalizedPosition.y > -0.5f)
                canPaint = true;

        //Debug.Log(clickedPos);

        int xPixelIndex = (int)Mathf.Floor(Face.width * NormalizedPosition.x);
        int yPixelIndex = (int)Mathf.Floor(Face.height * NormalizedPosition.y);

        xPixelIndex += Face.width/2;
        yPixelIndex += Face.height/2;

        

        if(xPixelIndex > 27)
            xPixelIndex = 27;
        if(xPixelIndex < 3)
            xPixelIndex = 3;

        if(yPixelIndex > 27)
            yPixelIndex = 27;
        if(yPixelIndex < 3)
            yPixelIndex = 3;
        


        //Debug.Log(xPixelIndex + "  " + yPixelIndex);
        if(canPaint)
        {
            if(Value)
            {
                Face.SetPixel(xPixelIndex, yPixelIndex, new Color(0f, 0f, 0f, 1f));
            }else
            {
                Face.SetPixel(xPixelIndex, yPixelIndex, new Color(0f, 0f, 0f, 0f));
            }
            
            Face.Apply(false);
        }
    }

    void OnEnable()
    {
        Painted.texture = Face;
    }

    void OnDisable()
    {
        CM.CustomFace = Face;
        CM.SaveCustomFace();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
    }
 
    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;
    }

    
}