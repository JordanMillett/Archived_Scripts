using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBox : MonoBehaviour
{

    public RectTransform Top;
    public RectTransform Bottom;
    public RectTransform Left;
    public RectTransform Right;

    Vector2 ClickPos;

    Vector2 x_range;
    Vector2 y_range;

    public Camera Cam;

    public GameObject[] Select_Debug_Markers = new GameObject[3];

    public MouseControls Mc;

    void Start()
    {
        Clear();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {

            ClickPos = Input.mousePosition;
            //ClickPos -= new Vector2(1280/2f,720/2f);
            StartCoroutine(MakeBox());

        }

        //Debug.Log(Input.mousePosition);

    }


    IEnumerator MakeBox()
    {

        //Vector2 pos;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out pos);
        //transform.position = myCanvas.transform.TransformPoint(pos);

        //Vector2 localpoint;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, GetComponentInParent<Canvas>().worldCamera, out localpoint);
 
        //Vector2 normalizedPoint = Rect.PointToNormalized(rectTransform.rect, localpoint);

        //RectTransformUtility.ScreenPointToLocalPointInRectangle(Top, Input.mousePosition, out pos);

        while(Input.GetMouseButtonUp(0) == false)
        {
            //x2 to make complete box
            //need to remove ends
            //use simple bars
            
            Top.sizeDelta = new Vector2(Mathf.Abs(ClickPos.x - Input.mousePosition.x),2f); //Thickness
            Top.transform.position = ClickPos;
            Top.anchoredPosition += new Vector2((ClickPos.x - Input.mousePosition.x)/-2f,0f);

            Left.sizeDelta = new Vector2(2f,Mathf.Abs(ClickPos.y - Input.mousePosition.y)); //Thickness
            Left.transform.position = ClickPos;
            Left.anchoredPosition += new Vector2(0f,(ClickPos.y - Input.mousePosition.y)/-2f);

            Right.sizeDelta = new Vector2(2f,Mathf.Abs(ClickPos.y - Input.mousePosition.y)); //Thickness
            Right.transform.position = Input.mousePosition;
            Right.anchoredPosition += new Vector2(0f,(ClickPos.y - Input.mousePosition.y)/2f);

            Bottom.sizeDelta = new Vector2(Mathf.Abs(ClickPos.x - Input.mousePosition.x),2f); //Thickness
            Bottom.transform.position = Input.mousePosition;
            Bottom.anchoredPosition += new Vector2((ClickPos.x - Input.mousePosition.x)/2f,0f);

            yield return null;

        }

        if(Input.GetMouseButtonUp(0)) //use 3 points to get vector2 range for select box in world position
        {
            RaycastHit UL_hit;
            Ray UL_ray = Cam.ScreenPointToRay(ClickPos);
            if (Physics.Raycast(UL_ray, out UL_hit))
            {
                x_range.x = UL_hit.point.x;
                y_range.x = UL_hit.point.z;
            }
    
            RaycastHit UR_hit;
            Ray UR_ray = Cam.ScreenPointToRay(new Vector2(Input.mousePosition.x, ClickPos.y));
            if (Physics.Raycast(UR_ray, out UR_hit))
                x_range.y = UR_hit.point.x;

            RaycastHit LL_hit;
            Ray LL_ray = Cam.ScreenPointToRay(new Vector2(ClickPos.x, Input.mousePosition.y));
            if (Physics.Raycast(LL_ray, out LL_hit))
                y_range.y = LL_hit.point.z;

            //Select_Debug_Markers[0].transform.position = new Vector3(x_range.x,1f,y_range.x);
            //Select_Debug_Markers[1].transform.position = new Vector3(x_range.y,1f,y_range.x);
            //Select_Debug_Markers[2].transform.position = new Vector3(x_range.x,1f,y_range.y);

            Mc.Select(x_range,y_range);

            //Debug.Log("X-Range : " + x_range.x + ", " + x_range.y);
            //Debug.Log("Y-Range : " + y_range.x + ", " + y_range.y);

        }

        Clear();

    }

    void Clear()
    {

        Top.anchoredPosition = Vector2.zero;
        Bottom.anchoredPosition = Vector2.zero;
        Left.anchoredPosition = Vector2.zero;
        Right.anchoredPosition = Vector2.zero;

        Top.sizeDelta = Vector2.zero;
        Bottom.sizeDelta = Vector2.zero;
        Left.sizeDelta = Vector2.zero;
        Right.sizeDelta = Vector2.zero;

    }
}
