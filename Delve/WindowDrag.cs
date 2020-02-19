using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WindowDrag : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isOver = false;
    public bool isTracking = false;

    RectTransform RC;

    Vector3 startPosition = Vector3.zero;
    Vector3 startMouse = Vector3.zero;

    void Start()
    {

        RC = GetComponent<RectTransform>();

    }

    void Update()
    {

        if(isOver)
        {
            if(Input.GetMouseButtonDown(0))
            {
                transform.SetSiblingIndex(1);
                //startPosition = Input.mousePosition;
                //startPosition = RC.anchoredPosition;
                //Vector2 localPoint;
                //RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), RC.anchoredPosition, null, out localPoint);
                //Vector2 normalizedPoint = Rect.PointToNormalized(transform.parent.GetComponent<RectTransform>().rect, localPoint);
                //startPosition = new Vector2(normalizedPoint.x * Screen.width, normalizedPoint.y * Screen.height);
                //startPosition = new Vector2(localPoint.x * Screen.width, localPoint.y * Screen.height);

                //startPosition = RC.anchoredPosition;
                
                //startPosition = new Vector2((Screen.width + RC.anchoredPosition.x)/2f, (Screen.height + RC.anchoredPosition.y)/2f);

                startPosition = RC.anchoredPosition + new Vector2(640, 360);

                //Vector2 localPoint;
                //RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), RC.anchoredPosition, null, out localPoint);
                //startPosition += (Vector3) localPoint /2f ;
                //startPosition = (Vector3) localPoint/2f;

                startMouse = Input.mousePosition;

                //Vector2 localPoint;
                //RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), startMouse , null, out localPoint);
                //startMouse = startPosition;
                //startPosition += (startPosition + startMouse);
                

                //Vector2 offset;
                //RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, null, out offset);
                //normalizedPoint = Rect.PointToNormalized(transform.parent.GetComponent<RectTransform>().rect, localPoint);


                //startPosition += new Vector3(offset.x, 0f, 0f);

                //Debug.Log(startPosition);
                isTracking = true;
            }
        }

        if(isTracking)
        {
            if(Input.GetMouseButtonUp(0) || Input.GetKey(KeyCode.Tab))
            {
                startPosition = Vector3.zero;
                startMouse = Vector3.zero;
                isTracking = false;
            }else
            {

                Vector2 offset;

                offset = Input.mousePosition - startMouse;
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), offset, null, out localPoint);

                //Debug.Log(offset + " == " + localPoint);

                //Vector2 normalizedPoint = Rect.PointToNormalized(transform.parent.GetComponent<RectTransform>().rect, offset);
                //offset = new Vector2(normalizedPoint.x * Screen.width, normalizedPoint.y * Screen.height);
                //startPosition += Input.mousePosition;
                //RC.anchoredPosition = Input.mousePosition - startPosition;
                //Debug.Log(startPosition + " - " + offset);
                //RC.anchoredPosition = offset;
                //RC.anchoredPosition = localPoint;
                RC.anchoredPosition = (Vector2) startPosition + (Vector2) localPoint;
                //RC.anchoredPosition = (Vector2) startPosition + (Vector2) offset;
                //RC.anchoredPosition = Input.mousePosition + startPosition;
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
