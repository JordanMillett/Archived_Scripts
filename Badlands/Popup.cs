using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Popup : MonoBehaviour
{
    public static Popup Selected;

    public Transform Target;
    public bool Selectable = false;
    [HideInInspector]
    public bool SelectorActive = false;
    public RectTransform Selector;
    public List<RectTransform> Selectables;
    [HideInInspector]
    public int SelectorIndex = 0;
    
    RectTransform RT;
    RectTransform ParentRT;
    public Camera Cam;

    protected virtual void OnEnable()
    {
        if(Selectable)
            Selected = this;
        SelectorIndex = 0;
    }
    
    void OnDisable()
    {
        if (Selectable)
        {
            if (Selected == this)
                Selected = null;
        }
    }

    protected virtual void Start()
    {
        RT = GetComponent<RectTransform>();
        ParentRT = this.transform.parent.GetComponent<RectTransform>();
    }

    protected virtual void Update()
    {
        //Selector.GetComponent<RawImage>().enabled = Selected;

        if (Selected == this)
        {
            if (SelectorActive)
            {
                if (Input.mouseScrollDelta.y > 0 && SelectorIndex != 0)
                    SelectorIndex--;
                if (Input.mouseScrollDelta.y < 0 && SelectorIndex != Selectables.Count - 1)
                    SelectorIndex++;

                Selector.anchoredPosition = Selectables[SelectorIndex].anchoredPosition;
                //selector position to size of thingy
            }
        }
        
        if(Target)
        {
            Vector3 ScreenSpace = Cam.WorldToScreenPoint(Target.transform.position);
            Vector2 Result;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(ParentRT, ScreenSpace, Cam, out Result);

            RT.anchoredPosition = Result;
        }
        
        
        //Transform.GetSiblingIndex
        //Transform.SetSiblingIndex
        //Transform.SetAsLastSibling
        //Transform.SetAsFirstSibling
        //Change with distance to player
    }
}
