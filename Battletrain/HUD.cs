using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Canvas C;
    public Player P;
    public GameObject Crosshair;
    
    void Update()
    {
        if(P.OnGun)
        {
            Crosshair.SetActive(true);
            
            Vector2 movePos;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(C.transform as RectTransform, Input.mousePosition, C.worldCamera, out movePos);

            Vector3 mousePos = C.transform.TransformPoint(movePos);
            
            Crosshair.transform.position = mousePos;
            
        }else
        {
            Crosshair.SetActive(false);
        }
    }
}
