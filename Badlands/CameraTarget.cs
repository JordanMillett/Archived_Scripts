using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public Transform Target;

    Vector3 GoalZoom = Vector3.one;
    float ZoomAlpha = 0.5f;//1f

    //public AnimationCurve CamPanCurve;

    Transform Cam;
    Vector3 CamAngles;

    void Start()
    {
        Cam = this.transform.GetChild(0).transform;
        CamAngles = Cam.localEulerAngles;
    }

    void Update()
    {
        /*
        if (Input.mouseScrollDelta.y > 0f)
            ZoomAlpha += 0.25f;
        if (Input.mouseScrollDelta.y < 0f)
            ZoomAlpha -= 0.25f;
        ZoomAlpha = Mathf.Clamp(ZoomAlpha, 0f, 1f);*/

        if(UIManager.UI.CurrentScreen == "Main" || UIManager.UI.CurrentScreen == "World")
            Cam.localEulerAngles = new Vector3(CamAngles.x, CamAngles.y, Time.time);
        else
            Cam.localEulerAngles = CamAngles;

        GoalZoom = Vector3.Lerp(Vector3.one * 1.5f, Vector3.one, ZoomAlpha);
        this.transform.localScale = Vector3.Lerp(this.transform.localScale, GoalZoom, Time.deltaTime * 3f);
    }

    void FixedUpdate()
    {
        Vector3 MousePos = Input.mousePosition;

        MousePos.x -= Screen.width/2;
        MousePos.y -= Screen.height/2;

        MousePos.x /= Screen.width;
        MousePos.y /= Screen.height;

        MousePos *= 2f;

        MousePos.x = Mathf.Clamp(MousePos.x, -0.6f, 0.6f);
        MousePos.y = Mathf.Clamp(MousePos.y, -0.6f, 0.6f);

        MousePos *= 5f;
        //use just screen width cause that's the widest and it would make it a box when moving mouse

        //Debug.Log(MousePos);

        //MousePos.x *= 1f + Mathf.Lerp(0f, 5f, CamPanCurve.Evaluate(Mathf.Min(1f, Mathf.Abs(MousePos.x))));
        //MousePos.y *= 1f + Mathf.Lerp(0f, 5f, CamPanCurve.Evaluate(Mathf.Min(1f, Mathf.Abs(MousePos.y))));

        

        Vector3 LookOffset = Vector3.zero;

        LookOffset.x = MousePos.x;
        LookOffset.z = MousePos.y;
        
        LookOffset = Quaternion.AngleAxis(45, Vector3.up) * LookOffset;

        if (Target)
        {
            if(Player.P.SpawnedIn)
                this.transform.position = Vector3.Lerp(this.transform.position, Target.position + LookOffset, Time.fixedDeltaTime * 4f);
            else
                this.transform.position = Target.position;
        }
    }
}
