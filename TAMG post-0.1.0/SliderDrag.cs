using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderDrag : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isOver = false;
    public bool isTracking = false;

    RectTransform RC;

    float startPosition = 0f;
    float startMouse = 0f;

    float offset = 0f;

    IntSlider IS;

    void Start()
    {
        IS = this.transform.parent.GetComponent<IntSlider>();
        RC = GetComponent<RectTransform>();
    }

    void Update()
    {
        if(isOver)
        {
            if(Input.GetMouseButtonDown(0))
            {
                startPosition = RC.anchoredPosition.x + (RC.sizeDelta.x/2f);
                startMouse = Input.mousePosition.x - offset;
                //startMouse = Input.mousePosition.x;
                isTracking = true;
            }
        }

        if(isTracking)
        {
            if(Input.GetMouseButtonUp(0))
            {
                startPosition = 0f;
                startMouse = 0f;
                isTracking = false;
            }else
            {
                //Vector2 localPoint;
                //RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.GetComponent<RectTransform>(), new Vector2(offset, 0f), null, out localPoint);
                //RC.anchoredPosition = (Vector2) startPosition + (Vector2) localPoint;

                offset = Input.mousePosition.x - startMouse;

                IS.UpdatePosition(offset);
            }
        }

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
